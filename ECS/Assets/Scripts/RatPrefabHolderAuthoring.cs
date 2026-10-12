using UnityEngine;

public class RatPrefabHolderAuthoring : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject RatPrefab;

    [Header("Spawn area")]
    [Min(0f)] public float MinSpawnRadius = 2f;
    [Min(0f)] public float MaxSpawnRadius = 4f;
    [Tooltip("Смещение по Y относительно высоты Spawnermanager. Для плоскости на Y=0 обычно начните с 0.05.")]
    public float SpawnHeightOffset = 0.05f;

    [Header("Rat target (world coordinates)")]
    public Vector3 TargetPosition = new Vector3(10f, 0f, 10f);

    [Header("Wave timing")]
    [Min(0f)] public float DelayBeforeFirstWave = 3f;
    [Min(0f)] public float DelayBetweenWaves = 4f;
}
