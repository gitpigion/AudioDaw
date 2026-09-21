using UnityEngine;
using System.Linq; 

public class MusicLookup : MonoBehaviour
{
    float[] averageBrightness;
    float[] averageDifference;
    float[][] averageRGB;

    int bpm;

    lookupTable lookupTable;

    // matches music information to the video data
    // Create BPM and groups
    //Major groups: change in video
    //Minor groups: BPM division between these in time sign
    //Signature average
    //Progression groups: generally 3 or 4 minor groups that try
    //To match each other to the other progression groups of the same length either side within the same Major Group

    //Run through each slice, taking the color values, brightness,
    //And nearby slice values to assert chord position; do not
    //Include Slices outside of group

    //After first recursion, set a key and recur again, encourage different chords in each minor group
    //Encourage root chord in first or last chord, if not in every progression, then in every second.

    // Within major groups, encourage repetitive chord progressions in progression groups

 
    //Each slice will have an array of each formation of chord, i.e. V,7th at 40% 
    //These arrays will be influenced by all above factors, decided on and then recurred again in the hope to create a cohesive chord base.

    //Major groups will be able to add modifiers like, making one side major and other minor in example.

    ReadVideo videoData;

    PeakData slice(float[] handledData, int threshold)
    {
        // This function will handle slicing the video data and analyzing it for music matching
        // uses an algorithim to determine peaks based off of 
        // avaerage brightness and color values, and then uses these peaks to determine the best
        //  matching music for the video.
        // called 5 times for r,g,b,brightness and difference, and then the results are combined to 
        // create a final result.
        // uses an arbitrary number to control how many slices should be in a value of time
        // combines the 5 values and threshold to form groups

        float Avg = handledData.Average();

        // checks difference between curvalue and either side, sets it to curDifference
        // then adds curDifference of previous and next value to group distance
        // then takes the highest group distance, ignores the values on either side
        // then repeats this process until threshold is met
        // then returns the groups to be used in the next step of the process

        int ignoreAmount = 3;


        float[] curDifference = new float[handledData.Length];
        float[] groupDistance = new float[handledData.Length];

        float[] maxDistances = new float[threshold];
        

        for (int i = 0; i < handledData.Length; i++)
        {
            if (i == 0 || i == handledData.Length - 1)
            {
                curDifference[i] = 0;
            }
            else
            {
                // adds the differences
                curDifference[i] = Mathf.Abs(handledData[i] - handledData[i - 1]) + Mathf.Abs(handledData[i] - handledData[i + 1]);
            }
        }

        for (int i = 0; i < handledData.Length; i++)
        {
            if (i == 0 || i == handledData.Length - 1)
            {
                groupDistance[i] = 0;
            }
            else
            {
                // adds the differences
                groupDistance[i] = curDifference[i - 1] + curDifference[i + 1];
            }
        }

        int[] peakIndexes = new int[threshold];

        for (int i = 0; i < threshold; i++)
        {
            float maxGroupDistance = groupDistance.Max();
            int maxIndex = System.Array.IndexOf(groupDistance, maxGroupDistance);
            peakIndexes[i] = maxIndex;

            // ignore the values on either side of the maxIndex
            for (int j = 1; j <= ignoreAmount; j++)
            {
                if (maxIndex - j >= 0)
                {
                    groupDistance[maxIndex - j] = 0;
                }
                if (maxIndex + j < groupDistance.Length)
                {
                    groupDistance[maxIndex + j] = 0;
                }
            }

            // set the maxIndex to 0 so it won't be considered in the next iteration
            groupDistance[maxIndex] = 0;
            maxDistances[i] = maxGroupDistance;
        }
        

        return new PeakData(peakIndexes, maxDistances);
    }


    public struct PeakData
    {
        public int[] indexes;
        public float[] values;

        public PeakData(int[] index, float[] value)
        {
            this.indexes = index;
            this.values = value;
        }
    }

    // takes all the peaks and returns final peak data
    // this will be an avaerage of all the peaks with weights attached to each peak based on 
    // the value of the peak, and the distance between the peaks`
    // weights are R,G,B,brightness,difference

    // very similar to slice, but with much smaller data set
    // multiplies the values of each peak by the weight of that peak
   

