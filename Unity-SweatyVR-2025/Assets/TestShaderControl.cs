using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShaderControl : MonoBehaviour
{
    public Material testMaterial; // Assign your material in the Inspector
    [Range(0, 1)] public float testValue = 0.5f; // Adjustable slider in the Inspector

    private static readonly int EmgValueProperty = Shader.PropertyToID("_EMGValue");

    void Update()
    {
        if (testMaterial != null)
        {
            // Update the shader's _EMGValue property based on the slider
            testMaterial.SetFloat(EmgValueProperty, testValue);
        }
        else
        {
            Debug.LogWarning("Material not assigned!");
        }
    }
}
