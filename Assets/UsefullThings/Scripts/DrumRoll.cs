using System.Collections.Generic;
using UnityEngine;

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

}
