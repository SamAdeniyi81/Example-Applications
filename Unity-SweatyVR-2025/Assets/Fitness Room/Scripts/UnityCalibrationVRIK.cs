using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;
using UnityEngine.Events;

public class UnityCalibrationVRIK : MonoBehaviour
{
    [System.Serializable]
    public class CalibrationEvent : UnityEvent<float> { }

    [Header("VRIK Setup")]
    [SerializeField] private VRIK ik;

    [Header("Tracking Anchors")]
    [SerializeField] private Transform centerEyeAnchor;
    [SerializeField] private Transform leftHandAnchor;
    [SerializeField] private Transform rightHandAnchor;
    [SerializeField] private Transform pelvisAnchor;

    [Header("Leg Tracking")]
    [SerializeField] private Transform leftUpLegAnchor;
    [SerializeField] private Transform leftLegAnchor;
    [SerializeField] private Transform leftFootAnchor;
    [SerializeField] private Transform rightUpLegAnchor;
    [SerializeField] private Transform rightLegAnchor;
    [SerializeField] private Transform rightFootAnchor;

    [Header("Offset Adjustments")]
    [SerializeField] private Vector3 headAnchorPositionOffset;
    [SerializeField] private Vector3 headAnchorRotationOffset;
    [SerializeField] private Vector3 handAnchorPositionOffset;
    [SerializeField] private Vector3 handAnchorRotationOffset;

    [Header("Scaling")]
    [SerializeField] private float scaleMlp = 1f;
    [SerializeField] private float defaultAvatarHeight = 2.0f;
    [SerializeField] private Vector2 validHeightRange = new Vector2(1f, 2.5f);

    /*[Header("UI References")]
    [SerializeField] private Text angleDisplayText;
    [SerializeField] private Text calibrationStatusText;*/

    [Header("Debug Visualization")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color legGizmosColor = Color.white;

    [Header("Events")]
    public CalibrationEvent OnCalibrationComplete;
    public CalibrationEvent OnKneeAngleChanged;

    // Stored calibration data
    private VRIKCalibrator.CalibrationData calibrationData = new VRIKCalibrator.CalibrationData();
    private List<float> kneeAngleHistory = new List<float>();
    private bool isCalibrated = false;
    private Vector3 initialScale;

    private void Start()
    {
        ValidateComponents();
        initialScale = ik.transform.localScale;
    }

    private void ValidateComponents()
    {
        if (ik == null)
        {
            ik = GetComponent<VRIK>();
            if (ik == null)
            {
                Debug.LogError("No VRIK component found on GameObject or in reference!", this);
                enabled = false;
                return;
            }
        }

        // Validate essential transforms
        if (centerEyeAnchor == null || leftHandAnchor == null || rightHandAnchor == null)
        {
            Debug.LogError("Essential tracking anchors are missing!", this);
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (isCalibrated)
        {
            UpdateKneeAngleCalculations();
        }
    }

    public void CalibrateAvatar()
    {
        StartCoroutine(CalibrationSequence());
    }

    private IEnumerator CalibrationSequence()
    {
      /*  if (!ValidateCalibrationSetup())
        {
            Debug.LogError("Calibration setup validation failed!");
            yield break;
        }*/

        // Wait for stable pose
        yield return new WaitForSeconds(0.5f);

        // Calculate and apply height scaling
        float modelHeight = centerEyeAnchor.transform.localPosition.y;
        if (modelHeight < validHeightRange.x || modelHeight > validHeightRange.y)
        {
            Debug.LogWarning($"Model height {modelHeight}m is outside valid range ({validHeightRange.x}m - {validHeightRange.y}m)");
            modelHeight = Mathf.Clamp(modelHeight, validHeightRange.x, validHeightRange.y);
        }

        float scale = modelHeight / defaultAvatarHeight;
        ik.transform.localScale = initialScale * scale;

        // Perform VRIK calibration
        try
        {
            calibrationData = VRIKCalibrator.Calibrate(
                ik,
                centerEyeAnchor,
                leftHandAnchor,
                rightHandAnchor,
                headAnchorPositionOffset,
                headAnchorRotationOffset,
                handAnchorPositionOffset,
                handAnchorRotationOffset,
                scaleMlp
            );

            isCalibrated = true;
            //UpdateCalibrationStatus("Calibration Complete");
            OnCalibrationComplete?.Invoke(scale);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Calibration failed: {e.Message}");
            //UpdateCalibrationStatus("Calibration Failed");
            isCalibrated = false;
        }
    }

    private void UpdateKneeAngleCalculations()
    {
        if (leftUpLegAnchor == null || leftLegAnchor == null || leftFootAnchor == null)
            return;

        Vector3 thighVector = leftLegAnchor.position - leftUpLegAnchor.position;
        Vector3 shinVector = leftFootAnchor.position - leftLegAnchor.position;

        float kneeAngle = CalculateKneeAngle(shinVector, thighVector);
        //UpdateKneeAngleDisplay(kneeAngle);

        if (showDebugGizmos)
        {
            Debug.DrawLine(leftFootAnchor.position, leftLegAnchor.position, legGizmosColor);
            Debug.DrawLine(leftLegAnchor.position, leftUpLegAnchor.position, legGizmosColor);
        }

        OnKneeAngleChanged?.Invoke(kneeAngle);
    }

    private float CalculateKneeAngle(Vector3 shinVector, Vector3 thighVector)
    {
        float dotProduct = Vector3.Dot(shinVector, thighVector);
        float magnitudeProduct = shinVector.magnitude * thighVector.magnitude;

        if (magnitudeProduct == 0)
            return 0f;

        float cosAngle = Mathf.Clamp(dotProduct / magnitudeProduct, -1f, 1f);
        float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg - 180f;

        // Add to history for smoothing if needed
        kneeAngleHistory.Add(angle);
        if (kneeAngleHistory.Count > 10)
            kneeAngleHistory.RemoveAt(0);

        return angle;
    }

   /* private bool ValidateCalibrationSetup()
    {
        if (ik == null || centerEyeAnchor == null || leftHandAnchor == null || rightHandAnchor == null)
        {
            UpdateCalibrationStatus("Missing Required Components");
            return false;
        }
        return true;
    }*/

  /*  private void UpdateCalibrationStatus(string status)
    {
        if (calibrationStatusText != null)
            calibrationStatusText.text = status;
        Debug.Log($"Calibration Status: {status}");
    }

    private void UpdateKneeAngleDisplay(float angle)
    {
        if (angleDisplayText != null)
            angleDisplayText.text = angle.ToString("F1");
    }
*/
    public void RecalibrateScale()
    {
        if (!isCalibrated || calibrationData.scale == 0f)
        {
            Debug.LogWarning("Cannot recalibrate scale before initial calibration");
            return;
        }

        VRIKCalibrator.RecalibrateScale(ik, calibrationData, scaleMlp);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        if (leftUpLegAnchor != null && leftLegAnchor != null && leftFootAnchor != null)
        {
            Gizmos.color = legGizmosColor;
            Gizmos.DrawWireSphere(leftUpLegAnchor.position, 0.05f);
            Gizmos.DrawWireSphere(leftLegAnchor.position, 0.05f);
            Gizmos.DrawWireSphere(leftFootAnchor.position, 0.05f);
        }
    }
}