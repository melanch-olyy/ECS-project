using Unity.Entities;
using Unity.Mathematics;

public struct RatSpawnSettings : IComponentData
{
    public float MinSpawnRadius;
    public float MaxSpawnRadius;
    public float SpawnHeightOffset;

    public float3 TargetPosition;

    public float DelayBeforeFirstWave;
    public float DelayBetweenWaves;
    public uint RandomSeed;
}
