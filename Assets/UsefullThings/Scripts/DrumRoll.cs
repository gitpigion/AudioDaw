    using System.Collections.Generic;
    using System.Collections;
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

public GameObject segmentPrefab;

public VideoController vidController;

public GameObject instrumentPrefab;

public MusicLookup musicLookup;

public bool segmentPlayThrough;

public int y = 8;
public int x = 20;

public int bpm = 120;
public int cellsPerBeat = 4;

public int loopSize;
private int loopPosition;

public GameObject LineGameObject;
private LineRenderer Line;

public Vector3 LineEndPoint;
public Vector3 LineStartPoint;

UITouch uI;
//      larger array for each instrument
float [][,] amplitudes;
float [,] lengths;
DrumCell [,] cells;


List<GameObject> segmentObjs;
List<Segment> segmentClasses; // only list as each segment has arrays

List<GameObject> instrumentObjs;
List<Instrument> instrumentClasses;

int amountOfSegments=0;
int amountOfInstruments;

Segment[][] InstrumentsSegments;
// this list will be same length as audio midis
// this will be updated with an array of empty segments when a new instrument is made

public int curSegment;
int curInstrument;

public float xPosSegments;
public float yPosSegments;

public float segmentSeperation;

public float xPosInstruments;
public float yPosInstruments;

public float instrumentSeperation;


public float squashValueX;

public int StartingSegmentCount = 1;
float squashValueY;

float moveX = 0;
float moveY = 0;

    void Awake()
    {
        segmentClasses = new List<Segment>();
        segmentObjs = new List<GameObject>();
        instrumentClasses = new List<Instrument>();
        instrumentObjs = new List<GameObject>();
    }
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
    
    cells = new DrumCell[x,y];
    
    CreateRoll();
    
    for (int i = 0; i < StartingSegmentCount; i++)
        {
            CreateSegment();
        }
    StartCoroutine(WaitAndRunRoutine());
   
}
IEnumerator WaitAndRunRoutine()
{
    // Wait for 1 real-world second
    yield return new WaitForSeconds(1f);

    musicLookup.Process();
    vidController.Play();
    
}

