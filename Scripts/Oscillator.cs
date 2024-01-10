using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(AudioSource))]
public class Oscillator : MonoBehaviour{

    public Waveform waveform; // Assign the waveform ScriptableObject in the inspector
    //public float frequency = 440.0f; // Default to A4 note

    [Range(0f, 1f)]
    //public float amplitude = 0.5f; // Default to half volume
    //private float increment;
    //private float phase;
    private AudioSource audioSource;

    public int sampleRate { get; private set; } // Standard CD quality sample rate

    public ADSREnvelope envelope;
    public FilterConfig filterConfig;
    public ModulatorConfig amplitudeModulatorConfig;
    public ModulatorConfig frequencyModulatorConfig;

    public Modulator panModulator;

    [Range(1,12)] // Max number of voices is 12
    public int numberOfVoicesperKey = 3;  // Number of voices
    [SerializeField] private float phaseOffset = 0f;  // Phase offset between voices
    public List<Voice> voices = new List<Voice>();

    private Dictionary<KeyCode, List<Voice>> activeVoiceMapping = new Dictionary<KeyCode, List<Voice>>();

    [SerializeField] private float detuneFactor = 0.1f;  // Detune factor between voices

    [SerializeField] private float frequencyDetuning = 10.0f;  // In Hertz. Determines how much the LFO affects the frequency.
    
    [SerializeField] 
    [Range(0f, 1f)] // Max paning is 1.0f
    private float panDetuning = 0.0f;  //Determines how much the pan is modifed fo the chorus effect.

    float leftVolume = 0.5f;
    float rightVolume = 0.5f;

    private VoicePoolManager voicePoolManager;

    private Type previousFilterConfigType;

    private Dictionary<KeyCode, float> frequencyMapping = new Dictionary<KeyCode, float>(){
        {KeyCode.A, 261.63f}, // C4
        {KeyCode.W, 277.18f}, // C#4
        {KeyCode.S, 293.66f}, // D4
        {KeyCode.E, 311.13f}, // D#4
        {KeyCode.D, 329.63f}, // E4
        {KeyCode.F, 349.23f}, // F4
        {KeyCode.T, 369.99f}, // F#4
        {KeyCode.G, 392.00f}, // G4
        {KeyCode.Y, 415.30f}, // G#4
        {KeyCode.H, 440.00f}, // A4
        {KeyCode.U, 466.16f}, // A#4
        {KeyCode.J, 493.88f}, // B4
    };
 
    void OnAudioFilterRead(float[] data, int channels){   
        if (voicePoolManager == null){
            for (int i = 0; i < data.Length; i++){
                data[i] = 0f;
            }
        }
        voices = voicePoolManager.activeVoices; 
        if (voices.Count == 0) { // If there are no voices, return immediately
            for (int i = 0; i < data.Length; i++){
                data[i] = 0f;
            }
        } else {
            for (int sample = 0; sample < data.Length; sample += channels){
                float leftValue = 0f;
                float rightValue = 0f;
                for (int i = 0; i < voices.Count; i++) {
                    Voice voice = voices[i];
                    var (leftSample, rightSample) = voice.GenerateSample();
                    leftValue += leftSample;
                    rightValue += rightSample;
                }
                leftValue /= voices.Count;  // Average the values, if desired
                rightValue /= voices.Count;  // Average the values, if desired

                if (panModulator != null){
                    float panValue = panModulator.GetModulationValue(); // This should be between -1 and 1
                    leftVolume = 0.5f * (1 - panValue);
                    rightVolume = 0.5f * (1 + panValue);
                } else {
                    leftVolume = 0.5f;
                    rightVolume = 0.5f;
                }
                data[sample] = Mathf.Clamp(leftValue * leftVolume, 0f, 1f); // Left channel
                data[sample + 1] = Mathf.Clamp(rightValue * rightVolume, 0f, 1f); // Right channel
            }
            voices.RemoveAll(voice => !voice.IsActive);
        }
    }

