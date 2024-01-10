using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

[CreateAssetMenu(menuName = "Envelopes/ADSREnvelope")]
public class ADSREnvelope : ScriptableObject{
    public float attackTime = 0.1f;
    public float decayTime = 0.1f;
    public float sustainLevel = 0.8f;
    public float releaseTime = 0.1f;

    private bool releaseStarted = false;

    private float timeSinceReleaseStarted = 0f;

    [BurstCompile]
    public float GetAmplitude(float timeSinceNoteOn){
        if (timeSinceNoteOn <= attackTime){
            return Mathf.Lerp(0f, 1f, timeSinceNoteOn / attackTime);
        }
        else if (timeSinceNoteOn <= attackTime + decayTime){
            return Mathf.Lerp(1f, sustainLevel, (timeSinceNoteOn - attackTime) / decayTime);
        }
        else if (!releaseStarted){
            return sustainLevel;
        }
        else {
            timeSinceReleaseStarted += Time.deltaTime;
            return Mathf.Lerp(sustainLevel, 0f, timeSinceReleaseStarted/ releaseTime);
        }
        return 0f;
    }

    public void StartReleasePhase(){
        releaseStarted = true;
        timeSinceReleaseStarted = 0f;
    }
}

