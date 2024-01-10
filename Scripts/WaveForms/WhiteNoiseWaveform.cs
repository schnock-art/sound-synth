using UnityEngine;

[CreateAssetMenu(menuName = "Waveforms/WhiteNoise")]
public class WhiteNoiseWaveform : Waveform
{
    public override void InitializeTable()
    {
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            table[i] = Random.Range(-1.0f, 1.0f);
        }
    }
}
