using UnityEngine;

public class Instrument : MonoBehaviour
{
    public int Index;
    public SoundMaster soundMaster;

    public void OnClick()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
