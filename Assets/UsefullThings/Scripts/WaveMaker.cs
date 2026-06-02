using UnityEngine;
using System.Collections.Generic;

public class WaveMaker : AudioPull
{
    public float[] frequencies;

    private float[] frequencyBuffer = new float[0];
    private float[] phases = new float[0];
    private Envelope[] envelopes = new Envelope[0];
    private List<ReleasingNote> releasePool = new List<ReleasingNote>();
    private float sampleRate;

    public void Init()
    {
        sampleRate = AudioSettings.outputSampleRate;
    }

    public void SetFrequency(float[] f)
    {
        // move old envelopes into release pool
        for (int i = 0; i < envelopes.Length; i++)
        {
            envelopes[i].NoteOff();
            if (i < phases.Length)
                releasePool.Add(new ReleasingNote(envelopes[i], phases[i], frequencies[i]));
        }

        frequencyBuffer = f;
        frequencies = f;

        envelopes = new Envelope[f.Length];
        // gets all the phases for the notes, so next set they can be held
        phases = new float[f.Length];
        for (int i = 0; i < f.Length; i++)
        {
            envelopes[i] = new Envelope { attack = 0.1f, decay = 0.1f, sustain = 0.5f, release = 0.5f };
            envelopes[i].NoteOn();
        }
    }

    public override float[] Pull(int framesRequested)
    {
        frequencies = frequencyBuffer;
        int chordSize = frequencies.Length;

        float[] buffer = new float[framesRequested];

        if (chordSize == 0 && releasePool.Count == 0) return buffer;

        float deltaTime = 1f / sampleRate;

        for (int j = 0; j < framesRequested; j++)
        {
            // active notes
            for (int note = 0; note < chordSize; note++)
            {
                float env = envelopes[note].Process(deltaTime);
                float phaseIncrement = frequencies[note] / sampleRate;
                buffer[j] += (Mathf.Sin(phases[note] * 2f * Mathf.PI) / chordSize) * env;

                phases[note] += phaseIncrement;
                if (phases[note] >= 1f) phases[note] -= 1f;
            }

            // releasing notes from pool
            foreach (ReleasingNote rn in releasePool)
            {
                float env = rn.envelope.Process(deltaTime);
                float phaseIncrement = rn.frequency / sampleRate;
                buffer[j] += Mathf.Sin(rn.phase * 2f * Mathf.PI) * env;

                rn.phase += phaseIncrement;
                if (rn.phase >= 1f) rn.phase -= 1f;
            }
        }

        releasePool.RemoveAll(rn => rn.envelope.stage == Envelope.Stage.Idle);

        return buffer;
    }
}

public class ReleasingNote
{
    public Envelope envelope;
    public float phase;
    public float frequency;

    public ReleasingNote(Envelope envelope, float phase, float frequency)
    {
        this.envelope = envelope;
        this.phase = phase;
        this.frequency = frequency;
    }
}

public class Envelope
{
    public float attack = 0.1f;
    public float decay = 0.1f;
    public float sustain = 0.5f;
    public float release = 0.5f;

    public float currentValue = 0f;
    private float releaseStartValue = 0f;

    public enum Stage { Idle, Attack, Decay, Sustain, Release }
    public Stage stage = Stage.Idle;

    public void NoteOn()
    {
        stage = Stage.Attack;
        currentValue = 0f;
    }

    public void NoteOff()
    {
        releaseStartValue = currentValue;
        stage = Stage.Release;
    }

    public float Process(float deltaTime)
    {
        switch (stage)
        {
            case Stage.Attack:
                currentValue += deltaTime / attack;
                if (currentValue >= 1f)
                {
                    currentValue = 1f;
                    stage = Stage.Decay;
                }
                break;

            case Stage.Decay:
                currentValue -= deltaTime / decay;
                if (currentValue <= sustain)
                {
                    currentValue = sustain;
                    stage = Stage.Sustain;
                }
                break;

            case Stage.Sustain:
                currentValue = sustain;
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