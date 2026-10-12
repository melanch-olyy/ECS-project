using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WaveData
{
    public int EnemyCount = 5;
    public float SpawnDelay = 0.5f;
    public float SpeedMin = 2f;
    public float SpeedMax = 4f;
}

public class WavesAuthoring : MonoBehaviour
{
    public List<WaveData> Waves = new List<WaveData>();
}
