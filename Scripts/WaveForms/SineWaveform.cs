using UnityEngine;

[CreateAssetMenu(menuName = "Waveforms/Sine")]
public class SineWaveform : Waveform{
    public override void InitializeTable(){
        for (int i = 0; i < TABLE_SIZE; i++){
            table[i] = Mathf.Sin(2 * Mathf.PI * i / TABLE_SIZE);
        }
    }
}