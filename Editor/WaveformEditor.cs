using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waveform), true)] // The 'true' allows it to target derived types
[CanEditMultipleObjects]
public class WaveformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();
        //EditorGUILayout.PropertyField();
        
        // Get a reference to the Oscillator script
        Waveform waveform = (Waveform)target;

        // Add a button that calls the Play method of the Oscillator script
        if (GUILayout.Button("Draw Waveform")){
            waveform.Initialize();
        }
    }
}
