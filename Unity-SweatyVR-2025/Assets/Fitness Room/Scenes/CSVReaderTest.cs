using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class CSVReaderTest : MonoBehaviour
{
    public string filePath = "Synthetic_EMG_Data.csv"; // Replace with your file name
    public List<float> time = new List<float>();
    public List<float> emgValues = new List<float>();

    void Start()
    {
        LoadCSV();
    }

    void LoadCSV()
    {
        string fullPath = Path.Combine(Application.dataPath, filePath);

        if (File.Exists(fullPath))
        {
            var lines = File.ReadAllLines(fullPath);
            bool isFirstLine = true;

            foreach (var line in lines)
            {
                if (isFirstLine) // Skip header
                {
                    isFirstLine = false;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line)) continue; // Skip empty lines

                var values = line.Split(',');
                if (values.Length < 2) continue; // Skip malformed lines

                try
                {
                    float timeValue = float.Parse(values[0].Trim(), CultureInfo.InvariantCulture);
                    float emgValue = float.Parse(values[1].Trim(), CultureInfo.InvariantCulture);

                    time.Add(timeValue);
                    emgValues.Add(emgValue);
                }
                catch (System.FormatException ex)
                {
                    Debug.LogError($"Error parsing line: {line}. Exception: {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogError("File not found: " + fullPath);
        }
    }
}
