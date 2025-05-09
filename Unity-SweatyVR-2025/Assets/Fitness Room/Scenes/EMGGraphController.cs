using UnityEngine;
using XCharts;
using System.IO;
using System.Collections.Generic;
using XCharts.Runtime;
using System.Collections;

public class EMGGraphController : MonoBehaviour
{
    [Header("CSV and Chart Settings")]
    public LineChart lineChart; 
    public TextAsset csvFile; // Assign your CSV file here in the Inspector.
    public float updateInterval = 0.016f; // Time between updates in seconds. // ~60 FPS (16ms)
    private const int maxDataPoints = 60; 


    private List<float> timeData = new List<float>();//stores time from csv
    private List<float> emgData = new List<float>(); //stores emg data, normalized freom 0-1
    private int currentLineIndex = 0; 
    private string[] dataLines; //holds csv lines after being split
    private float timer = 0f; // timer
    private List<(float time, float emg)> parsedData = new List<(float, float)>();


    [Header("EMG Range Mapping")]
    private float minEMG = -0.3967f;
    private float maxEMG = 0.46f;

    [Header("Muscle Shader & Pulse Effect")]
    public List<Material> muscleMaterial; //shader material for muscles
    //Adding Pulse effect
    public List<Transform> muscleTransform;
    public float scaleMultiplier = 0.05f;
    public float smoothSpeed = 5f; // Higher = faster lerp

    private List<Vector3> originalScale = new List<Vector3>();
    private List<Vector3> targetScale = new List<Vector3>();
    private List<Vector3> currentScale = new List<Vector3>();

    [Header("Breathing Animation")]
    public float breathingAmplitude = 0.01f;
    public float breathingSpeed = 2f;

    [Header("Force Field Shader Settings")]
    [SerializeField] private Material forceFieldMaterial;
    [SerializeField] private Color minForceFieldColor = Color.white;
    [SerializeField] private Color maxForceFieldColor = Color.red;

    [SerializeField] private Gradient emgGradient;



    void Start()
    {
        StartCoroutine(LoadCSVAsync());
        //LoadCSV(); // read data from csv
        lineChart.ClearData(); // clear chart data

        foreach(Transform muscle in muscleTransform)
        {
            if (muscle != null)
            {
                Vector3 original = muscle.localScale;
                originalScale.Add(original);
                currentScale.Add(original);
                targetScale.Add(original);
            }
        }
    }

    void Update() 
    {
        timer += Time.deltaTime; 

        if (timer >= updateInterval) // check if time exceeds update interval, reset timer and move to next line
        {
            timer = 0f;
            UpdateChartRealTime();

            for(int i = 0; i < muscleTransform.Count; i++)
            {
                if (muscleTransform[i] != null)
                {
                    currentScale[i] = Vector3.Lerp(currentScale[i], targetScale[i], smoothSpeed * Time.deltaTime);
                    muscleTransform[i].localScale = currentScale[i];
                }
            }
        }
    }

/*    void LoadCSV()
    {
        dataLines = csvFile.text.Split('\n'); // solits scv into lines
        currentLineIndex = 1; // Skip the header row
        for (int i = currentLineIndex; i < dataLines.Length; i++)
        {
            string[] data = dataLines[i].Split(',');
            if (data.Length == 2 && float.TryParse(data[0], out float time) && float.TryParse(data[1], out float emg))
            {
                parsedData.Add((time, emg));
            }
        }
    }*/

    IEnumerator LoadCSVAsync()
    {
        dataLines = csvFile.text.Split('\n');
        for (int i = 1; i < dataLines.Length; i++)
        {
            string[] data = dataLines[i].Split(',');
            if (data.Length == 2 && float.TryParse(data[0], out float time) && float.TryParse(data[1], out float emg))
            {
                parsedData.Add((time, emg));
            }

            if (i % 100 == 0) yield return null; // Prevent freezing on large files
        }
    }

