using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGradientBlend : MonoBehaviour
{
    // Total duration for the full gradient cycle (red -> yellow -> green)
    public float cycleDuration = 5f;

    // The gradient used for color transitions
    public Gradient colorGradient;

    // Reference to the Renderer component
    private Renderer objRenderer;

    // Property ID for the emissive color
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        // Get the Renderer component attached to the GameObject
        objRenderer = GetComponent<Renderer>();

        if (objRenderer == null)
        {
            Debug.LogError("GameObject does not have a Renderer component.");
            enabled = false; // Disable the script if no Renderer is found
            return;
        }

        // Enable the emission keyword so the material will glow
        objRenderer.material.EnableKeyword("_EMISSION");

        // If no gradient is assigned, create a default red -> yellow -> green gradient
        if (colorGradient == null)
        {
            colorGradient = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[3];
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[3];

            // Color keys at the respective times
            colorKey[0].color = Color.red;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.yellow;
            colorKey[1].time = 0.5f;
            colorKey[2].color = Color.green;
            colorKey[2].time = 1.0f;

            // Alpha keys for full opacity
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 0.5f;
            alphaKey[2].alpha = 1.0f;
            alphaKey[2].time = 1.0f;

            colorGradient.SetKeys(colorKey, alphaKey);
        }
    }

    void Update()
    {
        // Calculate time as a percentage of the total cycle duration
        float time = Mathf.PingPong(Time.time / cycleDuration, 1f);

        // Evaluate the gradient at the calculated time
        Color currentColor = colorGradient.Evaluate(time);

        // Apply the color to the material's emissive color property
        objRenderer.material.SetColor(EmissionColor, currentColor);
    }
}