using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RatPrefabHolderBaker : Baker<RatPrefabHolderAuthoring>
{
    public override void Bake(RatPrefabHolderAuthoring authoring)
    {
        if (authoring.RatPrefab == null)
        {
            Debug.LogError("RatPrefabHolderAuthoring: назначьте RatPrefab в Inspector.", authoring);
            return;
        }


        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        Entity ratPrefabEntity = GetEntity(authoring.RatPrefab, TransformUsageFlags.Dynamic);

        AddComponent(entity, new RatPrefabHolder
        {
            RatPrefab = ratPrefabEntity
        });


        uint seed = (uint)UnityEngine.Random.Range(1, int.MaxValue);

        AddComponent(entity, new RatSpawnSettings
        {
            MinSpawnRadius = math.max(0f, authoring.MinSpawnRadius),
            MaxSpawnRadius = math.max(0f, authoring.MaxSpawnRadius),
            SpawnHeightOffset = authoring.SpawnHeightOffset,
            TargetPosition = new float3(
                authoring.TargetPosition.x,
                authoring.TargetPosition.y,
                authoring.TargetPosition.z),
            DelayBeforeFirstWave = math.max(0f, authoring.DelayBeforeFirstWave),
            DelayBetweenWaves = math.max(0f, authoring.DelayBetweenWaves),
            RandomSeed = seed
        });
    }
}
