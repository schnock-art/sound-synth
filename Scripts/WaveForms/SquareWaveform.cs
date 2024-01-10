using UnityEngine;

[CreateAssetMenu(menuName = "Waveforms/Square")]
public class SquareWaveform : Waveform
{
    public override void InitializeTable(){
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            table[i] = i < TABLE_SIZE / 2 ? -1.0f : 1.0f;
        }
    }
}
