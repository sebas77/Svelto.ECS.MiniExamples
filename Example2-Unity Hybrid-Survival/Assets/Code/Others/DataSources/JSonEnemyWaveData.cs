using System;
using UnityEngine;

namespace Svelto.ECS.Example.Survive
{
    [Serializable]
    public class JSonEnemyWaveData
    {
        public EnemyWaveData enemyWaveData;

        public JSonEnemyWaveData(EnemyWaveData waveData) { enemyWaveData = waveData; }
    }

    [Serializable]
    public class EnemyWaveData
    {
        public Vector3 amountToSpawn;
        public int waveNum;
    }
}
