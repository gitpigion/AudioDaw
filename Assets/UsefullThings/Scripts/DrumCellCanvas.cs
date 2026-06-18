using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrumCellCanvas : MonoBehaviour
{
    public int row;
    public int col;
    public float amplitude = 0f;

    public float noteLength = 0f;

    private DrumRoll drumRoll;
    private TextMeshPro textMeshPro;
    private Image rend;

    private Color baseColor;

    static readonly int[] blackKeys = { 1, 3, 6, 8, 10 }; // Eb, F#, Ab, Bb, C#  in low to high order matching NoteTable

    void Start()
    {
        rend = GetComponent<Image>();
        drumRoll = GetComponentInParent<DrumRoll>();

        baseColor = IsBlackKey(row) ? Color.black : Color.white;
        rend.color = baseColor;

        textMeshPro = GetComponentInChildren<TextMeshPro>();
        UpdateVisual();
    }

    bool IsBlackKey(int note)
    {
        foreach (int b in blackKeys)
            if (b == note) return true;
        return false;
    }

    public void OnClick()
    {
        amplitude = amplitude > 0f ? 0f : 1f;
        noteLength = noteLength > 0f ? 0f : 1f;
        UpdateVisual();
        drumRoll.SetCell(row, col, amplitude);
    }

    void UpdateVisual()
    {
        if (amplitude > 0f)
            rend.material.color = Color.cyan;
        else
            rend.material.color = baseColor;
        textMeshPro.text = noteLength.ToString();
        Debug.Log(amplitude);
    }
}