public void CreateRoll(float[][,] amplitudes = null)
    {
        if (amplitudes != null)
        {
            this.amplitudes = amplitudes;
        }
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


//     handy way to set any value anywhere! setting -1 on segment or instrument will set them to defualt values
public void SetCell(int row, int col, float amplitude, int Segment = -1, int Instrument = -1)
{
    //Debug.Log($"row (y) {row} col (x) {col} amp {amplitude} segment {Segment} Instrument {Instrument}");
    if (Instrument == -1)
        {
            Instrument = curInstrument;
        }
    if (Segment == -1)
        {
            Segment = curSegment;
        }

    segmentClasses[Segment].amps[Instrument][col, row] = amplitude;
    
}


//      adds a soundmaster to audiomidis
//      gets each segment to make a new instrument
//      creates an instrument button prefab
public void AddSound(SoundMaster soundMaster)
{
    audioMidis.Add(soundMaster.audioMidi);
    amountOfInstruments++;

    // If there are no segments yet, make one first
    if (segmentClasses.Count == 0)
    {
        CreateSegment();
    }

    // Tell every segment to add a new instrument layer
    foreach (Segment segment in segmentClasses)
    {
        segment.AddInstrument(x, y);
        
    }
    if (amountOfInstruments ==1)
        {
            amplitudes = new float[1][,];
            amplitudes[0] = new float [x,y];
        }
    else
    {

        float [][,] newAmps = new float[amplitudes.Length+1][,];
        for (int i = 0; i < amplitudes.Length; i++)
        {
            newAmps[i] = amplitudes[i];
        }   
        newAmps[amplitudes.Length] = new float[x,y];
        amplitudes = newAmps;
        }
    

    // Create instrument button UI
    CreateInstrument(soundMaster);
}

//      creates an instrument button, positions it vertically, sets index
void CreateInstrument(SoundMaster soundMaster)
{
    GameObject instObj = Instantiate(instrumentPrefab);
    Instrument instClass = instObj.GetComponent<Instrument>();
    
    instrumentObjs.Add(instObj);
    instrumentClasses.Add(instClass);

    instClass.Index = amountOfInstruments - 1;
    instClass.soundMaster = soundMaster;
    
    instObj.transform.position = new Vector3(
        xPosInstruments, 
        yPosInstruments + (1 + instClass.Index) * instrumentSeperation, 
        0);
}

//      allows any segment to be updated with new amps
public void UpdateSegment(float [,] newamplitudes, int Segment, int Instrument)
{
    segmentClasses[Segment].amps[Instrument] = newamplitudes;
}


//      increases amount of segments vairable at the END so that it starts at position 0,
//  instantiates a segment gameobject, gets the segment class on the gameobeject
//      sets index on segment, sets position of segment on screen, adds instruments for each instrument
public void CreateSegment()
    {
        
        
        GameObject seg = Instantiate(segmentPrefab);

        Segment segClass;
        
        segmentObjs.Add(seg);
        segClass = seg.GetComponent<Segment>();
        segmentClasses.Add(segClass);
        segClass.drumRoll = this;
        segClass.Index = amountOfSegments;
        seg.transform.position = new Vector3 (xPosSegments + (1+segClass.Index) * segmentSeperation,yPosSegments,0);

        for (int i = 0; i < amountOfInstruments; i++)
        {
            segClass.AddInstrument(x,y);
        }

        curSegment = segClass.Index;
        amountOfSegments++;
    }


//      sets current segment,changes the amplitudes for each instrument in the segment
//      changes the amplitudes of the cells and updates the visual to match
public void SwitchSegment(int segment)
    {
        curSegment = segment;
        for (int i = 0; i < amountOfInstruments; i++)
        {
            amplitudes[i] = segmentClasses[curSegment].amps[i];
        }
        
        Debug.Log("switching segment to " + segment + " on instrument " + curInstrument);
        for (int i = 0; i< x; i++)
        {
            for (int j = 0; j <y; j++)
            {
                cells[i,j].amplitude = amplitudes[curInstrument][i,j];
                cells[i,j].UpdateVisual(); 
            }
        }
    }

//      sets current instrument, changes the amplitudes displayed in the drum roll
public void SwitchInstrument(int instrument)
{
    curInstrument = instrument;
    
    Debug.Log("switching segment to " + curSegment + " on instrument " + instrument);
    for (int i = 0; i < x; i++)
    {
        for (int j = 0; j < y; j++)
        {
            cells[i, j].amplitude = amplitudes[curInstrument][i, j];
            cells[i, j].UpdateVisual();
        }
    }
}

float timer = 0;

//      moves line visual, if its time for a new beat, loops through all instruments and plays the notes on the accociated midis
void Update()
{
    Line.transform.position = new Vector3 (Line.transform.position.x +( Time.deltaTime/(60f/(bpm * cellsPerBeat)))*squashValueX, 
    Line.transform.position.y, Line.transform.position.z);
    timer += Time.deltaTime;
    if (timer >= 60f/(bpm * cellsPerBeat) )
        {
            timer = 0;
            for (int i = 0; i < amountOfInstruments; i++)
            {
                CallMidis(audioMidis[i], i);
            }
            loopPosition++;
        if (loopPosition >= x)
        {
            if (segmentPlayThrough)
            {
                if (curSegment >= amountOfSegments - 1)
                {
                    SwitchSegment(0);
                    vidController.RestartVideo();
                }
                else
                {
                    SwitchSegment(curSegment + 1);
                }
                
            }
            loopPosition = 0;
            resetLine();
            
        }
            
        }
    
}

private float[] compNoteBatch = new float[0];
private float[] compAmpBatch = new float[0];
private float[] compLengthBatch = new float[0];


//      makes lits, for each note in the vertical strip that beat, sets amplitudes of selected instrument equal to the saved amplitudes
//      same with length, and then adds frequency amps and lengths to lists.
//      these are set to arrays and then they are sent off to midi, if the beat has reached the end of the line it resets
void CallMidis(AudioMidi midi, int Instrument)
    {

        List<float> noteBatch = new List<float>();
        List<int> sampleBatch = new List<int>();
        List<float> ampBatch = new List<float>();
        List<float> lengthBatch = new List<float>();
        for (int i = 0; i < y; i++)
        {
            float amplitude = amplitudes[Instrument][loopPosition, i];
            float length = cells[loopPosition, i].noteLength;
            if (amplitude >0)
            {
                sampleBatch.Add(i);
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
            midi.Play(compAmpBatch, compNoteBatch, compLengthBatch, sampleBatch.ToArray());   
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
