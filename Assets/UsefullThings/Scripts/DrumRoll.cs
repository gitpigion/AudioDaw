using System.Collections.Generic;
using System.Net;
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

public int bpm = 120;

public int loopSize;
private int loopPosition;

public GameObject LineGameObject;
private LineRenderer Line;

public Vector3 LineEndPoint;
public Vector3 LineStartPoint;

float [,] amplitudes;
DrumCell [,] cells;

float squashValueX;
float squashValueY;
void Start()
{   
    squashValueX = cellPrefab.transform.localScale.x  ;
    squashValueY = cellPrefab.transform.localScale.y ;
    LineStartPoint = new Vector3(this.transform.position.x, 
    this.transform.position.y-squashValueY/2 , this.transform.position.z);;
    LineEndPoint = new Vector3(this.transform.position.x, 
    this.transform.position.y + y*squashValueY-squashValueY/2, this.transform.position.z);
    
    Line = Instantiate<GameObject>(LineGameObject).GetComponent<LineRenderer>();
    Line.SetWidth(0.1f, 0.1f);
    Line.SetPosition(0, LineStartPoint);
    Line.SetPosition(1, LineEndPoint);
    resetLine();
    amplitudes = new float[x,y];
    cells = new DrumCell[x,y];

    for (int row = 0; row < y; row++)
    {
        for (int col = 0; col < x; col++)
        {
            GameObject cell = Instantiate(cellPrefab, transform);
            cell.transform.localPosition = new Vector3(col*squashValueX, row * squashValueY, 0);
            
            DrumCell dc = cell.GetComponent<DrumCell>();
            dc.row = row;
            dc.col = col;
            cells[col,row] = dc;
        }
    }
}

public void SetCell(int row, int col, float amplitude)
{
    amplitudes[col, row] = amplitude;
}

float timer = 0;
void Update()
{
    Line.transform.position = new Vector3 (Line.transform.position.x +( Time.deltaTime/(60f/bpm))*squashValueX, 
    Line.transform.position.y,
    Line.transform.position.z);
    timer += Time.deltaTime;
    if (timer >= 60f/bpm )
        {
            timer = 0;
            CallMidis(audioMidis[0]);
        }

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

void CallMidis(AudioMidi midi)
    {
        for (int i = 0; i < y; i++)
        {
            midi.Play(cells[loopPosition,i].amplitude);
            Debug.Log(cells[loopPosition,i].amplitude + " " + loopPosition + " x= "+ x + " y = "+ y);
        }
        loopPosition++;
        if (loopPosition >= x)
        {
            loopPosition = 0;
            resetLine();
        }

    }

    void resetLine()
    {
        Line.transform.position = new Vector3(-squashValueX,0,0);
    }

/*
    drum roll will jump from beat to beat, with a rate of 60BPM
    with a period of 1/60BPM    in the update it will add to a timer and reset this every beat length
    when hits a beat, runs through list of audiomidis and sends signal
*/

}
