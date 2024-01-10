using UnityEngine;

[CreateAssetMenu(menuName = "Waveforms/Sawtooth")]
public class SawtoothWaveform : Waveform
{
    public override void InitializeTable(){
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            table[i] = ((float)i / TABLE_SIZE) * 2.0f - 1.0f;
        }
    }
}
