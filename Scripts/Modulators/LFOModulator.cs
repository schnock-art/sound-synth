using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst; 

//[CreateAssetMenu(menuName = "Modulator/LFO")]
public class LFOModulator : Modulator
{
    public LFOModulator(
        Waveform waveform, 
        float frequency, 
        float modulationDepth,
        float sampleRate)
    {
        this.sampleRate = sampleRate;
        this.frequency = frequency;
        if (waveform == null)
        {
            this.waveform = ScriptableObject.CreateInstance<SineWaveform>();
        } else {
            this.waveform = waveform;
        }
        this.waveform.Initialize();
        this.modulationDepth = modulationDepth;
        Initialize();
    }


    [BurstCompile]
    public override float GetModulationValue(){
        
        float modulationValue = waveform.Evaluate(phase);
        // Increment the phase based on the frequency
        phase = (phase + _increment) % 1.0f;
        return modulationValue;
    }

    
}