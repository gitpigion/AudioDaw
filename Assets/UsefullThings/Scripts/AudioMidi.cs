using UnityEngine;

public class AudioMidi : MonoBehaviour
{
    WaveMaker waveMaker;
    public int bpm = 120;

    public void SetWave(WaveMaker wave)
    {
        waveMaker = wave;
        waveMaker.Init();
    }

    public void Play(float[] amplitudes, float[] frequencies, float[] lengths)
    {
        waveMaker.SetFrequency(frequencies, amplitudes, lengths, bpm);
    }

    public void NoteOff()
    {
        waveMaker.NoteOff();
    }
}