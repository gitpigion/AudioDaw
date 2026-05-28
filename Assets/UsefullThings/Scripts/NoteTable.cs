using UnityEngine;
using System.Collections.Generic;
using System.Net;

public static class NoteTable
{
    public static readonly Dictionary<int, float> NoteFrequencies = new Dictionary<int, float>
    {
        { 0,  138.59f },  // C#3
        { 1,  146.83f },  // D3
        { 2,  155.56f },  // Eb3
        { 3,  164.81f },  // E3
        { 4,  174.61f },  // F3
        { 5,  185.00f },  // F#3
        { 6,  196.00f },  // G3
        { 7,  207.65f },  // Ab3
        { 8,  220.00f },  // A3
        { 9,  233.08f },  // Bb3
        { 10, 246.94f },  // B3
        { 11, 261.63f },  // C4
    };

    public static float GetFrequency(int note) => NoteFrequencies[note];
}
