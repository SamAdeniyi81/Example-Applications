using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;     // make sure Oculus Integration package is imported
using OVR;                // for OVRInput

[RequireComponent(typeof(BodyPartToggle))]
public class OculusBodyPartClicker : MonoBehaviour
{
    [Header("Pointer Settings")]
    [Tooltip("The transform of your right‐hand controller (tip)")]
    public Transform rightHandController;

    [Tooltip("Max distance for your pointer ray")]
    public float maxDistance = 10f;

    [Tooltip("Layer mask for all clickable body‐part meshes")]
    public LayerMask bodyPartLayer;

    // Cache a reference to your toggle script
    private BodyPartToggle toggleScript;

    void Awake()
    {
        toggleScript = GetComponent<BodyPartToggle>();
    }

    void Update()
    {
        // 1) Get the forward ray from the controller
        Ray ray = new Ray(rightHandController.position, rightHandController.forward);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.cyan);

        // 2) Only when the user *just* pulled the index trigger
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, bodyPartLayer))
            {
                // 3) Tell your existing script to toggle *that* GameObject
                toggleScript.ToggleBodyPart(hit.collider.gameObject);
            }
        }
    }
}
