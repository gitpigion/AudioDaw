using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class AudioPull
{
    protected float[] buffer;

    public virtual void Init(int bufferSize)
    {
        buffer = new float[bufferSize];
    }

    // writes into buffer and returns it, no new allocation
    public abstract float[] Pull(int framesRequested);
}
