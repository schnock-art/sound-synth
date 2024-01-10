using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FilterConfig : ScriptableObject
{
    public float cutoffFrequency = 440.0f;  // Cutoff frequency for the filter
    public float resonance = 1.0f;        // Resonance (or Q) of the filter
    public float sampleRate { get; private set; }
    public abstract Filter CreateFilter();

    public abstract void ApplyToExistingFilter(Filter filter);

    public void UpdateParameters(float newSampleRate,
        float newCutoffFrequency, float newResonance)
    {
        this.sampleRate = newSampleRate;
        // Update the filter's internal parameters
        this.cutoffFrequency = newCutoffFrequency;
        this.resonance = newResonance;
    } 

    public void SetSampleRate(float newSampleRate)
    {
        this.sampleRate = newSampleRate;
    }
}
