using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sampler : AudioPull
{
    // given numbers instead of frequncies
    // plays samples given in it via setting them into noisy buffers
    // mono because thats cooler

    public AudioClip[] Clips = new AudioClip[0];
    private float[][] buffers;

    int bufferSize;
    int sampleRate;

    Sample[] incomingSamples;

    List<Sample> playingSamples;





    public override void Play(int[] sampleHits)
    {
        incomingSamples = new Sample[sampleHits.Length];
        for (int i = 0; i< sampleHits.Length; i++)
        {
            Sample buffy = new Sample();
            buffy.buffer = buffers[i];
            incomingSamples[i] = buffy;
        }
    }


    // turns alllll the wav files given into usable buffers
    public override void Init()
    {
        buffers = new float[Clips.Length][];
        for (int i = 0; i < Clips.Length; i++ ) buffers[i] = setToBuffer(Clips[i]);
        playingSamples = new List<Sample>();
        sampleRate = AudioSettings.outputSampleRate;
        AudioSettings.GetDSPBufferSize(out bufferSize, out _);
    }

    float[] setToBuffer(AudioClip clip)
    {
        float[] buffer = new float[clip.samples * clip.channels];

        clip.GetData(buffer, 0);
        //make mono
        if (clip.channels >1)
        {
            float[] monoBuffer= new float[buffer.Length/clip.channels];
            for (int i = 0; i < monoBuffer.Length; i++)
            {
                monoBuffer[i] = buffer[i * clip.channels];
                // itll be a wonder if this actualy works
            }
            return monoBuffer;
        }

        return buffer;
    }

    // new samples? great lets add them
    void CheckForNewSamples()
    {
        if (incomingSamples != null)
        {
            foreach (Sample sample in incomingSamples)
        {
            playingSamples.Add(sample);
        }
        }
        

    }
    
    // creates array of how many buffers are being played (maybe a list)
    // initially checks to see if any new samples needed to be added to list
    // each sample can be stored in a class called sample!!!!
    // in this sample class they have the pleasure of containg the full sample, the increment, and amplitude
    // because it is in a class it can be moved around in the list seamlessly
     public override float[] Pull(int framesRequested)
    {
        float[] buffer = new float[framesRequested];
        CheckForNewSamples();
        if (Clips.Length == 1) return buffer;

        

        
        for (int curSample = 0; curSample < playingSamples.Count; curSample++)
        {
            for (int i = 0; i < framesRequested; i++)
            {
                buffer[i] += playingSamples[curSample].GetBuffer(i) / playingSamples.Count;
            }
            playingSamples[curSample].Increment(framesRequested);
            if (playingSamples[curSample].DestructCheck())
            {
                playingSamples.Remove(playingSamples[curSample]);
            }

        }
        

        return buffer;
    }
}

public class Sample
{
    public float [] buffer;

    public int position = 0;

    public void Increment (int bufferSize)
    {
        if (position < buffer.Length)
        {
            position += bufferSize;
        }

    }

    public float GetBuffer(int i)
    {
        return buffer[i+ position];
    }

    public bool DestructCheck()
    {
        return position == buffer.Length;
    }


    

}