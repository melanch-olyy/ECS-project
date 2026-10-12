using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class WavesBaker : Baker<WavesAuthoring>
{
    public override void Bake(WavesAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        var builder = new BlobBuilder(Allocator.Temp);

        ref WavesBlob root = ref builder.ConstructRoot<WavesBlob>();

        int waveCount = authoring.Waves == null ? 0 : authoring.Waves.Count;
        BlobBuilderArray<Wave> wavesArray = builder.Allocate(ref root.Waves, waveCount);

        for (int i = 0; i < waveCount; i++)
        {
            WaveData source = authoring.Waves[i];
            float speedMin = math.max(0f, math.min(source.SpeedMin, source.SpeedMax));
            float speedMax = math.max(0f, math.max(source.SpeedMin, source.SpeedMax));

            wavesArray[i] = new Wave
            {
                EnemyCount = math.max(0, source.EnemyCount),
                SpawnDelay = math.max(0f, source.SpawnDelay),
                SpeedMin = speedMin,
                SpeedMax = speedMax
            };
        }

        BlobAssetReference<WavesBlob> blobReference = builder.CreateBlobAssetReference<WavesBlob>(Allocator.Persistent);

        builder.Dispose();

        
        AddBlobAsset(ref blobReference, out var hash);

        AddComponent(entity, new WavesBlobReference
        {
            Waves = blobReference
        });
    }
}
