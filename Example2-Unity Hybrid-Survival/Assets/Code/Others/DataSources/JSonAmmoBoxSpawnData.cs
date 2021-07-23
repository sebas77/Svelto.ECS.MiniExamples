using System;
using Svelto.ECS.Example.Survive.Player;
using UnityEngine;

namespace Svelto.ECS.Example.Survive
{
    [Serializable]
    public class JSonAmmoBoxSpawnData
    {
        public AmmoBoxSpawnData ammoBoxSpawnData;

        public JSonAmmoBoxSpawnData (AmmoBoxSpawnData spawnData) { ammoBoxSpawnData = spawnData; }
    }

    [Serializable]
    public class AmmoBoxSpawnData
    {
        public string AmmoBoxPrefab;
        public Vector3[] spawnPoints; 
        public float spawnTime;
        public int ammoReturn = 25;
        public PlayerTargetType playerTargetTag;
    }
}
