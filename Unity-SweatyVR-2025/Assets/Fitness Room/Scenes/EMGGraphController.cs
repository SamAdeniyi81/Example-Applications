using UnityEngine;
using XCharts;
using System.IO;
using System.Collections.Generic;
using XCharts.Runtime;
using System.Collections;

public class EMGGraphController : MonoBehaviour
{
    public LineChart lineChart; // reference to line chart
    public TextAsset csvFile; // Assign your CSV file here in the Inspector.
    public float updateInterval = 0.016f; // Time between updates in seconds. // ~60 FPS (16ms)
    private const int maxDataPoints = 60; // Maximum number of points to display


    private List<float> timeData = new List<float>();//stores time from csv
    private List<float> emgData = new List<float>(); //stores emg data, normalized freom 0-1
    private int currentLineIndex = 0; // track current poisiton when reading csv
    private string[] dataLines; //holds csv lines after being split

    private float timer = 0f; // timer

    /// <summary>
    /// new stuff
    public List<Material> muscleMaterial; //shader material

    // Observed EMG data range
    private float minEMG = -0.3967f;
    private float maxEMG = 0.46f;
    private List<(float time, float emg)> parsedData = new List<(float, float)>();

    /// </summary>

    void Start()
    {
        StartCoroutine(LoadCSVAsync());
        //LoadCSV(); // read data from csv
        lineChart.ClearData(); // clear chart data
    }

    void Update() 
    {
        timer += Time.deltaTime; // set timer

        if (timer >= updateInterval) // check if time exceeds update interval, reset timer and move to next line
        {
            timer = 0f;
            UpdateChartRealTime();
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

            if (i % 100 == 0) yield return null; // Prevents freezing on large files
        }
    }

    void UpdateChartRealTime()
    {
        if (currentLineIndex < parsedData.Count)
        {
            string[] data = dataLines[currentLineIndex].Split(','); // split data into time and emg 

            if (data.Length == 2 && float.TryParse(data[0], out float time) && float.TryParse(data[1], out float emg)) // convert to float
            {
                // Map EMG value to a normalized range between -1 and 1
                //float mappedEMG = Mathf.LerpUnclamped(-1f, 1f, (emg - minEMG) / (maxEMG - minEMG));
                float mappedEMG = Mathf.InverseLerp(minEMG, maxEMG, emg); // map values form [0,1]

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
