using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

public abstract class Modulator
{
    [SerializeField]
    
    private float _frequency = 1.0f;

    private float minValue = -1.0f;
    private float maxValue = 1.0f;

    public float frequency
    {
        get { return _frequency; }
        set
        {
            _frequency = value;
            //UpdateIncrement();
        }
    }
    private float _sampleRate;

    public float sampleRate
    {
        get { return _sampleRate; }
        set
        {
            _sampleRate = value;
            //UpdateIncrement();
        }
    }
    protected float _increment;

    protected float phase = 0.0f;
    public Waveform waveform; // Reference to another ScriptableObject
    
    public float modulationDepth = 1.0f;

    public void ResetPhase()
    {
        phase = 0.0f;
    }

    public void Initialize()
    {
        //_sampleRate = AudioSettings.outputSampleRate;
        UpdateIncrement();
        ResetPhase();
        waveform.Initialize();
    }

    public void SetWaveformRange(float minValue, float maxValue)
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
        waveform.SetTableRange(minValue, maxValue);
    }

    //public void SetFrequency(float frequency)
    //{
    //    _frequency = frequency;
    //    UpdateIncrement();
    //}

    [BurstCompile]
    protected void UpdateIncrement()
    {
        _increment = _frequency / _sampleRate;
    }

    public abstract float GetModulationValue();

    //public void SetSampleRate(float sampleRate)
    //{
    //    _sampleRate = sampleRate;
    //    UpdateIncrement();
    //}
}
