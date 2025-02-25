using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class TrackerDebug : MonoBehaviour
{
    public SteamVR_Behaviour_Pose trackerPose;

    void Update()
    {
        if (trackerPose != null)
        {
            Debug.Log($"Tracker Position: {trackerPose.transform.position}, Rotation: {trackerPose.transform.rotation}");
        }
    }
}
