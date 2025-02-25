using UnityEngine;
using XCharts;
using System.IO;
using System.Collections.Generic;
using XCharts.Runtime;

public class EMGGraphControllerTest : MonoBehaviour
{
    public LineChart lineChart; // reference to line chart
    public TextAsset csvFile; // Assign your CSV file here in the Inspector.
    public float updateInterval = 0.1f; // Time between updates in seconds.
    private const int maxDataPoints = 50; // Maximum number of points to display


    private List<float> timeData = new List<float>();//stores time from csv
    private List<float> emgData = new List<float>(); //stores emg data, normalized freom 0-1
    private int currentLineIndex = 0; // track current poisiton when reading csv
    private string[] dataLines; //holds csv lines after being split

    private float timer = 0f; // timer

    /// <summary>
    /// new stuff
    public Material muscleGlowMaterial; //shader material

    /// </summary>

    void Start()
    {
        LoadCSV(); // read data from csv
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

    void LoadCSV()
    {
        dataLines = csvFile.text.Split('\n'); // solits scv into lines
        currentLineIndex = 1; // Skip the header row
    }

    void UpdateChartRealTime()
    {
        if (currentLineIndex < dataLines.Length)
        {
            string[] data = dataLines[currentLineIndex].Split(','); // split data into time and emg 

            if (data.Length == 2 && float.TryParse(data[0], out float time) && float.TryParse(data[1], out float emg)) // convert to float
            {

                // Normalize EMG value (assuming EMG ranges between 0 and 1, adjust if needed)
                float normalizedEMG = Mathf.InverseLerp(0f, 1f, emg); // normalize emg

                // Add new data point to the lists
                timeData.Add(time);
                emgData.Add(normalizedEMG);

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
                    lineChart.AddData(0, timeData[i], emgData[i]);//repopulate chart 
                }

                // Update the shader with the current EMG value
                muscleGlowMaterial.SetFloat("_EMG_Value", normalizedEMG);

                Debug.Log("Setting EMG Value: " + normalizedEMG);
                muscleGlowMaterial.SetFloat("_EMGValue", normalizedEMG);

            }


            currentLineIndex++;
            Debug.Log("Data count: " + lineChart.GetSerie(0).dataCount);


        }
    }
    public void AddRealTimeDataPoint(float time, float emgValue)//add emg poits manually
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
