using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Oscillator))]
public class OscillatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();
        //EditorGUILayout.PropertyField();
        
        // Get a reference to the Oscillator script
        Oscillator oscillator = (Oscillator)target;

        // Add a button that calls the Play method of the Oscillator script
        if (GUILayout.Button("Play Oscillator")){
            oscillator.Play();
        }

        if (GUILayout.Button("Stop Oscillator")){
            oscillator.Stop();
        }
    }
}
