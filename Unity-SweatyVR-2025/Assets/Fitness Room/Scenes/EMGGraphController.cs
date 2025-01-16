using UnityEngine;
using XCharts;
using System.IO;
using System.Collections.Generic;
using XCharts.Runtime;

public class EMGGraphController : MonoBehaviour
{
    public LineChart lineChart;
    public TextAsset csvFile; // Assign your CSV file here in the Inspector.
    public float updateInterval = 0.1f; // Time between updates in seconds.
    private const int maxDataPoints = 50; // Maximum number of points to display


    private List<float> timeData = new List<float>();
    private List<float> emgData = new List<float>();
    private int currentLineIndex = 0;
    private string[] dataLines;

    private float timer = 0f;

    void Start()
    {
        LoadCSV();
        lineChart.ClearData();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateChartRealTime();
        }
    }

    void LoadCSV()
    {
        dataLines = csvFile.text.Split('\n');
        currentLineIndex = 1; // Skip the header row.
    }

    void UpdateChartRealTime()
    {
        if (currentLineIndex < dataLines.Length)
        {
            string[] data = dataLines[currentLineIndex].Split(',');

            if (data.Length == 2 && float.TryParse(data[0], out float time) && float.TryParse(data[1], out float emg))
            {
                // Add new data point to the lists
                timeData.Add(time);
                emgData.Add(emg);

                // Maintain the maximum number of data points
                if (timeData.Count > maxDataPoints)
                {
                    timeData.RemoveAt(0);
                    emgData.RemoveAt(0);
                }


                // Update the chart using SetData
               lineChart.GetSerie(0).ClearData(); // Clear current chart data
                for (int i = 0; i < timeData.Count; i++)
                {
                    lineChart.AddData(0, timeData[i], emgData[i]);
                }
            }
         

            currentLineIndex++;
            Debug.Log("Data count: " + lineChart.GetSerie(0).dataCount);

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
