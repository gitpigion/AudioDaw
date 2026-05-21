using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DrumRoll : MonoBehaviour
{
    /*
    large class that creates drum roll, runs through it in frames that are normalized to fps
    when it enters a new beat, if there is any midi information, sends it to the midi, this information includes

        -   bool active/inactive
            this is good as it allows processing to only be done on active beats
            the audio midi can still be outputing signal if the drum roll hits an inactive beat,
            as the note may be fading off or sustained.
            when a new active beat is hit, the envelope should be reset to note hit
        
        -   float[] target frequncies
            this is sent to the audiomidi to then be sent on to audio creator

        -   float amplitude
            this is sent to the audio midi to control the volume of the sound

    */

public List<AudioMidi> audioMidis;

public GameObject cellPrefab;

public int y = 8;
public int x = 20;

float [,] amplitudes;

void Start()
{   
    amplitudes = new float[y,x];

    for (int row = 0; row < y; row++)
    {
        for (int col = 0; col < x; col++)
        {
            GameObject cell = Instantiate(cellPrefab, transform);
            cell.transform.localPosition = new Vector3(col, row, 0);
            
            DrumCell dc = cell.GetComponent<DrumCell>();
            dc.row = row;
            dc.col = col;
        }
    }
}

public void SetCell(int row, int col, float amplitude)
{
    amplitudes[row, col] = amplitude;
}

void Update()
{
    // in DrumRoll Update instead of OnMouseDown on each cell
    if (Mouse.current.leftButton.wasPressedThisFrame)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {   
            DrumCell cell = hit.collider.GetComponent<DrumCell>();
            if (cell != null)
                cell.OnClick();
        }
    }
}

}
