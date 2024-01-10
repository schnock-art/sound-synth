using UnityEngine;

[CreateAssetMenu(menuName = "Waveforms/Triangle")]
public class TriangleWaveform : Waveform
{
    public override void InitializeTable(){
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            if (i < TABLE_SIZE / 2)
                table[i] = ((float)i / (TABLE_SIZE / 2)) * 2.0f - 1.0f;
            else
                table[i] = 2.0f - ((float)i / (TABLE_SIZE / 2)) * 2.0f;
        }
    }
}
