using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;


[CreateAssetMenu(menuName = "Waveforms/Combined")]
public class CombinedWaveform : Waveform{
    [System.Serializable]
    public struct WeightedWaveform
    {
        public Waveform waveform;
        public float weight;
    }

    public List<WeightedWaveform> waveforms;

    public override void InitializeTable()
    {
        if (table.IsCreated)
            table.Dispose();

        table = new NativeArray<float>(TABLE_SIZE, Allocator.Persistent);

        foreach (var weightedWave in waveforms)
        {
            weightedWave.waveform.Initialize();
        }

        float maxValue = 0f;

        for (int i = 0; i < TABLE_SIZE; i++)
        {
            float combinedValue = 0f;
            float totalWeight = 0f;
            

            foreach (var weightedWave in waveforms)
            {
                totalWeight += weightedWave.weight;
                combinedValue += weightedWave.weight * weightedWave.waveform.GetValueAt(i);
            }
            combinedValue /= totalWeight;
            table[i] = combinedValue;

            if (Mathf.Abs(combinedValue) > maxValue)
                maxValue = Mathf.Abs(combinedValue);
        }

        for (int i = 0; i < TABLE_SIZE; i++)
        {
            table[i] /= maxValue;
        }
    }
}
