using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldController : MonoBehaviour
{
    public Material forceFieldMaterial;
    public Transform muscleTransform;

    [Range(0f, 2f)] public float baseRadius = 0.5f;
    [Range(0f, 2f)] public float baseThickness = 0.1f;

    public float radiusScaleMultiplier = 0.5f;
    public float thicknessScaleMultiplier = 0.05f;

    void Update()
    {
        if (forceFieldMaterial == null || muscleTransform == null) return;

        // Set shader _Center to muscle's world position
        Vector3 center = muscleTransform.position;
        forceFieldMaterial.SetVector("_Center", new Vector4(center.x, center.y, center.z, 1f));
        //forceFieldMaterial.SetVector("_Color", new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
        // Get current scale (assuming uniform scale for radius)
        float scale = muscleTransform.localScale.x;

        // Set dynamic radius and thickness based on scale
        float dynamicRadius = baseRadius + scale * radiusScaleMultiplier;
        float dynamicThickness = baseThickness + scale * thicknessScaleMultiplier;

        forceFieldMaterial.SetFloat("_Radius", dynamicRadius);
        forceFieldMaterial.SetFloat("_Thickness", dynamicThickness);
    }
}
