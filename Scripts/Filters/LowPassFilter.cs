using UnityEngine;

//[CreateAssetMenu(menuName = "Filter/LowPass")]
public class LowPassFilter : Filter
{
    private float previousValue = 0.0f;
    private float alpha;

    public LowPassFilter(
        float initCutoffFrequency, 
        float initResonance, 
        float initSampleRate){
        this.cutoffFrequency = initCutoffFrequency;
        this.resonance = initResonance;
        this.sampleRate = initSampleRate;
        Initialize();
    }

    public override void Initialize()
    {
        previousValue = 0.0f;
        // Calculate alpha based on the cutoff frequency
        float dt = 1.0f / sampleRate;
        float rc = 1.0f / (cutoffFrequency * 2 * Mathf.PI);
        alpha = dt / (rc + dt);
        //Debug.Log("LowPassFilter.Initialize: "+ sampleRate + " " + dt +" " +rc +" " + alpha);
    }

    public override float ApplyFilter(float sample)
    {
        //Debug.Log("LowPassFilter.ApplyFilter: "+ sampleRate + " " + sample + " " + alpha + " " + previousValue + " " + (previousValue * (1.0f - alpha)) + " " + (sample * alpha + (previousValue * (1.0f - alpha))));
        //previousValue = sample * alpha + (previousValue * (1.0f - alpha));
        return sample; //previousValue;
    }
}
