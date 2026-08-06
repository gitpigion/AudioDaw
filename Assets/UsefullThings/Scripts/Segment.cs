using UnityEngine;

public class Segment : MonoBehaviour
{
    public int Index;

    // amps[instrument][x,y]
    public float[][,] amps;


    //      if no amplitudes logged (first run) creates array with size equal to the keybed (x,y) supplied
    //      creates a slightly larger array to accomadate amps for a new instrument and then sets all crossover
    public void AddInstrument(int x, int y)
    {
        if (amps == null)
        {
            amps = new float[1][,];
            amps[0] = new float[x, y];
            return;
        }

        float[][,] newAmps = new float[amps.Length + 1][,];

        // Copy existing instrument layers
        for (int i = 0; i < amps.Length; i++)
        {
            newAmps[i] = amps[i];
        }

        // Add a new empty layer
        newAmps[amps.Length] = new float[x, y];

        amps = newAmps;
    }
}