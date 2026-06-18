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

UITouch uI;

float [,] amplitudes;
float [,] lengths;
DrumCell [,] cells;


public float squashValueX;
float squashValueY;

float moveX = 0;
float moveY = 0;
void Start()
{   
    moveX = transform.position.x;
    moveY = transform.position.y;
    uI = GetComponent<UITouch>();
    squashValueX = cellPrefab.transform.localScale.x  ;
    squashValueY = cellPrefab.transform.localScale.y ;
    LineStartPoint = new Vector3(this.transform.position.x, 
    this.transform.position.y-squashValueY/2 , this.transform.position.z);;
    LineEndPoint = new Vector3(this.transform.position.x, 
    this.transform.position.y + y*squashValueY-squashValueY/2, this.transform.position.z);
    DrawGrid();

    uI.Init(squashValueX, this);


    Line = Instantiate<GameObject>(LineGameObject).GetComponent<LineRenderer>();
    Line.   sortingOrder = 5;
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
            
            cell.transform.position= new Vector3(col*squashValueX + moveX, row * squashValueY + moveY, 0);
            
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

public void AddSound(SoundMaster soundMaster)
{
    audioMidis.Add(soundMaster.audioMidi);
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
    
}

private float[] compNoteBatch = new float[0];
private float[] compAmpBatch = new float[0];
private float[] compLengthBatch = new float[0];

void CallMidis(AudioMidi midi)
    {

        List<float> noteBatch = new List<float>();
        List<float> ampBatch = new List<float>();
        List<float> lengthBatch = new List<float>();
        for (int i = 0; i < y; i++)
        {
            float amplitude = amplitudes[loopPosition, i];
            float length = cells[loopPosition, i].noteLength;
            if (amplitude >0)
            {
                noteBatch.Add(NoteTable.GetFrequency(i));
                ampBatch.Add(amplitude);
                lengthBatch.Add(length);
            }


        }
        compNoteBatch = noteBatch.ToArray();
        compAmpBatch = ampBatch.ToArray();
        compLengthBatch = lengthBatch.ToArray();
        if (compNoteBatch.Length >0 )
        {
            midi.Play(compAmpBatch, compNoteBatch, compLengthBatch);   
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


   void DrawGrid()
{
    // vertical lines
    for (int col = 0; col <= x; col++)
    {
        float xPos = transform.position.x + col * squashValueX - squashValueX / 2;

        LineRenderer gridLine = Instantiate(LineGameObject).GetComponent<LineRenderer>();
        gridLine.sortingOrder = 5;
        gridLine.SetWidth(0.05f, 0.05f);
        gridLine.SetPosition(0, new Vector3(xPos, transform.position.y - squashValueY / 2, 0));
        gridLine.SetPosition(1, new Vector3(xPos, transform.position.y + y * squashValueY - squashValueY / 2, 0));
    }

    // horizontal lines
    for (int row = 0; row <= y; row++)
    {
        float yPos = transform.position.y + row * squashValueY - squashValueY / 2;

        LineRenderer gridLine = Instantiate(LineGameObject).GetComponent<LineRenderer>();
         gridLine.sortingOrder = 5;
        gridLine.SetWidth(0.05f, 0.05f);
        gridLine.SetPosition(0, new Vector3(transform.position.x - squashValueX / 2, yPos, 0));
        gridLine.SetPosition(1, new Vector3(transform.position.x + x * squashValueX - squashValueX / 2, yPos, 0));
    }
}

/*
    drum roll will jump from beat to beat, with a rate of 60BPM
    with a period of 1/60BPM    in the update it will add to a timer and reset this every beat length
    when hits a beat, runs through list of audiomidis and sends signal
*/

}
