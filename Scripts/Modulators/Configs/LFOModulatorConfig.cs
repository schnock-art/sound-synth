using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Synth/Modulators/LFOConfig")]
public class LFOModulatorConfig : ModulatorConfig
{
    
    public override Modulator CreateModulator()
    {
        return new LFOModulator(
            this.waveform,
            this._frequency, 
            this.modulationDepth, 
            this.sampleRate
        );
         
    }

    public override void ApplyToExistingModulator(Modulator modulator)
    {
        modulator.waveform = this.waveform;
        modulator.frequency = this._frequency;
        modulator.modulationDepth = this.modulationDepth;

    }

}
