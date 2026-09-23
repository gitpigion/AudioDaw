using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadVideo
{
    string CSVPath = "/Users/benjiegan/Documents/benji/Python dataset/video_data.csv";

    public List<int[]> avgRGB = new List<int[]>();
    public List<float> avgBrightness = new List<float>();
    public List<float> avgDifference = new List<float>();

    public ReadVideo()
    {
        ReadCSV();
    }

    void ReadCSV()
    {

        
        using var reader = new StreamReader(CSVPath);

        string headerLine = reader.ReadLine();
        string[] headers = headerLine.Split(',');

        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] fields = line.Split(',');

            for (int i = 0; i < fields.Length; i++)
            {
                if (headers[i] == "Average_Color")
                {
                    int[] result = fields[i]
                        .Trim('[', ']')
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray();

                    avgRGB.Add(result);
                }
                else if (headers[i] == "Average_Brightness")
                {
                    float result = float.Parse(
                        fields[i],
                        System.Globalization.CultureInfo.InvariantCulture
                    );

                    avgBrightness.Add(result);
                }
                else if (headers[i] == "Average_Frame_Difference")
                {
                    float result = float.Parse(
                        fields[i],
                        System.Globalization.CultureInfo.InvariantCulture
                    );

                    avgDifference.Add(result);
                }
            }
        }

        Debug.Log("Read CSV with item length " + avgDifference.Count);
    }
}