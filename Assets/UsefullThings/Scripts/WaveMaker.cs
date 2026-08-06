using UnityEngine;
using System.Collections.Generic;

public class WaveMaker : AudioPull
{
    public float[] frequencies = new float[0];

    private float[] phases = new float[0];
    private Envelope[] envelopes = new Envelope[0];
    private float sampleRate;

    private float fadeOutTimer = 0f;
    private float fadeOutDuration = 0.005f;
    private bool pendingFrequencyChange = false;

    private float[] pendingFrequencies = new float[0];
    private float[] pendingAmplitudes = new float[0];
    private int[] noteOffCountdowns = new int[0]; // samples until noteoff per note
    private readonly object lockObj = new object();
    private bool hasNewFrequencies = false;

//  sets sample rate settings based of whats im unity and buffer size
    public override void Init()
{
    sampleRate = AudioSettings.outputSampleRate;
    AudioSettings.GetDSPBufferSize(out int bufferSize, out _);
    buffer = new float[bufferSize];
}

    // length is in beats, bpm needed to convert to samples
    // sets target frequncies to be set as soon as buffer over
    // sets length of notes as countdowns
    public override void Play(float[] f, float[] amps, float[] lengths, float bpm)
    {
        int[] countdowns = new int[f.Length];
        for (int i = 0; i < f.Length; i++)
        {
            float noteSeconds = lengths[i] * (60f / bpm);
            countdowns[i] = Mathf.RoundToInt(noteSeconds * sampleRate);
        }

        lock (lockObj)
        {
            pendingFrequencies = f;
            pendingAmplitudes = amps;
            noteOffCountdowns = countdowns;
            hasNewFrequencies = true;
        }
    }


    // if new frequncies, great! sets them all up
    void CheckForNewFrequencies()
    {
        lock (lockObj)
        {
            if (hasNewFrequencies)
            {
                fadeOutTimer = 0f;
                pendingFrequencyChange = true;
                hasNewFrequencies = false;
            }
        }
    }


    // first, check if notes are siilar! if so then set same phase
    // then crete new envelopes for everyone
    // then set no new frequncies!
    void ApplyFrequencyChange()
    {
        float[] newPhases = new float[pendingFrequencies.Length];
        if (phases != null && frequencies != null)
        {
            for (int i = 0; i < pendingFrequencies.Length; i++)
            {
                for (int j = 0; j < frequencies.Length; j++)
                {
                    if (j < phases.Length && Mathf.Approximately(pendingFrequencies[i], frequencies[j]))
                    {
                        newPhases[i] = phases[j];
                        break;
                    }
                }
            }
        }

        phases = newPhases;
        frequencies = pendingFrequencies;

        envelopes = new Envelope[pendingFrequencies.Length];
        for (int i = 0; i < pendingFrequencies.Length; i++)
        {
            envelopes[i] = new Envelope
            {
                attack = 0.05f,
                decay = 0.1f,
                sustain = 0.5f,
                release = 0.3f,
                amplitude = pendingAmplitudes[i]

            };
            envelopes[i].NoteOn();
        }

        pendingFrequencyChange = false;
    }

    // for all the notes, stop them!
    public void NoteOff()
    {
        foreach (Envelope env in envelopes)
            env.NoteOff();
    }

    // needed as part of abstract class
    // clears buffer
    // if gettingg a new note, quickly fades out old frequncy to prevent clicks
    // when faded out sets new frequncys
    // then for each note, play them for length given to wave in package, if off turn that note off!
    // process every envelope increment phase and then send off to buffer :)
    public override float[] Pull(int framesRequested)
    {
        CheckForNewFrequencies();

        int chordSize = frequencies != null ? frequencies.Length : 0;
        System.Array.Clear(buffer, 0, framesRequested);

        if (chordSize == 0 && !pendingFrequencyChange) return buffer;

        float deltaTime = 1f / sampleRate;

        for (int j = 0; j < framesRequested; j++)
        {
            float fadeScale = 1f;

            if (pendingFrequencyChange)
            {
                fadeOutTimer += deltaTime;
                fadeScale = 1f - (fadeOutTimer / fadeOutDuration);

                if (fadeOutTimer >= fadeOutDuration)
                {
                    ApplyFrequencyChange();
                    chordSize = frequencies.Length;
                    fadeScale = 0f;
                }
            }

            for (int note = 0; note < chordSize; note++)
            {
                // count down to noteoff
                if ( note < noteOffCountdowns.Length && noteOffCountdowns[note] > 0 )
                {
                    noteOffCountdowns[note]--;
                    if (noteOffCountdowns[note] == 0)
                        envelopes[note].NoteOff();
                }

                float env = envelopes[note].Process(deltaTime);
                float phaseIncrement = frequencies[note] / sampleRate;
                buffer[j] += (Mathf.Sin(phases[note] * 2f * Mathf.PI) / Mathf.Max(chordSize, 1)) * env * fadeScale;
                phases[note] = (phases[note] + phaseIncrement) % 1f;
            }
        }

        return buffer;
    }
}

// millions of these bad boys
// can be attacking decaying sustaining releasing
// goes from one to other
// mess with these settings to change synth tone
public class Envelope
{
    public float amplitude = 1f;
    public float attack = 0.05f;
    public float decay = 0.1f;
    public float sustain = 0.5f;
    public float release = 0.3f;

    public float currentValue = 0f;

    public enum Stage { Idle, Attack, Decay, Sustain, Release }
    public Stage stage = Stage.Idle;

    public void NoteOn()
    {
        stage = Stage.Attack;
        currentValue = 0f;
    }

    public void NoteOff()
    {
        stage = Stage.Release;
    }

    public float Process(float deltaTime)
    {
        switch (stage)
        {
            case Stage.Attack:
                currentValue += deltaTime / attack;
                if (currentValue >= amplitude)
                {
                    currentValue = amplitude;
                    stage = Stage.Decay;
                }
                break;

            case Stage.Decay:
                currentValue -= deltaTime / decay;
                if (currentValue <= sustain * amplitude)
                {
                    currentValue = sustain * amplitude;
                    stage = Stage.Sustain;
                }
                break;

            case Stage.Sustain:
                currentValue = sustain * amplitude;
                break;

            case Stage.Release:
                currentValue -= deltaTime / release;
                if (currentValue <= 0f)
                {
                    currentValue = 0f;
                    stage = Stage.Idle;
                }
                break;
        }
        
        return currentValue;
    }
}