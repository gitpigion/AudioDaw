using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class AudioPull
{
    protected float[] buffer;

    public virtual void Init()
    {
       
    }

    public virtual void Play(int[] notes) { }

    public virtual void Play(float[] f, float[] amplitudes, float[] notes, float bpm) { }

    // writes into buffer and returns it, no new allocation
    public abstract float[] Pull(int framesRequested);
}
