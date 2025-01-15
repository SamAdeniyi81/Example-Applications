using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XCharts.Runtime;

public class CSVReader : MonoBehaviour
{
    public LineChart lineChart; // Reference to your XCharts LineChart
    public string filePath = "Assets/Scripts/Test_EMG_data.csv"; // CSV file
    public float updateInterval = 0.001f; // Time in seconds between updates (1 ms per sample)
    public int xAxisWindowSize = 500; // Number of data points visible on the graph at any time
    public float minXAxisInterval = 1f; // Minimum spacing between X-axis ticks

    List<float> xData = new List<float>();
    List<float> yData = new List<float>();
    private int currentIndex = 0; // Tracks which row of the CSV we're at
    private float elapsedTime = 0f; // Tracks time since last update

    void Start()
    {
        LoadCSVData();
        ConfigureChartAppearance();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Check if it's time to add the next data point
        if (elapsedTime >= updateInterval && currentIndex < xData.Count)
        {
            elapsedTime = 0f;

            // Downsample: Add every nth data point
            int downsampleRate = 5; // Adjust based on performance
            if (currentIndex % downsampleRate == 0)
            {
                var serie = lineChart.GetSerie(0);
                serie.AddXYData(xData[currentIndex], yData[currentIndex]);

                // Remove old data points to limit visible data
                if (serie.dataCount > xAxisWindowSize)
                {
                    serie.RemoveData(0);
                }
            }

            // Scroll the graph when exceeding the window size
            if (currentIndex >= xAxisWindowSize)
            {
                AdjustXAxis(currentIndex);
            }

            currentIndex++;
        }
    }

    void LoadCSVData()
    {
        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            bool isHeader = true;

            while ((line = sr.ReadLine()) != null)
            {
                if (isHeader) // Skip the header row
                {
                    isHeader = false;
                    continue;
                }

                var values = line.Split(',');
                xData.Add(float.Parse(values[0])); // Time
                yData.Add(float.Parse(values[1])); // Value
            }
        }
    }

    void AdjustXAxis(int currentIndex)
    {
        var xAxis = lineChart.GetChartComponent<XAxis>();

        // Dynamically adjust the X-axis range for scrolling
        float newMin = Mathf.Max(0, xData[currentIndex] - xAxisWindowSize);
        xAxis.min = newMin;
        xAxis.max = xData[currentIndex];
    }

    void ConfigureChartAppearance()
    {
        var serie = lineChart.GetSerie(0);
        if (serie == null)
        {
            Debug.LogError("Line chart has no series configured!");
            return;
        }

        // Smooth line appearance
        serie.lineStyle.type = LineStyle.Type.Solid;
        serie.lineStyle.width = 2f;

        // Disable tooltip to avoid NullReferenceException
        var tooltip = lineChart.GetChartComponent<Tooltip>();
        if (tooltip != null)
        {
            tooltip.show = false;
        }

        // Y-axis configuration
        var yAxis = lineChart.GetChartComponent<YAxis>();
        yAxis.min = 0;
        yAxis.max = 1f;

        // X-axis configuration
        var xAxis = lineChart.GetChartComponent<XAxis>();
        xAxis.min = 0;
        xAxis.max = xAxisWindowSize;
        xAxis.splitNumber = 100;
        xAxis.boundaryGap = false;
    }
}
