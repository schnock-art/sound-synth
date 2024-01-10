using UnityEngine;
using Unity.Collections;
using Unity.Burst;

public abstract class Waveform : ScriptableObject
{
    protected const int TABLE_SIZE = 2048;
    protected NativeArray<float> table;

    private float minValue = -1.0f;
    private float maxValue = 1.0f;   

    public AnimationCurve curve;

    [BurstCompile]
    public float Evaluate(float phase){
        int index = (int)(phase * TABLE_SIZE) % TABLE_SIZE;
        index = Mathf.Clamp(index, 0, TABLE_SIZE - 1);
        return table[index];
    }

    public void DrawCurve(){
        curve = new AnimationCurve();
        for (int i = 0; i < TABLE_SIZE; i++)
        {
            curve.AddKey((float)i / TABLE_SIZE, table[i]);
        }
    }

    public abstract void InitializeTable();

    public void Initialize(){
        if (table.IsCreated){
            table.Dispose();
        }
        table = new NativeArray<float>(TABLE_SIZE, Allocator.Persistent);
        InitializeTable();
        MapTableToRange();
        DrawCurve();
    }
    public virtual void Dispose()
    {
        if (table.IsCreated)
            table.Dispose();
    }

    public float GetValueAt(int index){
        return table[index];
    }

    public void SetTableRange(float minValue, float maxValue){
        this.minValue = minValue;
        this.maxValue = maxValue;
        MapTableToRange();
    }

    public float MapToRange(float x, float minValue, float maxValue) {
        return minValue + (x + 1) * (maxValue - minValue) / 2;
    }

    public void MapTableToRange(){
        for (int i = 0; i < TABLE_SIZE; i++){
            table[i] = MapToRange(table[i], minValue, maxValue);
        }
    }

    private void OnDestroy()
    {
        Dispose();
    }

    private void OnDisable()
    {
        Dispose();
    }

}