    void Awake(){
        sampleRate = AudioSettings.outputSampleRate;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = true;
        audioSource.spatialBlend = 0;  // 0.0 means 2D audio

        if (envelope == null){
            envelope = ScriptableObject.CreateInstance<ADSREnvelope>();
        };
        if (filterConfig == null){
            filterConfig = ScriptableObject.CreateInstance<LowPassFilterConfig>();
        }
        if (amplitudeModulatorConfig == null){
            amplitudeModulatorConfig = ScriptableObject.CreateInstance<LFOModulatorConfig>();
        }
        if (frequencyModulatorConfig == null){
            frequencyModulatorConfig = ScriptableObject.CreateInstance<LFOModulatorConfig>();
        }
        if (waveform == null){
            waveform = ScriptableObject.CreateInstance<SineWaveform>();
        }
        if (panModulator == null){
            panModulator = new LFOModulator(
                null,
                0.5f,
                0.5f,
                sampleRate
            );
        }

        filterConfig.SetSampleRate(sampleRate);
        amplitudeModulatorConfig.SetSampleRate(sampleRate);
        frequencyModulatorConfig.SetSampleRate(sampleRate);
        voicePoolManager = this.GetComponentInChildren<VoicePoolManager>();
    }

    void Start(){
        voicePoolManager.InitializeVoicePool();
        voicePoolManager.UpdateVoicesFilter(filterConfig);
        voicePoolManager.UpdateVoicesAmplitudeModulator(amplitudeModulatorConfig);
        voicePoolManager.UpdateVoicesFrequencyModulator(frequencyModulatorConfig);
        //voicePoolManager.UpdateVoicesPanModulator(panModulator);
    }

    public void Play(){
        audioSource.Play();
    }

    public void Stop(){
        audioSource.Stop();
    }

    public void Update(){
        if (!audioSource.isPlaying) return;
        foreach (var key in frequencyMapping.Keys){
            if (Input.GetKeyDown(key)){
                Debug.Log("Key pressed: " + key);
                float frequency = frequencyMapping[key];
                List<Voice> voicesForKey = new List<Voice>();
                // Now use this frequency to start a voice or any other action
                for (int i = 0; i < numberOfVoicesperKey; i++) {
                     // Calculate detune: alternate the sign with every iteration
                    float detuneSign = (i % 2 == 0) ? 1 : -1;
                    float detune = detuneSign * ((i+1)*detuneFactor);
                    //float detune = (numberOfVoicesperKey - (i**2 - 1))* frequencyDetuneFactor;
                    float randomDetune = UnityEngine.Random.Range(-frequencyDetuning, frequencyDetuning);
                    float detunedFrequency = frequency + randomDetune + detune;
                    float pan = 0.0f;
                    if (panDetuning > 0f){
                        pan = (i - (numberOfVoicesperKey - 1) / 2.0f) * panDetuning;
                    }
                    float phase = 0.0f;
                    if (phaseOffset > 0f){
                        phase = (i - (numberOfVoicesperKey - 1) / 2.0f) * phaseOffset;
                    }
                    Voice newVoice = voicePoolManager.GetAvailableVoice();
                    if (newVoice != null){
                        newVoice.Start(detunedFrequency, pan, phase);
                        voices.Add(newVoice);
                    }
                }
                // Store the voices for this key
                activeVoiceMapping[key] = voicesForKey;
            } // Handle key release
            else if (Input.GetKeyUp(key)){
                if (activeVoiceMapping.TryGetValue(key, out var voicesForKey)){
                    foreach (var voice in voicesForKey){
                        voice.Stop();  // Call the Stop method on each voice
                    }
                    activeVoiceMapping.Remove(key);  // Optionally remove the key from the dictionary
                }
            }
        }
    }

    public void OnFilterConfigChanged(){
        voicePoolManager.UpdateVoicesFilter(filterConfig);
    }

    public void OnAmplitudeModulatorConfigChanged(){
        voicePoolManager.UpdateVoicesAmplitudeModulator(amplitudeModulatorConfig);
    }

    public void OnFrequencyModulatorConfigChanged(){
        voicePoolManager.UpdateVoicesFrequencyModulator(frequencyModulatorConfig);
    }
}

