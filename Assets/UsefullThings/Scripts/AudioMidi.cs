using UnityEngine;

public class AudioMidi : MonoBehaviour
{
    WaveMaker waveMaker;

    public void SetWave(WaveMaker wave)
    {
        waveMaker = wave;
        waveMaker.Init();  // initialise sample rate as soon as wave is set
    }

    public void Play(float[] amplitudes, float[] tarFrequencies)
    {
        waveMaker.SetFrequency(tarFrequencies);
    }
}