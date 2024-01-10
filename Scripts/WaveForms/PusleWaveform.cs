using UnityEngine;

[CreateAssetMenu(menuName = "Waveforms/Pulse")]
public class PulseWaveform : Waveform
{
    [Range(0.1f, 0.9f)]
    public float dutyCycle = 0.5f;  // 50% by default

    public override void InitializeTable()
    {
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            table[i] = (float)i / TABLE_SIZE < dutyCycle ? 1.0f : -1.0f;
        }
    }
}
