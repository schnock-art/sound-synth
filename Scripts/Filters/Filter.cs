using UnityEngine;

public abstract class Filter
{
    private float _cutoffFrequency = 440.0f;  // Cutoff frequency for the filter
    private float _resonance = 1.0f;         // Resonance (or Q) of the filter
    private float _sampleRate;

    public float cutoffFrequency
    {
        get { return _cutoffFrequency; }
        set
        {
            _cutoffFrequency = value;
            //Initialize();
        }
    }

    public float resonance
    {
        get { return _resonance; }
        set
        {
            _resonance = value;
            //Initialize();
        }
    }

    public float sampleRate
    {
        get { return _sampleRate; }
        set
        {
            _sampleRate = value;
            //Initialize();
        }
    }

    public abstract float ApplyFilter(float sample);

    public abstract void Initialize();
}
