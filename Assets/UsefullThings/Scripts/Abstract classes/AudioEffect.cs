using UnityEngine;

[System.Serializable]
public abstract class AudioEffect
{
     public bool enabled = true;
    public abstract void Process(float[] buffer, int channels);
}
