using UnityEngine;
using System;
using System.Linq; 
using System.Collections.Generic;
using System.IO;

public class LookupTable : MonoBehaviour
{
    // This class is used to store the lookup table for the music matching algorithm
    // It will take in data and return key, chord, light/dark
    // this will be done for each frame and processed in musiclookup.cs


    // this array is the likelyhood of each chord being played based on the color and brightness values of the video
    // it will be used to determine the chord progression of the music
    // it contains every main chord type asccociated with position in array

    float red;
    float green;
    float blue;
    float brightness;
    float movement;

    float maxMovementFloat;


    public LookupTable(float maxMovement)
    {
        maxMovementFloat = maxMovement;
    }

    public void UpdateLookupTable(float r, float g, float b, float br, float mv)
    {
       
        red = r/255f;
        green = g/255f;
        blue = b/255f;
        brightness = br/255f;
        movement = mv/maxMovementFloat;
    }



    public float[] chordWeights = new float[12];

    public enum ChordType
    {
        Major,
        Minor,
        Diminished,
        Augmented,
        Dominant7th,
        Major7th,
        Minor7th,
        HalfDiminished7th,
        Diminished7th,
        Suspended2nd,
        Suspended4th,
        Add9
    }

    public float[] positionWeights = new float[7];

    public enum Position
{
    I,
    II,
    III,
    IV,
    V,
    VI,
    VII
}


    // this array is the likelyhood based of previous chords
    float[] secondaryChordWeights = new float[12];


    public void publicChordLookup(int previousChord = -1, int previousShape = -1    )
    {
        // this function will take in data
        // depedngin on data it will use ranges for each color and brightness
        // then it will compare how strongly each chord is represented
        // then return chord
         Array.Clear(positionWeights, 0, positionWeights.Length);
        Array.Clear(chordWeights, 0, chordWeights.Length);

        //if red increase majors and add 9 and stronger I IV V
        chordWeights[(int)ChordType.Major]       += red * 0.25f;
        chordWeights[(int)ChordType.Add9]        += red * 0.15f;
        chordWeights[(int)ChordType.Dominant7th] += red * 0.10f;
        chordWeights[(int)ChordType.Major7th]    += red * 0.10f;

        // green means more ambiguity
        chordWeights[(int)ChordType.Suspended2nd]      += green * 0.20f;
        chordWeights[(int)ChordType.Suspended4th]      += green * 0.15f;
        chordWeights[(int)ChordType.Add9]              += green * 0.10f;
        chordWeights[(int)ChordType.Minor7th]          += green * 0.10f;
        chordWeights[(int)ChordType.HalfDiminished7th] += green * 0.05f;
        chordWeights[(int)ChordType.Diminished7th]     += green * 0.05f;
        chordWeights[(int)ChordType.Diminished]        += green * 0.05f;
        chordWeights[(int)ChordType.Augmented]         += green * 0.05f;
        chordWeights[(int)ChordType.Major7th]          += green * 0.05f;

        // blue means more minor and diminished
        chordWeights[(int)ChordType.Minor]             += blue * 0.25f;
        chordWeights[(int)ChordType.Diminished]        += blue * 0.15f;
        chordWeights[(int)ChordType.HalfDiminished7th] += blue * 0.10f;
        chordWeights[(int)ChordType.Diminished7th]     += blue * 0.05f;
        chordWeights[(int)ChordType.Minor7th]          += blue * 0.10f;


        // brightness means more major and dominant
        chordWeights[(int)ChordType.Major]       += brightness * 0.15f;
        chordWeights[(int)ChordType.Dominant7th] += brightness * 0.10f;
        chordWeights[(int)ChordType.Major7th]    += brightness * 0.10f;
        chordWeights[(int)ChordType.Add9]        += brightness * 0.05f;

        // movement means more suspended and augmented and minor
        chordWeights[(int)ChordType.Minor]        += movement * 0.15f;
        chordWeights[(int)ChordType.Suspended2nd] += movement * 0.15f;
        chordWeights[(int)ChordType.Suspended4th] += movement * 0.10f;
        chordWeights[(int)ChordType.Augmented]    += movement * 0.10f; 
        

        // Red → stronger I, IV and V
        positionWeights[(int)Position.I]  += red * 0.20f;
        positionWeights[(int)Position.IV] += red * 0.15f;
        positionWeights[(int)Position.V]  += red * 0.20f;

        // Green
        positionWeights[(int)Position.II]  += green * 0.15f;
        positionWeights[(int)Position.III] += green * 0.15f;
        positionWeights[(int)Position.IV]  += green * 0.10f;
        positionWeights[(int)Position.VI]  += green * 0.15f;

        // Blue
        positionWeights[(int)Position.II]  += blue * 0.15f;
        positionWeights[(int)Position.III] += blue * 0.15f;
        positionWeights[(int)Position.VI]  += blue * 0.20f;
        positionWeights[(int)Position.VII] += blue * 0.20f;


        float[] chordTotals =
        {
            0.40f, // Major
            0.40f, // Minor
            0.20f, // Diminished
            0.15f, // Augmented
            0.20f, // Dominant7th
            0.25f, // Major7th
            0.20f, // Minor7th
            0.15f, // HalfDiminished7th
            0.10f, // Diminished7th
            0.35f, // Suspended2nd
            0.25f, // Suspended4th
            0.30f  // Add9
        };

        float target = 0.40f;

        for (int i = 0; i < chordWeights.Length; i++)
        {
            if (chordTotals[i] > 0)
                chordWeights[i] *= target / chordTotals[i];
        }


        float[] positionTotals =
        {
            0.20f, // I
            0.30f, // II
            0.30f, // III
            0.25f, // IV
            0.20f, // V
            0.35f, // VI
            0.20f  // VII
        };

        target = 0.30f;

        for (int i = 0; i < positionWeights.Length; i++)
        {
            if (positionTotals[i] > 0)
                positionWeights[i] *= target / positionTotals[i];
        }


        for (int i = 0; i < chordWeights.Length; i++)
        {
            Debug.Log($"Chord weight for {(ChordType)i}: {chordWeights[i]}");
        }
        for (int i = 0; i < positionWeights.Length; i++)
        {
            Debug.Log($"position weight for {(Position)i}: {positionWeights[i]}");
        }


        if (previousChord != -1 && previousShape != -1)
        {       
            chordWeights[previousChord] +=  0.5f;

            positionWeights[previousShape] += 0.5f;
        }
        
        
       
    }


