using UnityEngine;

public class DrumCell : MonoBehaviour
{
    public int row;
    public int col;
    public float amplitude = 0f;

    private DrumRoll drumRoll;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        drumRoll = GetComponentInParent<DrumRoll>();
    }



    void UpdateVisual()
    {
        rend.material.color = amplitude > 0f ? Color.cyan : Color.gray;
    }

    public void OnClick()
    {
    amplitude = amplitude > 0f ? 0f : 1f;
    UpdateVisual();
    drumRoll.SetCell(row, col, amplitude);
    }
}
