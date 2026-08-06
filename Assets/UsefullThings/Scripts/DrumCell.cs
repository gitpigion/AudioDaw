using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class DrumCell : MonoBehaviour
{
    public int row;
    public int col;
    public float amplitude = 0f;

    public float noteLength = 0f;

    private DrumRoll drumRoll;

    private SpriteRenderer squareRenderer;
    public GameObject square;
    private TextMeshPro textMeshPro;
    private Renderer rend;

    private Color baseColor;

    RectTransform rect; 
    Vector2 initialPos; 

    static readonly int[] blackKeys = { 1, 3, 6, 8, 10 }; // Eb, F#, Ab, Bb, C#  in low to high order matching NoteTable

    void Start()
    {
        rend = GetComponent<Renderer>();
        drumRoll = GetComponentInParent<DrumRoll>();

        baseColor = IsBlackKey(row) ? Color.black : Color.white;
        rend.material.color = baseColor;

        textMeshPro = GetComponentInChildren<TextMeshPro>();

        squareRenderer = square.GetComponent<SpriteRenderer>();
        
        squareRenderer.material.color = Color.clear;

        rect = square.GetComponent<RectTransform>();
        initialPos = rect.anchoredPosition;
        
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

    public void UpdateVisual()
    {
        if (amplitude > 0f)
            squareRenderer.material.color = Color.cyan;
        else
            squareRenderer.material.color = baseColor;
        textMeshPro.text = amplitude.ToString();

        
        rect.localScale = new Vector2 (1, amplitude);
        rect.anchoredPosition = new Vector2(initialPos.x, initialPos.y + amplitude/2 -0.5f);
        drumRoll.SetCell(row, col, amplitude);
    }
}