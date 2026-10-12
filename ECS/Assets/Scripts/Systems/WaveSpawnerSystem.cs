using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class WaveSpawnerSystem : SystemBase
{
    private int _currentWaveIndex;
    private float _waveTimer;
    private float _spawnTimer;
    private int _enemiesSpawnedInWave;
    private bool _waveInProgress;

    private Random _random;
    private bool _randomInitialized;

    protected override void OnUpdate()
    {
        if (!SystemAPI.TryGetSingleton<RatPrefabHolder>(out RatPrefabHolder holder))
            return;

        if (!SystemAPI.TryGetSingleton<RatSpawnSettings>(out RatSpawnSettings settings))
            return;

        if (!SystemAPI.TryGetSingleton<WavesBlobReference>(out WavesBlobReference wavesReference))
            return;

        if (!wavesReference.Waves.IsCreated)
            return;

        ref WavesBlob wavesBlob = ref wavesReference.Waves.Value;
        if (_currentWaveIndex >= wavesBlob.Waves.Length)
            return; 

        if (!_randomInitialized)
        {
            uint seed = settings.RandomSeed == 0 ? 1u : settings.RandomSeed;
            _random = new Random(seed);
            _randomInitialized = true;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;
        Wave currentWave = wavesBlob.Waves[_currentWaveIndex];

        if (!_waveInProgress)
        {
            float requiredDelay = _currentWaveIndex == 0
                ? settings.DelayBeforeFirstWave
                : settings.DelayBetweenWaves;

            _waveTimer += deltaTime;
            if (_waveTimer < requiredDelay)
                return;

            _waveTimer = 0f;
            _spawnTimer = 0f;
            _enemiesSpawnedInWave = 0;
            _waveInProgress = true;
        }

        if (currentWave.EnemyCount <= 0)
        {
            FinishCurrentWave();
            return;
        }

        //spawnDelay промежуток между появлениями крыс внутри волны
        _spawnTimer += deltaTime;
        if (_spawnTimer < currentWave.SpawnDelay)
            return;

        Entity holderEntity = SystemAPI.GetSingletonEntity<RatPrefabHolder>();

        
        LocalTransform managerTransform = EntityManager.GetComponentData<LocalTransform>(holderEntity);
        float3 spawnCenter = managerTransform.Position;

        float minRadius = math.min(settings.MinSpawnRadius, settings.MaxSpawnRadius);
        float maxRadius = math.max(settings.MinSpawnRadius, settings.MaxSpawnRadius);

        //cлучайная точка в вокруг Spawnermanager
        float angle = _random.NextFloat(0f, 2f * math.PI);
        float radius;
        if (maxRadius <= minRadius)
        {
            radius = minRadius;
        }
        else
        {
            float radiusSquared = _random.NextFloat(minRadius * minRadius, maxRadius * maxRadius);
            radius = math.sqrt(radiusSquared);
        }
        float3 offset = new float3(math.cos(angle) * radius, 0f, math.sin(angle) * radius);
        float3 spawnPosition = spawnCenter + offset;
        spawnPosition.y = spawnCenter.y + settings.SpawnHeightOffset;


        LocalTransform ratTransform = EntityManager.GetComponentData<LocalTransform>(holder.RatPrefab);
        ratTransform.Position = spawnPosition;

  
        RatMovement movement = EntityManager.GetComponentData<RatMovement>(holder.RatPrefab);
        float speedMin = math.min(currentWave.SpeedMin, currentWave.SpeedMax);
        float speedMax = math.max(currentWave.SpeedMin, currentWave.SpeedMax);
        movement.MoveSpeed = speedMax <= speedMin
            ? speedMin
            : _random.NextFloat(speedMin, speedMax);
        movement.TargetPosition = settings.TargetPosition;


        var ecb = new EntityCommandBuffer(Allocator.Temp);
        Entity newRat = ecb.Instantiate(holder.RatPrefab);
        ecb.SetComponent(newRat, ratTransform);
        ecb.SetComponent(newRat, movement);
        ecb.Playback(EntityManager);
        ecb.Dispose();

        _spawnTimer = 0f;
        _enemiesSpawnedInWave++;

        if (_enemiesSpawnedInWave >= currentWave.EnemyCount)
            FinishCurrentWave();
    }

    private void FinishCurrentWave()
    {
        _currentWaveIndex++;
        _waveInProgress = false;
        _waveTimer = 0f;
        _spawnTimer = 0f;
        _enemiesSpawnedInWave = 0;
    }
}