    void UpdateChartRealTime()
    {
        /* if (currentLineIndex < parsedData.Count)*/
        if (currentLineIndex >= parsedData.Count) 
            { return; 
            }

        {
            string[] data = dataLines[currentLineIndex].Split(','); // split data into time and emg 

            if (data.Length == 2 && float.TryParse(data[0], out float time) && float.TryParse(data[1], out float emg)) // convert to float
            {
                // Map EMG value to a normalized range between -1 and 1
                //float mappedEMG = Mathf.LerpUnclamped(-1f, 1f, (emg - minEMG) / (maxEMG - minEMG));
                float mappedEMG = Mathf.InverseLerp(minEMG, maxEMG, emg); // map values form [0,1]
                Color currentColor = emgGradient.Evaluate(mappedEMG);

                // Add new data points to lists
                timeData.Add(time);
                emgData.Add(mappedEMG);

                if (timeData.Count > maxDataPoints)
                {
                    timeData.RemoveAt(0);
                    emgData.RemoveAt(0);
                }

                /* if (timeData.Count > 1)
            {
                float lastTime = timeData[timeData.Count - 1];
                float windowStart = Mathf.Max(0, lastTime - 10f); // Example window size: 10 seconds
                   
                    //lineChart.chartWidth.Equals(windowStart);
                    lineChart.GetChartComponent<XAxis>().max = lastTime;
                    //lineChart.XAxises[0].max = lastTime;
                }*/
                float lastTime = timeData[timeData.Count - 1];
                lineChart.GetChartComponent<XAxis>().max = lastTime; // Ensure X-axis updates


                lineChart.GetSerie(0).ClearData();
                for (int i = 0; i < timeData.Count; i++)
                {
                    lineChart.AddData(0, timeData[i], emgData[i]);
                }

                // Update shader with mapped EMG value
                
                foreach (Material mat in muscleMaterial)  // Ensure all materials on each renderer are updated
                {
                    mat.SetFloat("_EMG_Value", mappedEMG);
                }

                if (forceFieldMaterial != null)
                {
                    //Color currentColor = Color.Lerp(minForceFieldColor, maxForceFieldColor, mappedEMG);
                    forceFieldMaterial.SetColor("_Color", currentColor);
                    forceFieldMaterial.SetFloat("_EMG_Value", mappedEMG); // If also controlling emission/intensity


                }

                /*foreach (Transform muscle in muscleTransform)
                {
                    if(muscle != null)
                    {
                        float scaleFactor = 1f + (mappedEMG * scaleMultiplier);
                        muscle.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
                    }
                }*/

                for (int i = 0; i < muscleTransform.Count; i++)
                    {
                        if (muscleTransform[i] != null)
                        {
                            float scaleFactor = 1f + (mappedEMG * scaleMultiplier);
                            float breathingEffect = 1f + Mathf.Sin(Time.time * breathingSpeed) * breathingAmplitude;
                            scaleFactor *= breathingEffect; // add breathing effect over pulse

                            Vector3 original = originalScale[i];
                            targetScale[i] = new Vector3(original.x, original.y * scaleFactor, original.z * scaleFactor);
                        //    targetScale[i] = originalScale[i] * scaleFactor;

                    }
                }
                 //Debug.Log($"Raw EMG: {emg}, Mapped EMG: {mappedEMG}");
                //Debug.Log($"Setting Shader EMG Value: {mappedEMG}");
            }
            currentLineIndex++;
           // Debug.Log("Data count: " + lineChart.GetSerie(0).dataCount);

        }
    }
    public void AddRealTimeDataPoint(float time, float emgValue)
    {
        // Add new data point
        timeData.Add(time);
        emgData.Add(emgValue);

        // Maintain the maximum number of data points
        if (timeData.Count > maxDataPoints)
        {
            timeData.RemoveAt(0);
            emgData.RemoveAt(0);
        }

        // Update the chart
        lineChart.GetSerie(0).ClearData();
        for (int i = 0; i < timeData.Count; i++)
        {
            lineChart.AddData(0, timeData[i], emgData[i]);
        }
    }
}
