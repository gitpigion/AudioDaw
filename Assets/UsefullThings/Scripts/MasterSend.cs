using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public class MasterSend : MonoBehaviour
{
    //Trigger every call for audio, asks for a buffer with a preset amount of samples.
    //  Calls for a buffer a preset amount per second.
    


    private double sampleRate = 0.0F;
    private int bufferLength;

    private bool running = false;

    public List<AudioPull> Pulls = new List<AudioPull>();

    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        AudioSettings.GetDSPBufferSize(out int bufferLength, out int numBuffers);
    }


    void OnAudioFilterRead(float[] data, int channels)
    {
        if (channels == 0)
        {
            //  combines the pull signals with the data to be sent over
            // this is bassicaly useless and only works in mono
            // but i dont want to interleave yet
            foreach (AudioPull pull in Pulls)
            {
                float[] dataToAdd = pull.Pull(bufferLength);
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] += dataToAdd[i];
                }
            }
        }

        //call from busses
        //process into leaved signal
    }
    
 
}
/*
class oldwork
{
           public bool isInitialized = false;
    
    private AudioSource audioSource;
    private AudioClip audioClip;

    void Start()
    {
        isInitialized = true;
       // audioSource = GetComponent<AudioSource>();
    }

    public void SetSamples(float[] samples)
    {
        GenerateAudio(new float[][] { samples });
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void StopPlaying()
    {
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut(float duration = 0.05f)
{
    AudioSource source = audioSource;
    float startVolume = source.volume;

    while (source.volume > 0)
    {
        source.volume -= startVolume * Time.deltaTime / duration;
        yield return null;
    }

    source.Stop();
    source.volume = startVolume;
}

    void GenerateAudio(float[][] samples)
    {
        int sampleCount = samples[0].Length; // use actual data length
        float[] compressedSamples = new float[sampleCount];

        for (int i = 0; i < samples.Length; i++)
            for (int j = 0; j < samples[i].Length; j++)
                compressedSamples[j] += samples[i][j];

        if (samples.Length > 1)
            for (int j = 0; j < sampleCount; j++)
                compressedSamples[j] /= samples.Length;

        // Use AudioSettings.outputSampleRate to match Unity's actual rate
        int outputRate = AudioSettings.outputSampleRate;

        audioClip = AudioClip.Create("GeneratedWave", sampleCount, 1, outputRate, false);
        
        bool success = audioClip.SetData(compressedSamples, 0);
        if (!success)
            Debug.LogError("SetData failed — array length doesn't match clip length");
    }
}
*/