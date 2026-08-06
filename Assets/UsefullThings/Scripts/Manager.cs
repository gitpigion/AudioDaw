
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class Manager : MonoBehaviour
{
    public List<AudioBus> audioBuses;

    public List<SoundMaster> Sounds;

    public int channels;

    public GameObject prefab;

    private MasterSend masterSend;

    public DrumRoll drumRoll;
    // keeps track off what busses are connected to where, and what wavemakers are connected to
    // changes these connection around

    public void CreateBus()
    {
        audioBuses.Add(new AudioBus());
    }
    public void CreateSoundGameObject()
    {
        WaveMaker wave = new WaveMaker();
        GameObject waveObject = Instantiate(prefab);

        // class that just exists to have all 3 vairables in one, might change later if i hate it
        
        SoundMaster soundMaster = new SoundMaster(wave, waveObject, waveObject.GetComponent<AudioMidi>(), AudioMidi.Stage.Synth);
        Sounds.Add(soundMaster);
        drumRoll.AddSound(soundMaster);
        masterSend.Pulls.Add(wave);
        Debug.Log("created synth");
    }

    public void CreateSamplerGameObject()
    {
        Sampler sampler = new Sampler();
        GameObject sampleObject = Instantiate(prefab);

        SoundMaster soundMaster = new SoundMaster(sampler, sampleObject, sampleObject.GetComponent<AudioMidi>(), AudioMidi.Stage.Drums);
        Sounds.Add(soundMaster);
        drumRoll.AddSound(soundMaster);
        masterSend.Pulls.Add(sampler);
        Debug.Log("created drums");
    }

    public void Link(AudioPull input, AudioPull output)
    {
        // takes the input and adds the output to its inputs
        //output.inputs.Add(input);
    }


    void Start()
    {
        masterSend = gameObject.GetComponent<MasterSend>();
        var source = GetComponent<AudioSource>();
        source.loop = true;
        source.Play();
        CreateSoundGameObject();
        CreateSoundGameObject();
        //CreateSamplerGameObject();
        // testing
        

        
    }

}

[System.Serializable]
public class SoundMaster
{
    public AudioPull AudioPull;
    public GameObject gameObject;
    public AudioMidi audioMidi;

    public SoundMaster(AudioPull waveMaker, GameObject gameObject, AudioMidi audioMidi, AudioMidi.Stage stage)
    {
        this.AudioPull = waveMaker;
        this.gameObject = gameObject;
        this.audioMidi = audioMidi;
        audioMidi.SetAudio(waveMaker, stage);
    }
}