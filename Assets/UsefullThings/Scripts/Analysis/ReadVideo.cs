using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

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
                if (headers[i] == "avgRGB")
                {
                    int[] result = fields[i]
                        .Trim('[', ']')
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToArray();

                    avgRGB.Add(result);
                }
                else if (headers[i] == "avgBrightness")
                {
                    float result = float.Parse(fields[i]);
                    avgBrightness.Add(result);
                }
                else if (headers[i] == "avgDifference")
                {
                    float result = float.Parse(fields[i]);
                    avgDifference.Add(result);
                }
            }
        }
    }
}