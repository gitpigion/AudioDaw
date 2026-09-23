using UnityEngine;
using System;
using System.Linq; 
using System.Collections.Generic;
using System.IO;

public class MusicLookup : MonoBehaviour
{
    float[] averageBrightness;
    float[] averageDifference;
    float[][] averageRGB;

    int bpm;
    int dataLength;
    float sliceLength = 0.1f; // in seconds

    int[] finalChordPositions;
    int[] finalChordTypes;

    PeakData finalPeaks;

    public DrumRoll drumRoll;

    LookupTable lookupTable;

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


        // remove all 0 peaks
        peakIndexes = peakIndexes.Where(x => x != 0).ToArray();
        maxDistances = maxDistances.Where(x => x != 0).ToArray();

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
    public void Process()
    {
        PeakData[] peaks = new PeakData[5];
        lookupTable = new LookupTable();
        videoData = new ReadVideo();


        averageBrightness = videoData.avgBrightness.ToArray();
        averageDifference = videoData.avgDifference.ToArray();
        dataLength = averageDifference.Length;
        Debug.Log("size of data given is " + dataLength);
        averageRGB = new float[videoData.avgRGB.Count][];

        


        for (int i = 0; i < videoData.avgRGB.Count; i++)
        {
            averageRGB[i] = new float[3];
            for (int j = 0; j < 3; j++)
            {
                averageRGB[i][j] = videoData.avgRGB[i][j];
            }
        }


        // has set up all arrays

        peaks[0] = slice(averageBrightness, 10);
        peaks[1] = slice(averageDifference, 10);
        for (int i = 0; i < 3; i++)
        {
            peaks[2 + i] = slice(averageRGB.Select(x => x[i]).ToArray(), 10);
        }
        // has found max peaks for all data

        // may cause an error with different size peaks
        // creates final peaks by adding all previous peaks to an array and slicing that
        float[] handledData = new float[dataLength];
        for (int i = 0; i < peaks.Length; i++)
        {
            for (int j = 0; j < peaks[i].indexes.Length; j++)
            {
                handledData[peaks[i].indexes[j]] += peaks[i].values[j];
                
            }
        }
        // ordeded based of intensity of movement
        finalPeaks = slice(handledData, 10);

        for (int i = 0; i < finalPeaks.indexes.Length; i++ )
        {
            Debug.Log("peak at "+ finalPeaks.indexes[i]+ " with value " + finalPeaks.values[i]);
        }

        createBPM();
        Debug.Log("BPM is " + bpm);
        BuildChord();
 

        addToDrumRoll();

        Debug.Log("finished Process");

       

    }


        // then use bpm to add the chord notes to the set cells on drum roll
        // using the video length it creates the correct amount of segments
        // then checks where each beat would be at what slice and makes it the chord their
        // then ta da
    void addToDrumRoll()
    {
         float cellLength =  60f / bpm; // in seconds
         Debug.Log($"cell length = {cellLength}");
        int cellsInSegment = 20; // arbitrary number of cells in a segment
        float videoLength = dataLength *sliceLength; // the length of the video in seconds
        Debug.Log($"videoLength = {videoLength}");
        int cellsInVideo = Mathf.CeilToInt(videoLength / cellLength); // the number of cells in the video
         Debug.Log($"cellsInVideo = {cellsInVideo}");
        int segmentsInVideo = Mathf.CeilToInt((float)cellsInVideo / cellsInSegment);
        Debug.Log($"segmentsInVideo = {segmentsInVideo}");

       
        
        for (int i = 0; i < segmentsInVideo -1; i++)
        {
            drumRoll.CreateSegment();
            // makes segments for the videolength assuming one already exists
        }

         Debug.Log($"indexes list length  = {finalPeaks.indexes.Length}");
        // for cellsInVideo, check what slice it is in, and then add the chord notes to that cell
        for (int i = 0; i < finalPeaks.indexes.Length; i++)
        {
          

            int[] chordNotes = lookupTable.GetChordNotes(0, 
            (LookupTable.Position)(finalChordPositions[i]),
             (LookupTable.ChordType)(finalChordTypes[i])); 
            // assuming finalChordTypes is a 2D array with chord types for each slice
            Debug.Log($"chord length for chord {i} = {chordNotes.Length}");
            for (int j = 0; j < chordNotes.Length; j++)
            {
                //public void SetCell(int row, int col, float amplitude, int Segment = -1, int Instrument = -1)
                drumRoll.SetCell(chordNotes[j], (i % cellsInSegment), 1f, Mathf.FloorToInt(i / cellsInSegment), 0); // assuming instrument 0 is the one you want to use
            }
        }
    }


    int fps = 30;

