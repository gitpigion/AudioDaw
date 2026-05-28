using UnityEngine;

public class AudioMidi : MonoBehaviour
{
    public void SetWave(WaveMaker wave)
    {
        waveMaker = wave;
    }
    WaveMaker waveMaker;

    public void Play(float[] amplitudes, float[] TarFrequencys)
    {
        
        waveMaker.SetFrequency(TarFrequencys);
    }
    
}
