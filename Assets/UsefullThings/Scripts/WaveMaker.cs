using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class WaveMaker : AudioPull
{

    /*
        takes target frequncy, amplitude, and envelope position and creates a buffer

    */



    public float[] frequencys;

    float[] freqeuncyBuffer = new float[440];

    private float[] phases = new float[0];

    float sampleRate;

    public WaveMaker()
    {
        sampleRate = AudioSettings.outputSampleRate;
        Debug.Log("Sample rate: " + sampleRate);
    }

    public void SetFrequency(float[] F)
    {
        freqeuncyBuffer = F;
    }

    public override float[] Pull(int framesRequested)
    {
        frequencys = freqeuncyBuffer;
        // this is where it grabs the information from the midi info and forms it into waves
        // these waves are then passed along

        // will choose what type of wave

        // test freqeuncy
        float[] buffer = new float[framesRequested];
        int chordSize = frequencys.Length;
        if (phases == null || phases.Length != chordSize)
        {
            phases = new float[chordSize];
        }



        for (int j = 0; j < framesRequested; j++)
        {

            for (int note = 0; note < chordSize; note++)
            {
                float phaseIncrement = frequencys[note] / sampleRate;
                buffer[j] += Mathf.Sin(phases[note] * 2f * Mathf.PI)/chordSize;
            
                phases[note] += phaseIncrement;
                if (phases[note] >= 1f) phases[note] -= 1f;
            }
        }

        

        return buffer;
    }
}
