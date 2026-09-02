using UnityEngine;
using System.Linq; 

public class MusicLookup : MonoBehaviour
{
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

    readVideo videoData;

    void slice(float[] handledData, float threshold)
    {
        // This function will handle slicing the video data and analyzing it for music matching
        // uses an algorithim to determine peaks based off of 
        // avaerage brightness and color values, and then uses these peaks to determine the best matching music for the video.
        // called 5 times for r,g,b,brightness and difference, and then the results are combined to create a final result.
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
        }
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoData = new ReadVideo();
        slice();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
