using UnityEngine;

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

    public void UpdateLookupTable(float r, float g, float b, float br, float mv)
    {
        red = r;
        green = g;
        blue = b;
        brightness = br;
        movement = mv;
    }



    float[] chordWeights = new float[12];

    enum ChordType
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

    public float[] chordPositions = new float[7];

    enum Position
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


        //if red increase majors and add 9 and stronger I IV V
        chordWeights[(int)ChordType.Major] += red * 0.5f;
        chordWeights[(int)ChordType.Add9] += red * 0.3f;
        chordWeights[(int)ChordType.Dominant7th] += red * 0.2f;
        chordWeights[(int)ChordType.Major7th] += red * 0.1f;

        // green means more ambiguity
        chordWeights[(int)ChordType.Suspended2nd] += green * 0.5f;
        chordWeights[(int)ChordType.Suspended4th] += green * 0.3f;
        chordWeights[(int)ChordType.Add9] += green * 0.2f;
        chordWeights[(int)ChordType.Minor7th] += green * 0.1f;
        chordWeights[(int)ChordType.HalfDiminished7th] += green * 0.1f;
        chordWeights[(int)ChordType.Diminished7th] += green * 0.1f;
        chordWeights[(int)ChordType.Diminished] += green * 0.1f;
        chordWeights[(int)ChordType.Augmented] += green * 0.1f;
        chordWeights[(int)ChordType.Major7th] += green * 0.1f;

        // blue means more minor and diminished
        chordWeights[(int)ChordType.Minor] += blue * 0.5f;
        chordWeights[(int)ChordType.Diminished] += blue * 0.3f;
        chordWeights[(int)ChordType.HalfDiminished7th] += blue * 0.2f;
        chordWeights[(int)ChordType.Diminished7th] += blue * 0.1f;
        chordWeights[(int)ChordType.Minor7th] += blue * 0.1f;


        // brightness means more major and dominant
        chordWeights[(int)ChordType.Major] += brightness * 0.5f;
        chordWeights[(int)ChordType.Dominant7th] += brightness * 0.3f;
        chordWeights[(int)ChordType.Major7th] += brightness * 0.2f;
        chordWeights[(int)ChordType.Add9] += brightness * 0.1f; 

        // movement means more suspended and augmented and minor
        chordWeights[(int)ChordType.Minor] += movement * 0.5f;
        chordWeights[(int)ChordType.Suspended2nd] += movement * 0.5f;
        chordWeights[(int)ChordType.Suspended4th] += movement * 0.3f;
        chordWeights[(int)ChordType.Augmented] += movement  * 0.2f; 
        

        // Red → stronger I, IV and V
        positionWeights[(int)Position.I]  += red * 0.5f;
        positionWeights[(int)Position.IV] += red * 0.4f;
        positionWeights[(int)Position.V]  += red * 0.5f;

        // Green
        positionWeights[(int)Position.II]  += green * 0.2f;
        positionWeights[(int)Position.III] += green * 0.3f;
        positionWeights[(int)Position.IV]  += green * 0.3f;
        positionWeights[(int)Position.VI]  += green * 0.3f;

        // Blue
        positionWeights[(int)Position.II]  += blue * 0.4f;
        positionWeights[(int)Position.III] += blue * 0.4f;
        positionWeights[(int)Position.VI]  += blue * 0.5f;
        positionWeights[(int)Position.VII] += blue * 0.5f;


        if (previousChord != -1 && previousShape != -1)
        {       
            chordWeights[previousChord] +=  0.5f;

            positionWeights[previousShape] += 0.5f;
        }
        
        
       
    }
}
