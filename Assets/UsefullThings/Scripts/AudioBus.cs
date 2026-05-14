using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AudioBus : AudioPull
{
    public string name;
    public float volume = 1f;

    public int channels;
    
    
    [SerializeReference]
    public List<AudioEffect> effectsChain;
    
    public float[] buffer;

    public override float[] Pull(int framesRequested)
    {

        throw new System.NotImplementedException();
        // this is where it grabs the other pulls from any other busses / audio creators
        // timing stuff may be needed, check if sound is made etc (or that could be ANOTHER ABSTRACT CLASS
        // WITH A FUNCTION LIKE SEND AUDIO THAT RETURNS EITHER PREMADE OR NEW SIGNAL)
    }
}   