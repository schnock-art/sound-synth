using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;
public class Voice{

    public Voice(int sampleRate,
            Waveform waveform, 
            ADSREnvelope envelope, 
            ModulatorConfig amplitudeModulatorConfig,
            ModulatorConfig frequencyModulatorConfig,
            FilterConfig filterConfig,
            VoicePoolManager voicePoolManager
        ){
        this.voicePoolManager = voicePoolManager;
        this.sampleRate = sampleRate;
        this.timeDelta = 1.0f / sampleRate;
        SetWaveform(waveform);
        SetEnvelope(envelope);
        ReplaceAmplitudeModulator(amplitudeModulatorConfig);
        ReplaceFrequencyModulator(frequencyModulatorConfig);
        ReplaceFilter(filterConfig);
    }
    public bool IsActive { get; private set; } = false;
    public float Phase { get; private set; } = 0.0f;
    public float Frequency { get; private set; } = 0.0f;

    //public float Amplitude { get; private set; } = 1.0f;
    private float phaseIncrement;
    private Waveform waveform;
    private ADSREnvelope envelope;

    private Filter filter;

    private Modulator amplitudeModulator;

    private Modulator frequencyModulator;

    private int sampleRate;
    private float timeDelta;

    private float timeSinceNoteOn;

    private float pan = 0.0f;  // Default to center

    private float leftAmp = 0.5f;
    private float rightAmp = 0.5f;

    private const float AMPLITUDE_THRESHOLD = 0.1f;

    private VoicePoolManager voicePoolManager;

    public void Start(float frequency, float initialPan, float phase = 0.0f)
    {
        if (waveform == null)
        {
            Debug.LogWarning("Trying to start a voice with a null waveform.");
            return;
        }
        Debug.Log("Starting voice with frequency: " + frequency + " and pan: " + initialPan);
        // Initialize or reset voice properties
        IsActive = true;
        Phase = phase;
        Frequency = frequency;
        timeSinceNoteOn = 0f;
        pan = initialPan;
        
        SetFrequency(frequency);
        SetPan(pan);
    }

    public void Stop(){
        // Instead of immediately deactivating the voice, let it play out the release phase
        // The voice will be marked as inactive after the release phase completes in the GenerateSample method.
        envelope.StartReleasePhase();
    }

    [BurstCompile]
    public (float leftSample, float rightSample) GenerateSample()
    {
        //if (!IsActive)
        //    return (0f, 0f);
        //Debug.Log("Generating sample for voice with frequency: " + Frequency + " and pan: " + pan);

        timeSinceNoteOn += timeDelta ;

        // Apply ADSR envelope
        float amplitude = envelope.GetAmplitude(timeSinceNoteOn);
        //Debug.Log("Amplitude: " + amplitude);
        if (!IsActive && amplitude <= AMPLITUDE_THRESHOLD){  // Some small threshold
            Debug.Log("Deactivating voice");
            // If voice is in the release phase and amplitude is near zero, deactivate the voice
            DeactivateVoice();
            return (0f, 0f);
        }

        // Apply amplitude modulation
        float modulatedAmplitude = amplitude * amplitudeModulator.GetModulationValue();
        //Debug.Log("Modulated amplitude: " + modulatedAmplitude);
        float modulatedFrequency = Frequency + frequencyModulator.GetModulationValue();
        Phase += modulatedFrequency * timeDelta;
        if (Phase > 1.0f) Phase -= 1.0f;  // Wrap phase if it exceeds 1

        float sampleValue = waveform.Evaluate(Phase);
        //Debug.Log("Sample value: " + sampleValue);
        
        // Apply filter
        sampleValue = filter.ApplyFilter(sampleValue);
        //Debug.Log("Sample value after filter: " + sampleValue);
        
        sampleValue *= modulatedAmplitude;
        //Debug.Log("Sample value after ModulatedAmplitude: " + sampleValue);
        
        return (sampleValue * leftAmp, sampleValue * rightAmp);
    }

    public void SetFrequency(float freq)
    {
        Frequency = freq;
        phaseIncrement = Frequency / sampleRate;
    }

    public void SetPan(float pan)
    {
        this.pan = pan;
        // Apply pan
        this.leftAmp = Mathf.Sqrt(0.5f * (1 - pan));
        this.rightAmp = Mathf.Sqrt(0.5f * (1 + pan));
    }

    public void DeactivateVoice(){
        IsActive = false;
        voicePoolManager.ReturnVoice(this);
    }

    public void SetWaveform(Waveform waveform)
    {
        if (waveform == null) {
            //Debug.LogWarning("Trying to set a null waveform in Voice class.");
            return;
        }
        this.waveform = waveform;
    }

    public void SetEnvelope(ADSREnvelope envelope)
    {
        this.envelope = envelope;
    }

    public void ReplaceFilter(FilterConfig newFilterConfig)
    {
        this.filter = newFilterConfig.CreateFilter();
    }

    public void UpdateFilter(FilterConfig newFilterConfig)
    {
        newFilterConfig.ApplyToExistingFilter(filter);
    }

    public void ReplaceAmplitudeModulator(ModulatorConfig newAmplitudeModulatorConfig)
    {
        this.amplitudeModulator = newAmplitudeModulatorConfig.CreateModulator();
        this.amplitudeModulator.SetWaveformRange(0.0f, 1.0f);
    }

    public void UpdateAmplitudeModulator(ModulatorConfig newModulatorConfig)
    {
        newModulatorConfig.ApplyToExistingModulator(this.amplitudeModulator);
    }

    public void ReplaceFrequencyModulator(ModulatorConfig newFrequencyModulatorConfig)
    {
        this.frequencyModulator = newFrequencyModulatorConfig.CreateModulator();
    }

    public void UpdateFrequencyModulator(ModulatorConfig newFrequencyModulatorConfig)
    {
        newFrequencyModulatorConfig.ApplyToExistingModulator(this.frequencyModulator);
    }


}
