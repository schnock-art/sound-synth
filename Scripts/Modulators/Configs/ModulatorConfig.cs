using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModulatorConfig : ScriptableObject
{
    public float _frequency = 1.0f;  // Cutoff frequency for the filter
    public float sampleRate { get; private set; }
    public float modulationDepth = 1.0f;
    public Waveform waveform; // Reference to another ScriptableObject
    public abstract Modulator CreateModulator();

    public abstract void ApplyToExistingModulator(Modulator modulator);

    public void SetSampleRate(float newSampleRate)
    {
        this.sampleRate = newSampleRate;
    }
}
