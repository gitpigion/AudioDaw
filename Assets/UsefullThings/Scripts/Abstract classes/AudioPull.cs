using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class AudioPull
{
    [SerializeReference]
    public List<AudioPull> inputs;
    
    public abstract float[] Pull(int framesRequested);
}