    // slices each of the 5 values and returns the peak data
    void Start()
    {
        PeakData[] peaks = new PeakData[5];

        lookupTable = new lookupTable();

        videoData = new ReadVideo();
        averageBrightness = videoData.avgBrightness.ToArray();
        averageDifference = videoData.avgDifference.ToArray();
        averageRGB = new float[videoData.avgRGB.Count][];
        for (int i = 0; i < videoData.avgRGB.Count; i++)
        {
            averageRGB[i] = new float[3];
            for (int j = 0; j < 3; j++)
            {
                averageRGB[i][j] = videoData.avgRGB[i][j];
            }
        }

        peaks[0] = slice(averageBrightness, 10);
        peaks[1] = slice(averageDifference, 10);
        for (int i = 0; i < 3; i++)
        {
            peaks[2 + i] = slice(averageRGB.Select(x => x[i]).ToArray(), 10);
        }

        float[] weights = new float[5] { 1f, 1f, 1f, 1f, 1f };
        int dataLength = averageBrightness.Length;
         float[] handledData = new float[dataLength];
        for (int i = 0; i < peaks.Length; i++)
        {
            for (int j = 0; j < peaks[i].indexes.Length; j++)
            {
                handledData[peaks[i].indexes[j]] = peaks[i].values[j] * weights[i];
            }
        }
        PeakData finalPeaks = slice(handledData, 10);
    }


    void createBPM()
    {
        // this function will take the final peak data and create a BPM value for the video
        // it will use the distance between the peaks to determine the BPM
        // it will also use the average brightness and color values to determine the BPM
        // it will return a BPM value that can be used to match music to the video
        // then it will adjust peak data to match bpm, and form minor groups

        float brightnessBPM = avgBrightness.Average();

        brightnessBPM = brightnessBPM /255f * 120 + 60; // scale to 60-180 bpm

        bpm = Mathf.RoundToInt(brightnessBPM);


        int beatsPerBar = 4; // 4/4
        // int beatsPerBar = 3; // 3/4

        float barDuration = beatDuration * beatsPerBar;


        float beatDuration = 60f / bpm;

        // snaps peaks to bpm
        for (int i = 0; i < finalPeaks.indexes.Length; i++)
        {
            float peakTime = finalPeaks.indexes[i] * (1f / 30f); // assuming 30 fps
            
            float beatNumber = Mathf.Round(peakTime / beatDuration);
            float snappedTime = beatNumber * beatDuration;
            finalPeaks.values[i] = snappedTime;
        }

    }



   
    void BuildChord()
    {
        // this function will be recurred
        // it will loop through each major group and find the chord gradients (chords at each slice
        // influenced by nearbyslices) then move to next major group with a high weight on previous key

        // after the function has finished, it will reccur again
        // taking the chord gradient and minor groups it will attempt to create a progression in each major group
        // this should mean, very short major groups will have a sinle chord, and longer groups will have progressions


        List<string[]>[] chordPositions = new List<string[]>[finalPeaks.indexes.Length];
        List<string[]>[] chordTypes = new List<string[]>[finalPeaks.indexes.Length];

        for (int i = 0; i < finalPeaks.indexes.Length; i++)
        {
            // find the chord gradients for each major group
            // then move to next major group with a high weight on previous key

            List<string[]> chordForMajorGroup = new List<string[]>();
            List<string[]> positionsForMajorGroup = new List<string[]>();

            int index = 0;

            for (int j = index; j < finalPeaks.indexes.Length; j++)
            {
                lookupTable.publicChordLookup();
                index = j;
                chordForMajorGroup.Add(lookupTable.chordWeights);
                positionsForMajorGroup.Add(lookupTable.chordPositions);
                if (j < finalPeaks.indexes[i])
                {
                    break;
                }
                // breaks when over with group
            }

            chordPositions[i] = chordForMajorGroup.ToArray();
            chordTypes[i] = positionsForMajorGroup.ToArray();
        }
        // finishes with arrays for each major group full of chord gradients



    }

    void RecurChord(string[][][] chordPositions, string[][][] chordTypes)
    {
        // this function will be recurred
        // it will loop through each major group and find the chord gradients (chords at each slice
        // influenced by nearbyslices) then move to next major group with a high weight on previous key

        // after the function has finished, it will reccur again
        // taking the chord gradient and minor groups it will attempt to create a progression in each major group
        // this should mean, very short major groups will have a sinle chord, and longer groups will have progressions


        for (int i = 0; i < chordPositions.Length; i++)
        {
            // find the chord gradients for each major group
            // then move to next major group with a high weight on previous key

            

            int index = 0;

             for (int j = index; j < finalPeaks.indexes.Length; j++)
            {
                lookupTable.publicChordLookup();
                index = j;
                chordPositions[i][j-index] = lookupTable.chordPositions;
                chordTypes[i][j-index] = lookupTable.chordWeights;
                if (j < finalPeaks.indexes[i])
                {
                    break;
                }
                // breaks when over with group
            }

            chordPositions[i] = chordForMajorGroup.ToArray();
            chordTypes[i] = positionsForMajorGroup.ToArray();
        }

    }



}
