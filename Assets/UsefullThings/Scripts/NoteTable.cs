using UnityEngine;
using System.Collections.Generic;
using System.Net;

public static class NoteTable
{
   public static readonly Dictionary<int, float> NoteFrequencies = new Dictionary<int, float>
{
    { 0,  277.18f },  // C#4
    { 1,  293.66f },  // D4
    { 2,  311.12f },  // Eb4
    { 3,  329.63f },  // E4
    { 4,  349.23f },  // F4
    { 5,  369.99f },  // F#4
    { 6,  392.00f },  // G4
    { 7,  415.30f },  // Ab4
    { 8,  440.00f },  // A4
    { 9,  466.16f },  // Bb4
    { 10, 493.88f },  // B4
    { 11, 523.25f },  // C5
};

    public static float GetFrequency(int note) => NoteFrequencies[note];
}
