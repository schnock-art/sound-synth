using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Synth/Filters/LowPassConfig")]
public class LowPassFilterConfig : FilterConfig
{
    
    public override Filter CreateFilter()
    {
        return new LowPassFilter(
            this.cutoffFrequency, 
            this.resonance, 
            this.sampleRate
        );
         
    }

    public override void ApplyToExistingFilter(Filter filter)
    {
        
        filter.cutoffFrequency = this.cutoffFrequency;
        filter.resonance = this.resonance;
        filter.sampleRate = this.sampleRate;
    }
}