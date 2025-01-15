using UnityEngine;
using XCharts;
using System.IO;
using System.Collections.Generic;
using XCharts.Runtime;

public class RealTimeEMGController : MonoBehaviour
{
    // Reference to the line chart from XCharts
    public LineChart lineChart;

    // Lists to hold time and EMG data points
    private List<float> timeData = new List<float>();
    private List<float> emgData = new List<float>();

    // Maximum number of data points to display at once
    public int maxDataPoints = 100;

    void Start()
    {
        if (lineChart == null)
        {
            Debug.LogError("LineChart reference is missing.");
            return;
        }

        // Initialize the chart with an empty series
        lineChart.ClearData();
        lineChart.AddSerie<Line>("line");
    }
     /// <summary>
    /// Add a new data point for real-time visualization.
    /// </summary>
    /// <param name="time">The timestamp of the data point.</param>
    /// <param name="emgValue">The EMG value to plot.</param>
    public void AddRealTimeDataPoint(float time, float emgValue)
    {
        // Add new data point
        timeData.Add(time);
        emgData.Add(emgValue);

        // Remove the oldest data points if we exceed the maximum allowed
        if (timeData.Count > maxDataPoints)
        {
            timeData.RemoveAt(0);
            emgData.RemoveAt(0);
        }

        // Update the chart with the latest data
        lineChart.GetSerie(0).ClearData();
        for (int i = 0; i < timeData.Count; i++)
        {
            lineChart.AddData(0, timeData[i], emgData[i]);
        }
    }
}
