using UnityEngine;

// Placeholder for Pink Noise
[CreateAssetMenu(menuName = "Waveforms/PinkNoise")]
public class PinkNoiseWaveform : Waveform
{
    public override void InitializeTable()
    {
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            table[i] = Random.Range(-1.0f, 1.0f);
        }
    }
}
