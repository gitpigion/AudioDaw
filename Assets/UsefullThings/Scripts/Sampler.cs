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

    private readonly object lockObj = new object();

    int bufferSize;
    int sampleRate;

    Sample[] incomingSamples;

    List<Sample> playingSamples;




    //is given all the samples to trigger
    // sets incoming samples to an aray of new samples
    // for each incoming sample
    public override void Play(int[] sampleHits)
    {
        

        lock(lockObj)
        {
            List<Sample> incomingSamplesTemp = new List<Sample>();
        for (int i = 0; i< sampleHits.Length; i++)
        {
            if (sampleHits[i] < buffers.Length)
                {
                     Debug.Log("attempting to play sample at position " + sampleHits[i]);
            Sample buffy = new Sample();
            buffy.buffer = buffers[sampleHits[i]];
            Debug.Log(incomingSamples);
            Debug.Log(buffy);
            incomingSamplesTemp.Add(buffy);
                }
                else break;
           
        }

        incomingSamples = incomingSamplesTemp.ToArray();
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


    public void AddSample(AudioClip clip)
    {
        float[][] newBuffers = new float[buffers.Length+1][];
        for (int i = 0; i < buffers.Length; i++)
        {
            newBuffers[i] = buffers[i];
        }
        Debug.Log("current amount of samples " + newBuffers.Length);

        newBuffers[buffers.Length] = setToBuffer(clip);

        buffers = newBuffers;
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
            Debug.Log("created sample with size " + buffer.Length);
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
            incomingSamples = null;
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
        Debug.Log("there are currently " + buffers.Length + " buffers");
        if (buffers.Length == 0) return buffer;
        if (playingSamples.Count == 0) return buffer;

        Debug.Log("attempting to play sample" + playingSamples.Count);

        
        for (int curSample = 0; curSample < playingSamples.Count; curSample++)
        {
            for (int i = 0; i < framesRequested; i++)
            {
                buffer[i] += playingSamples[curSample].GetBuffer(i); // playingSamples.Count;
            }
            playingSamples[curSample].Increment(framesRequested);
            if (playingSamples[curSample].DestructCheck)
            {
                Debug.Log("getting rid of sample at position " + curSample);
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
        if ( i+position < buffer.Length)
        {
            return buffer[i+ position];
        }
        else
        {
            DestructCheck = true;
            return 0;
            
        }
        
    }

    public bool DestructCheck = false;
  


    

}