    void createBPM()
    {
        // this function will take the final peak data and create a BPM value for the video
        // it will use the distance between the peaks to determine the BPM
        // it will also use the average brightness and color values to determine the BPM
        // it will return a BPM value that can be used to match music to the video
        // then it will adjust peak data to match bpm, and form minor groups

        float brightnessBPM = averageBrightness.Average();

        brightnessBPM = brightnessBPM /255f * 120 + 60; // scale to 60-180 bpm

        bpm = Mathf.RoundToInt(brightnessBPM);


        int beatsPerBar = 4; // 4/4
        // int beatsPerBar = 3; // 3/4

        float beatDuration = 60f / bpm;

        float barDuration = beatDuration * beatsPerBar;




        // snaps peaks to bpm
        for (int i = 0; i < finalPeaks.indexes.Length; i++)
        {
            float peakTime = finalPeaks.indexes[i] * (1f / fps); // assuming 30 fps
            
            float beatNumber = Mathf.Round(peakTime / beatDuration);
            int snappedTime = Mathf.RoundToInt( beatNumber * beatDuration * fps); // convert back to frame index
            finalPeaks.indexes[i] = snappedTime;
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
        int chordCount = finalPeaks.indexes.Length;

         finalChordPositions = new int[chordCount];
         finalChordTypes = new int[chordCount];
            
        
            for (int i = 0; i < chordCount; i++)
            {
              
                
                // snapping causes out or array issues
                int index = finalPeaks.indexes[i];
                if (index >= dataLength)
                {
                    index = dataLength - 1;
                }
                // minus 1 as index counts from 1-x


                
                // updates lookup table with the indexes of the peaks being used 
                lookupTable.UpdateLookupTable(
                    averageRGB[index][0],
                    averageRGB[index][1],
                    averageRGB[index][2],
                    averageBrightness[index],
                    averageDifference[index]
                );
                lookupTable.publicChordLookup();
                
                float[] positionWeights = (float[])lookupTable.positionWeights.Clone();
                float[] typeWeights = (float[])lookupTable.chordWeights.Clone();

                finalChordPositions[i] = GetMaxIndex(positionWeights);
                finalChordTypes[i] = GetMaxIndex(typeWeights);

               Debug.Log($"Chosen chord position is {(LookupTable.Position)finalChordPositions[i]} with shape {(LookupTable.ChordType)finalChordTypes[i]} at index {index}");


                 //update lookup
            }

 
        
        // finishes with arrays for each major group full of chord gradients
       


    }

    int GetMaxIndex(float[] weights)
    {
        return Array.IndexOf(weights, weights.Max());
    }

    void addRandomness(float[][] data, int maxRecur, float randomness)
    {
        // this function will add randomness to the chord gradients
        // it will add a random amount to each value multiplied by maxrecur

        for (int i = 0; i < data.Length; i++)
        {
            for (int j = 0; j < data[i].Length; j++)
            {
                data[i][j] += UnityEngine.Random.Range(-randomness, randomness ) * maxRecur;
            }
        }

    }
    
 
    int recurCount = 0;
    int maxRecurCount = 3;


    /*
    void RecurChord(float[][] chordPositionsWeights, float[][] chordTypesWeights, int[] likelyChordIndex, int[] likelyShapeIndex)
    {
        // this function will be recurred
        // it will loop through each major group and find the chord gradients (chords at each slice
        // influenced by nearbyslices) then move to next major group with a high weight on previous key

        // after the function has finished, it will reccur again
        // taking the chord gradient and minor groups it will attempt to create a progression in each major group
        // this should mean, very short major groups will have a sinle chord, and longer groups will have progressions


        int num = 0;

             for (int j = 0; j < finalPeaks.indexes.Length; j++)
            {
                // if in a group it will use previous data to inform new chord
                // could be usefull to eventualy add a weight between groups
                
                if ( j != finalpeaks.indexes[num] && j !=0)
                {
                    lookupTable.publicChordLookup(likelyChordIndex[j-1], likelyShapeIndex[j-1]);
                    
                }
                else
                {
                    lookupTable.publicChordLookup();
                    num++;
                }

               

                chordPositionsWeights[j] += lookupTable.chordPositions;
                chordTypesWeights[j] += lookupTable.chordWeights;

                int likelyChordIndex = Array.IndexOf(lookupTable.chordWeights, lookupTable.chordWeights.Max());
                likelyChordIndex[j] = likelyChordIndex;
                 int likelyShapeIndex = Array.IndexOf(lookupTable.chordPositions, lookupTable.chordPositions.Max());
                likelyShapeIndex[j] = likelyShapeIndex;
                // finds most likely chord to inform next slice
            }

            recurCount++;
            if (recurCount < maxRecurCount)
            {
                RecurChord(chordPositionsWeights, chordTypesWeights, likelyChordIndex.ToArray(), likelyShapeIndex.ToArray());
            }
            else
            {
                finalChordPositions = chordPositionsWeights;
                finalChordTypes = chordTypesWeights;
            }
            

    }
    */


}
