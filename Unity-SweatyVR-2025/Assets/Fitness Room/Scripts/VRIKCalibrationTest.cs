using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

namespace RootMotion.Demos
{

    public class VRIKCalibrationTest : MonoBehaviour
    {
[Tooltip("The VRIK component.")] public VRIK ik;

        [Header("Head")]
        [Tooltip("HMD.")] public Transform centerEyeAnchor;
        [Tooltip("Position offset of the camera from the head bone (root space).")] public Vector3 headAnchorPositionOffset;
        [Tooltip("Rotation offset of the camera from the head bone (root space).")] public Vector3 headAnchorRotationOffset;

        [Header("Hands")]
        [Tooltip("Left Hand Controller")] public Transform leftHandAnchor;
        [Tooltip("Right Hand Controller")] public Transform rightHandAnchor;
        [Tooltip("Position offset of the hand controller from the hand bone (controller space).")] public Vector3 handAnchorPositionOffset;
        [Tooltip("Rotation offset of the hand controller from the hand bone (controller space).")] public Vector3 handAnchorRotationOffset;

        [Header("Pelvis")]
        [Tooltip("Hips")] public Transform pelvisAnchor;

         [Header("Legs")]
        [Tooltip("Left Leg")] public Transform leftUpLegAnchor;

        [Tooltip("Left Knee")] public Transform leftLegAnchor;
        
        [Tooltip("Left Foot")] public Transform leftFootAnchor;
        [Tooltip("Right Leg")] public Transform rightUpLegAnchor;

        [Tooltip("Right Knee")] public Transform rightLegAnchor;
        [Tooltip("Right Foot")] public Transform rightFootAnchor;

        [Header("Scale")]
        [Tooltip("Multiplies the scale of the root.")] public float scaleMlp = 1f;

        [Header("Data stored by Calibration")]
        public VRIKCalibrator.CalibrationData data = new VRIKCalibrator.CalibrationData();

        List<float> userAnglesList = new List<float>();

        public Text aboveHeadText;

        // public Transform target;

        private void Start()
        {

        }

        private void Update()
        {
            
         //Calculate model height
        //    float modelHeight = centerEyeAnchor.transform.localPosition.y;

         // Thigh = leftLegAnchor - leftUpLegAnchor
         // Shin = leftFootAnchor - leftLegAnchor
           Vector3 thighVector = leftLegAnchor.transform.position - leftUpLegAnchor.transform.position; 
           Vector3 shinVector = leftFootAnchor.transform.position - leftLegAnchor.transform.position;

           //Displays a colored line(show in Unity Scene only) visualizing the vectors of the avatars lower body components that we want to calculate
        //    Debug.DrawLine(leftFootAnchor.transform.position, leftLegAnchor.transform.position, Color.white);
        //    Debug.DrawLine(leftLegAnchor.transform.position, leftUpLegAnchor.transform.position, Color.white);

            //Show the knee angle of the Avatar each frame
            getVectorsAngle(shinVector, thighVector);

            // if (Input.GetKeyDown(KeyCode.A))
            // {
            //     Debug.Log("======================== Testing ==========================");
            //     // print("Model Height = " + modelHeight);

            //     //calculate angle using inverse tangent method
            //     // float angle = Mathf.Atan2(leftKneeVector3.y, leftKneeVector3.x)  * Mathf.Rad2Deg - 90;
            //     // print("Angle = " + angle);

            //      print("Thigh vector = " + thighVector);
            //      print("Shin vector = " + shinVector);
            //     //  print(("getVectorsAngle = " + getVectorsAngle(shinVector, thighVector));
            //      getVectorsAngle(shinVector, thighVector);

                // print("lowestAngle = " + lowestAngle);
                // print("highestAngle = " + highestAngle);
            // }
        }

        public void CalibrateAvatar()
        {

             // Get height of the Oculus Headset
            float defaultHeight = 2.0f;
            float modelHeight = centerEyeAnchor.transform.localPosition.y;
            // float scale = modelHeight/defaultHeight;
            // ik.transform.localScale = Vector3.one * scale;

            // print(modelHeight);

            //Set Avatar scale to the newly calculated scale(modelHeight/defaultAvatarHeight); 
            ik.transform.localScale = new Vector3(modelHeight/defaultHeight, modelHeight/defaultHeight, modelHeight/defaultHeight);
 /* 
            float sizeF = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);
            ik.transform.localScale *= sizeF;
            

            Debug.Log("headtarget: " + ik.solver.spine.headTarget);
            Debug.Log("root position1: " + ik.references.root.name); 
            Debug.Log("head position: " + ik.references.head); 
            Debug.Log("root position2: " + ik.references.root); 
            Debug.Log("sizef: " + sizeF);
            Debug.Log("local scale: " + ik.transform.localScale);
 */






            // if (Input.GetKeyDown(KeyCode.C))
            // {
                // Calibrate the character, store data of the calibration
                // data = VRIKCalibrator.Calibrate(ik, centerEyeAnchor, leftHandAnchor, rightHandAnchor, headAnchorPositionOffset, headAnchorRotationOffset, handAnchorPositionOffset, handAnchorRotationOffset, scaleMlp);
                
            // }

            /*
            * calling Calibrate with settings will return a VRIKCalibrator.CalibrationData, which can be used to calibrate that same character again exactly the same in another scene (just pass data instead of settings), 
            * without being dependent on the pose of the player at calibration time.
            * Calibration data still depends on bone orientations though, so the data is valid only for the character that it was calibrated to or characters with identical bone structures.
            * If you wish to use more than one character, it would be best to calibrate them all at once and store the CalibrationData for each one.
            * */
            // if (Input.GetKeyDown(KeyCode.D))
            // {
            //     if (data.scale == 0f)
            //     {
            //         Debug.LogError("No Calibration Data to calibrate to, please calibrate with 'C' first.");
            //     }
            //     else
            //     {
                    // VRIKCalibrator.Calibrate(ik, data, centerEyeAnchor, null, leftHandAnchor, rightHandAnchor);
            //     }
            // }

            // // // Recalibrates avatar scale only. Can be called only if the avatar has been calibrated already.
            // if (Input.GetKeyDown(KeyCode.S))
            // {
            //     if (data.scale == 0f)
            //     {
            //         Debug.LogError("Avatar needs to be calibrated before RecalibrateScale is called.");
            //     }
            //     VRIKCalibrator.RecalibrateScale(ik, data, scaleMlp);
            // }
        }

        private void getVectorsAngle(Vector3 a, Vector3 b)
        {

            //Assume that we have vector3 value
            //First, take dot product of Vectors a and b
            float dotProductAB = Vector3.Dot(a,b);

            //Determine the magnitude of each vector
            float magnitudeA = Mathf.Abs(Vector3.Magnitude(a));
            float magnitudeB = Mathf.Abs(Vector3.Magnitude(b));

            //Use transformed dot product equation
            float alpha = ((dotProductAB)/(magnitudeA*magnitudeB));
            float Arcos = Mathf.Acos(alpha);
            alpha = (int)((Arcos * 180f / Mathf.PI) - 180f);

            aboveHeadText.text =  alpha.ToString();

 
           }       
    }

}
