using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class WaveSpawnerSystem : SystemBase
{
    private int _currentWaveIndex;
    private float _timer;
    private int _enemiesSpawnedInWave;
    private bool _waveInProgress;
    private bool _allWavesCompleted;

    private Random _random;
    private bool _randomInitialized;

    protected override void OnUpdate()
    {
        // Если все волны завершены — выходим
        if (_allWavesCompleted)
            return;

        // Получаем синглтоны
        if (!SystemAPI.TryGetSingleton<RatPrefabHolder>(out RatPrefabHolder holder))
            return;

        if (!SystemAPI.TryGetSingleton<WavesBlobReference>(out WavesBlobReference wavesRef))
            return;

        if (!wavesRef.Waves.IsCreated)
            return;

        ref WavesBlob wavesBlob = ref wavesRef.Waves.Value;

        // Проверяем, не закончились ли волны
        if (_currentWaveIndex >= wavesBlob.Waves.Length)
        {
            _allWavesCompleted = true;
            return;
        }

        // Инициализируем Random один раз
        if (!_randomInitialized)
        {
            _random = new Random(42); // Фиксированный сид для простоты
            _randomInitialized = true;
        }

        float deltaTime = SystemAPI.Time.DeltaTime;
        Wave currentWave = wavesBlob.Waves[_currentWaveIndex];

        // Ждём 2 секунды перед началом волны
        if (!_waveInProgress)
        {
            _timer += deltaTime;
            if (_timer < 2f)
                return;

            // Начинаем волну
            _waveInProgress = true;
            _timer = 0f;
            _enemiesSpawnedInWave = 0;
        }
        else
        {
            // Ждём задержку между спавном крыс
            _timer += deltaTime;
            if (_timer < currentWave.SpawnDelay)
                return;

            // Спавним крысу
            if (_enemiesSpawnedInWave < currentWave.EnemyCount)
            {
                var ecb = new EntityCommandBuffer(Allocator.Temp);

                // Создаём копию префаба
                Entity newRat = ecb.Instantiate(holder.RatPrefab);

                // Спавним в позиции SpawnerManager (без сложной математики)
                LocalTransform managerTransform = EntityManager.GetComponentData<LocalTransform>(
                    SystemAPI.GetSingletonEntity<RatPrefabHolder>()
                );

                LocalTransform ratTransform = new LocalTransform
                {
                    Position = managerTransform.Position,
                    Rotation = quaternion.identity,
                    Scale = 1f
                };

                // Берём настройки движения из префаба
                RatMovement movement = EntityManager.GetComponentData<RatMovement>(holder.RatPrefab);

                // Random используем для скорости крысы в диапазоне волны
                float speedMin = math.min(currentWave.SpeedMin, currentWave.SpeedMax);
                float speedMax = math.max(currentWave.SpeedMin, currentWave.SpeedMax);
                movement.MoveSpeed = _random.NextFloat(speedMin, speedMax);

                // Устанавливаем цель (позицию игрока)
                movement.TargetPosition = new float3(10f, 0f, 10f);

                // Применяем изменения
                ecb.SetComponent(newRat, ratTransform);
                ecb.SetComponent(newRat, movement);
                ecb.Playback(EntityManager);
                ecb.Dispose();

                _enemiesSpawnedInWave++;
                _timer = 0f; // Сбрасываем таймер для следующей крысы
            }

            // Проверяем, закончилась ли волна
            if (_enemiesSpawnedInWave >= currentWave.EnemyCount)
            {
                _waveInProgress = false;
                _currentWaveIndex++;
                _timer = 0f;
            }
        }
    }
}
