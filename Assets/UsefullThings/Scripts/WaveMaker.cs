using UnityEngine;

public class WaveMaker : AudioPull
{

    /*
        takes target frequncy, amplitude, and envelope position and creates a buffer

    */



    public float frequency = 440f;

    private float phase = 0f;

    public override float[] Pull(int framesRequested)
    {
        // this is where it grabs the information from the midi info and forms it into waves
        // these waves are then passed along

        // will choose what type of wave

        // test freqeuncy
        float[] buffer = new float[framesRequested];
        float sampleRate = AudioSettings.outputSampleRate;
        float phaseIncrement = frequency / sampleRate;

        for (int i = 0; i < framesRequested; i++)
        {
            buffer[i] = Mathf.Sin(phase * 2f * Mathf.PI);

            phase += phaseIncrement;
            if (phase >= 1f) phase -= 1f;
        }

        return buffer;
    }
}
