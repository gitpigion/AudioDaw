using UnityEngine;

public class AudioMidi : MonoBehaviour
{
    AudioPull audioPull;
    public int bpm = 120;

    public void SetWave(WaveMaker wave)
    {
        audioPull = wave;
        audioPull.Init();
    }

    public void SetAudio(AudioPull pull, Stage stage)
    {
        this.stage = stage;
        audioPull = pull;
        audioPull.Init();
    }

    public enum Stage{Synth, Drums}
    public Stage stage;
    

    public void Play(float[] amplitudes, float[] frequencies, float[] lengths, int[] samples)
    {
        switch (stage)
        {
            case Stage.Synth:
            audioPull.Play(frequencies, amplitudes, lengths, bpm);
            break;
            
            case Stage.Drums:
            audioPull.Play(samples);
            break;
        }
        
    }

 
}