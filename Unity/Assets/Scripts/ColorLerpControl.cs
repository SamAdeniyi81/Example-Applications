using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ColorLerpControl : MonoBehaviour
{

    // CSV file path (relative to the Assets folder)
    public string csvFilePath = "Assets/Scripts/Test_EMG_data.csv";

    
    private Color minColor = Color.clear;     // Color for minimum EMG signal
    private Color maxColor = Color.green;   // Color for maximum EMG signal

    // EMG signal range
    public float minEMGValue = 0f;  
    public float maxEMGValue = 1000f;
   

    //Data storage
    private List<float> emgData = new List<float>();
    private float timeElapsed = 0f;
    private int currentIndex = 0;

    private Renderer objRenderer;
    // Property ID for the emissive color
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    //public RotateFence fenceRotator; // Reference to RotateFence script -- will use later likely


    void Start()
    {
        // Get the Renderer component attached to the GameObject
        objRenderer = GetComponent<Renderer>();
        if (objRenderer == null)
        {
            Debug.LogError("GameObject does not have a Renderer component.");
            enabled = false;
            return;
        }

        // Enable the emission keyword so the material will glow
        objRenderer.material.EnableKeyword("_EMISSION");

        LoadEMGData();

    }

    void Update()
    {
        if (emgData.Count == 0 || currentIndex >= emgData.Count) 
            return;

        // Simulate time progression and get current EMG signal
        float timeMultiplier = 1.0f; // Adjust speed preference
        timeElapsed += Time.deltaTime * timeMultiplier;

        if (currentIndex < emgData.Count)
        {
            float emgSignal = emgData[currentIndex]; 
            //normalize emg signal(0 - 1)
            float normalizedValue = Mathf.Clamp01((emgSignal - minEMGValue) / (maxEMGValue - minEMGValue));

            // Lerp between colors based on the normalized value
            Color currentColor = Color.Lerp(minColor, maxColor, normalizedValue);
            objRenderer.material.SetColor(EmissionColor, currentColor);

           // fenceRotator.RotateOnEmgInput(normalizedValue); -- will use later likely

            // Advance to next index if necessary
            if (timeElapsed >= 0.05f) // Adjust to match the interval of your CSV data frequency
            {
                currentIndex++;
                timeElapsed = 0f;
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
                if (values.Length >= 2 && float.TryParse(values[1], out float emgValue))
                {
                    emgData.Add(emgValue);
                }
            }
        }
    }
}



// Lerp between 3 colors//

/*public float lerpDuration = 5f;

  // Start and end colors for the emissive glow
  private Color startColor = Color.red;
  private Color middleColor = Color.yellow;
  private Color endColor = Color.green;

  // Internal timer to track lerp progress
  private float lerpTimer = 0f;

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
      // Update the timer
      lerpTimer += Time.deltaTime;

      // Calculate the lerp factor (clamped between 0 and 1)
      float lerpFactor = Mathf.PingPong(lerpTimer / lerpDuration, 1f);

      Color currentColor;

      // Determine the current phase and lerp between appropriate colors
      if (lerpFactor < 0.5f)
      {
          // First half: red to yellow
          float adjustedLerpFactor = lerpFactor * 2f; // Scale factor for the first phase
          currentColor = Color.Lerp(startColor, middleColor, adjustedLerpFactor);
      }
      else
      {
          // Second half: yellow to green
          float adjustedLerpFactor = (lerpFactor - 0.5f) * 2f; // Scale factor for the second phase
          currentColor = Color.Lerp(middleColor, endColor, adjustedLerpFactor);
      }


      // Apply the color to the material's emissive color property
      objRenderer.material.SetColor(EmissionColor, currentColor);
  }
}*/
