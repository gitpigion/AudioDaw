
using UnityEngine;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{
    public List<AudioBus> audioBuses;

    public List<SoundMaster> Sounds;

    public int channels;

    public GameObject prefab;

    private MasterSend masterSend;

    private DrumRoll drumRoll;
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

        SoundMaster soundMaster = new SoundMaster(wave, waveObject, waveObject.GetComponent<AudioMidi>());
        Sounds.Add(soundMaster);
        drumRoll.AddSound(soundMaster);
    }

    public void Link(AudioPull input, AudioPull output)
    {
        // takes the input and adds the output to its inputs
        //output.inputs.Add(input);
    }


    void Start()
    {
        masterSend = gameObject.GetComponent<MasterSend>();
        drumRoll = gameObject.GetComponent<DrumRoll>();
        var source = GetComponent<AudioSource>();
        source.loop = true;
        source.Play();
        CreateSoundGameObject();
        // testing
        masterSend.Pulls.Add(Sounds[0].waveMaker);

        
    }

}

[System.Serializable]
public class SoundMaster
{
    public WaveMaker waveMaker;
    public GameObject gameObject;
    public AudioMidi audioMidi;

    public SoundMaster(WaveMaker waveMaker, GameObject gameObject, AudioMidi audioMidi)
    {
        this.waveMaker = waveMaker;
        this.gameObject = gameObject;
        this.audioMidi = audioMidi;
        audioMidi.SetWave(waveMaker);
    }
}