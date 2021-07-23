using System.IO;
using Svelto;
using Svelto.ECS.Example.Survive;
using UnityEngine;

public class SpawningData : MonoBehaviour
{
    static bool serializedSpawnDataOnce;
    static bool serializedAttackDataOnce;
    static bool serialisedAmmoBoxDataOnce;

    void Awake() { Init(); }

    public void SerializeSpawnData()
    {
        serializedSpawnDataOnce = true;

        var data         = GetComponents<EnemyData>();
        var spawningdata = new JSonEnemySpawnData[data.Length];

        for (var i = 0; i < data.Length; i++)
            spawningdata[i] = new JSonEnemySpawnData(data[i].spawnData);

        var json = JsonHelper.arrayToJson(spawningdata);

        Console.Log(json);

        File.WriteAllText("EnemySpawningData.json", json);
    }

    public void SerializeAttackData()
    {
        var data       = GetComponents<EnemyData>();
        var attackData = new JSonEnemyAttackData[data.Length];

        serializedAttackDataOnce = true;

        for (var i = 0; i < data.Length; i++)
            attackData[i] = new JSonEnemyAttackData(data[i].attackData);

        var json = JsonHelper.arrayToJson(attackData);

        Console.Log(json);

        File.WriteAllText("EnemyAttackData.json", json);
    }

    public void SerializeAmmoBoxSpawnData()
    {
        serialisedAmmoBoxDataOnce = true;

        var data        = GetComponents<AmmoBoxData>();
        var spawnData   = new JSonAmmoBoxSpawnData[data.Length];

        for (var i = 0; i < data.Length; i++)
            spawnData[i] = new JSonAmmoBoxSpawnData(data[i].ammoBoxSpawnData);

        var json = JsonHelper.arrayToJson(spawnData);

        Console.Log(json);

        File.WriteAllText("AmmoBoxSpanwingData.json", json);
    }

    public void Init()
    {
        if (serializedSpawnDataOnce == false)
            SerializeSpawnData();

        if (serializedAttackDataOnce == false)
            SerializeAttackData();

        if (serialisedAmmoBoxDataOnce == false)
            SerializeAmmoBoxSpawnData();
    }
}