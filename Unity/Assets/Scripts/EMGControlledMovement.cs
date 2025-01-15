using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EMGControlledMovement : MonoBehaviour
{
    public string csvFilePath = "Assets/Scripts/Test_EMG_data.csv"; // CSV file path (relative to the Assets folder)

    public float minEMGValue = 0f;  // Minimum EMG value
    public float maxEMGValue = 1000f; // Maximum EMG value
    public float maxAcceleration = 10f; // Maximum acceleration of the cube
    public Vector3 movementDirection = Vector3.forward; // Direction of movement

    private List<float> emgData = new List<float>();
    private float timeElapsed = 0f;
    private int currentIndex = 0;
    private Rigidbody cubeRigidbody;

    void Start()
    {
        LoadEMGData(); // Load EMG data from CSV
    cubeRigidbody = GetComponent<Rigidbody>();

        if (cubeRigidbody == null)
        {
            Debug.LogError("GameObject does not have a Rigidbody component.");
            enabled = false; // Disable the script if no Rigidbody is found
            return;
        }
    }

    void Update()
{
    if (emgData.Count == 0 || currentIndex >= emgData.Count)
        return;

        float timeMultiplier = .5f; // Reduce the speed by half
        timeElapsed += Time.deltaTime * timeMultiplier;
        if (currentIndex < emgData.Count)
    {
        float emgSignal = emgData[currentIndex];
        float normalizedValue = Mathf.Clamp01((emgSignal - minEMGValue) / (maxEMGValue - minEMGValue));

        float currentAcceleration = normalizedValue * maxAcceleration;

        if (currentAcceleration > 0f)
        {
            cubeRigidbody.AddForce(movementDirection * currentAcceleration, ForceMode.Acceleration);
        }
        else
        {
            // Completely stop the cube's movement
            cubeRigidbody.velocity = Vector3.zero;
            cubeRigidbody.angularVelocity = Vector3.zero;
        }

        if (timeElapsed >= 0.05f) // Adjust to match the interval of your CSV data
        {
            currentIndex++;
            timeElapsed = 0f;

            // If the last line has been read, stop the cube completely
            if (currentIndex >= emgData.Count)
            {
                cubeRigidbody.velocity = Vector3.zero;
                cubeRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}

void LoadEMGData()
{
    if (!File.Exists(csvFilePath))
    {
        Debug.LogError("CSV file not found.");
        return;
    }

    using (StreamReader sr = new StreamReader(csvFilePath))
    {
        bool headerRead = false;
        while (!sr.EndOfStream)
        {
            string line = sr.ReadLine();
            if (!headerRead)
            {
                headerRead = true; // Skip the header line
                continue;
            }

            string[] values = line.Split(',');
            if (values.Length >= 2)
            {
                if (float.TryParse(values[1], out float emgValue))
                {
                    emgData.Add(emgValue);
                }
                }
            }
        }
    }
}

  /*  // Total duration for the full cycle (red -> yellow -> green)
    public float cycleDuration = 5f;

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
    }

    void Update()
    {
        // Calculate time as a percentage of the total cycle duration
        float time = Time.time / cycleDuration;

        // Use sine functions to transition between the colors
        float r = Mathf.Sin(time * Mathf.PI * 2) * 0.5f + 0.5f; // Red channel varies from 0 to 1
        float g = Mathf.Sin(time * Mathf.PI * 2 + Mathf.PI / 2) * 0.5f + 0.5f; // Green channel, phase-shifted
        float b = 0f; // Blue channel remains constant (not used in this case)

        // Combine the channels into a color
        Color currentColor = new Color(r, g, b);

        // Apply the color to the material's emissive color property
        objRenderer.material.SetColor(EmissionColor, currentColor);*/
/*    }
}
*/