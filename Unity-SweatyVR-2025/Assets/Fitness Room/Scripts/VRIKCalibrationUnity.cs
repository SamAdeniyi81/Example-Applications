using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.InputSystem;
using Valve.VR;

public class VRIKCalibrationUnity : MonoBehaviour
{
    [Tooltip("Reference to the VRIK component on the avatar.")] public VRIK ik;
    [Tooltip("The settings for VRIK calibration.")] public VRIKCalibrator.Settings settings;
    [Tooltip("The HMD.")] public Transform headTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.")] public Transform bodyTracker;
    [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.")] public Transform leftHandTracker;
    [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.")] public Transform rightHandTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.")] public Transform leftFootTracker;
    [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.")] public Transform rightFootTracker;

    [Header("Data stored by Calibration")]
    public VRIKCalibrator.CalibrationData data = new VRIKCalibrator.CalibrationData();

    //private InputAction calibrateAction;

    public SteamVR_Action_Boolean primaryButtonAction;

    void Awake()
    {
        // Initialize Input Action
        //calibrateAction = new InputAction(type: InputActionType.Button, binding: "<ValveIndexController>{RightHand}/primaryButton");
        //calibrateAction.Enable();
    }

    void OnDestroy()
    {
        //calibrateAction.Disable();
    }

    /*void LateUpdate()
    {
        if (calibrateAction.triggered)
        {
            CalibrateAvatar();
            Debug.Log("Primary button pressed!");
        }
    }
*/
    void Update()
    {
        // Check if the calibration button is pressed
        if (primaryButtonAction.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            CalibrateAvatar();
            Debug.Log("Controller button press detected");
        }
    }


    private void CalibrateAvatar()
    {
        // Perform calibration and store calibration data
        data = VRIKCalibrator.Calibrate(ik, settings, headTracker, bodyTracker, leftHandTracker, rightHandTracker, leftFootTracker, rightFootTracker);
        Debug.Log("Calibration complete.");
    }
}
