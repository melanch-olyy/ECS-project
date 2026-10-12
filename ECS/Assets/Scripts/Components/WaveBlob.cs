using Unity.Entities;

public struct Wave
{
    public int EnemyCount;
    public float SpawnDelay;
    public float SpeedMin;
    public float SpeedMax;
}

public struct WavesBlob
{
    public BlobArray<Wave> Waves;
}

public struct WavesBlobReference : IComponentData
{
    public BlobAssetReference<WavesBlob> Waves;
}