    int[] GetChordIntervals(ChordType type)
{
    switch (type)
    {
        case ChordType.Major:
            return new int[] { 0, 4, 7 };

        case ChordType.Minor:
            return new int[] { 0, 3, 7 };

        case ChordType.Diminished:
            return new int[] { 0, 3, 6 };

        case ChordType.Augmented:
            return new int[] { 0, 4, 8 };

        case ChordType.Dominant7th:
            return new int[] { 0, 4, 7, 10 };

        case ChordType.Major7th:
            return new int[] { 0, 4, 7, 11 };

        case ChordType.Minor7th:
            return new int[] { 0, 3, 7, 10 };

        case ChordType.HalfDiminished7th:
            return new int[] { 0, 3, 6, 10 };

        case ChordType.Diminished7th:
            return new int[] { 0, 3, 6, 9 };

        case ChordType.Suspended2nd:
            return new int[] { 0, 2, 7 };

        case ChordType.Suspended4th:
            return new int[] { 0, 5, 7 };

        case ChordType.Add9:
            return new int[] { 0, 4, 7, 14 };

        default:
            return new int[] { 0, 4, 7 };
    }
}



    public int[] GetChordNotes(int key, Position position, ChordType type)
{
    int[] scale = { 0, 2, 4, 5, 7, 9, 11 };

    int root = (key + scale[(int)position]) % 12;

    int[] intervals = GetChordIntervals(type);

    int[] notes = new int[intervals.Length];

    for (int i = 0; i < intervals.Length; i++)
    {
        notes[i] = (root + intervals[i]) % 12;
    }

    return notes;
}
}
