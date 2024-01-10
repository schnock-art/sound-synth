using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VoicePoolManager : MonoBehaviour{

    private Oscillator parentOscillator;
    private List<Voice> voicePool = new List<Voice>();

    public List<Voice> activeVoices { get; private set; } = new List<Voice>();
    private int maxVoices = 32;  // Arbitrary number, adjust based on your needs
    public Waveform waveform { get; private set; }
    public ADSREnvelope envelope { get; private set; }

    public FilterConfig filterConfig { get; private set; }
    public ModulatorConfig amplitudeModulatorConfig { get; private set; }
    public ModulatorConfig frequencyModulatorConfig { get; private set; }

    public int sampleRate { get; private set; }

    private void Awake()
    {
        parentOscillator = GetComponentInParent<Oscillator>();
        if (parentOscillator == null)
        {
            Debug.LogError("VoicePoolManager requires a parent Oscillator.");
            return;
        }

        InitializeVoicePool();
    }

    public void InitializeVoicePool()
    {
        int sampleRate = parentOscillator.sampleRate;
        Waveform waveform = parentOscillator.waveform;
        waveform.Initialize();
        ADSREnvelope envelope = parentOscillator.envelope;
        ModulatorConfig amplitudeModulatorConfig = parentOscillator.amplitudeModulatorConfig;
        ModulatorConfig frequencyModulatorConfig = parentOscillator.frequencyModulatorConfig;
        FilterConfig filterConfig = parentOscillator.filterConfig;
        for (int i = 0; i < maxVoices; i++)
        {
            voicePool.Add(new Voice(
                sampleRate,
                waveform, 
                envelope, 
                amplitudeModulatorConfig,
                frequencyModulatorConfig,
                filterConfig,
                this
                ));
        }
        UpdateVoicesFilter(filterConfig);
        UpdateVoicesAmplitudeModulator(amplitudeModulatorConfig);
        UpdateVoicesFrequencyModulator(frequencyModulatorConfig);

    }

    public Voice GetAvailableVoice()
    {
        foreach (var voice in voicePool)
        {
            if (!voice.IsActive)
            {
                activeVoices.Add(voice);
                return voice;
            }
        }
        return null;  // All voices are active, consider strategies for handling this
    }

    public void ReturnVoice(Voice voice){
        activeVoices.Remove(voice);
    }

    public void UpdateVoicesFilter(FilterConfig newFilterConfig)
    {
        if (newFilterConfig == null)
        {
            Debug.LogError("FilterConfig is null.");
            return;
        }

        if (filterConfig == null)
        {
            Debug.LogWarning    ("FilterConfig is null.");
            filterConfig = newFilterConfig;
        }

        Type previousFilterConfigType = filterConfig.GetType();
        
        filterConfig = newFilterConfig;

        Type currentFilterConfigType = filterConfig.GetType();
        
        
        if (previousFilterConfigType != currentFilterConfigType){
            // The filter type has changed
            foreach (Voice voice in voicePool)
            {
                voice.ReplaceFilter(filterConfig);
            }
            //spreviousFilterConfigType = currentFilterConfigType;
        }
        else {
            // Just update the existing filter's parameters
            foreach (Voice voice in voicePool)
            {
                voice.UpdateFilter(filterConfig);
            }
        }
    }

    public void UpdateVoicesAmplitudeModulator(ModulatorConfig newModulatorConfig){
        if (newModulatorConfig == null)
        {
            Debug.LogError("ModulatorConfig is null.");
            return;
        }

        if (amplitudeModulatorConfig == null)
        {
            Debug.LogWarning("AmplitudeModulatorConfig is null.");
            amplitudeModulatorConfig = newModulatorConfig;
        }
        
        Type previousModulatorConfigType = amplitudeModulatorConfig.GetType();

        amplitudeModulatorConfig = newModulatorConfig;

        Type currentodulatorConfigType = amplitudeModulatorConfig.GetType();
        
        if (previousModulatorConfigType != currentodulatorConfigType){
            // The filter type has changed
            foreach (Voice voice in voicePool) {
                voice.ReplaceAmplitudeModulator(amplitudeModulatorConfig);
            }
        } else {
            // Just update the existing filter's parameters
            foreach (Voice voice in voicePool) {
                voice.UpdateAmplitudeModulator(amplitudeModulatorConfig);
            }
        }
    }

    public void UpdateVoicesFrequencyModulator(ModulatorConfig newFrequencyModulatorConfig)
    {
        if (newFrequencyModulatorConfig == null)
        {
            Debug.LogError("ModulatorConfig is null.");
            return;
        }

        if (frequencyModulatorConfig == null)
        {
            Debug.LogWarning("FrequencyModulatorConfig is null.");
            frequencyModulatorConfig = newFrequencyModulatorConfig;
        }

        Type previousModulatorConfigType = frequencyModulatorConfig.GetType();

        frequencyModulatorConfig = newFrequencyModulatorConfig;

        Type currentModulatorConfigType = frequencyModulatorConfig.GetType();
        
        if (previousModulatorConfigType != currentModulatorConfigType){
            // The filter type has changed
            foreach (Voice voice in voicePool) {
                voice.ReplaceFrequencyModulator(frequencyModulatorConfig);
            }
        } else {
            // Just update the existing filter's parameters
            foreach (Voice voice in voicePool) {
                voice.UpdateFrequencyModulator(frequencyModulatorConfig);
            }
        }
    }
}
