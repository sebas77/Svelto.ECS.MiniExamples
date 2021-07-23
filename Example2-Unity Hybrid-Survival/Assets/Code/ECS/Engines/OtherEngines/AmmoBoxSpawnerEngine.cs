using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Svelto.ECS.Extensions.Unity;

namespace Svelto.ECS.Example.Survive.AmmoBox
{
    public class AmmoBoxSpawnerEngine : IQueryingEntitiesEngine, IReactOnSwap<AmmoBoxEntityViewComponent>, IStepEngine
    {
        // Start is called before the first frame update
        const int SECONDS_BETWEEN_SPAWNS = 10;
        const int NUMBER_OF_BOXES_TO_SPAWN = 1;

        public AmmoBoxSpawnerEngine(AmmoBoxFactory ammoBoxFactory, IEntityFunctions entityFunctions)
        {
            _entityFunctions = entityFunctions;
            _AmmoBoxFactory = ammoBoxFactory;
            _numberOfAmmoBoxToSpawn = NUMBER_OF_BOXES_TO_SPAWN;
        }

        public EntitiesDB entitiesDB { private get; set; }
        public void Ready() { _intervaledTick = IntervaledTick(); }
        public void Step() { _intervaledTick.MoveNext(); }
        public string name => nameof(AmmoBoxSpawnerEngine);

        public void MovedTo(ref AmmoBoxEntityViewComponent entityComponent, ExclusiveGroupStruct previousGroup, EGID egid)
        {
            if (egid.groupID.FoundIn(AmmoBoxUsed.Groups))
            {
                _numberOfAmmoBoxToSpawn++;
            }
        }

        IEnumerator IntervaledTick()
        {
            IEnumerator<JSonAmmoBoxSpawnData[]> ammoBoxtoSpawnJsons = ReadAmmoBoxSpawningDataServiceRequest();

            while (ammoBoxtoSpawnJsons.MoveNext())
                yield return null;

            var ammoBoxToSpawn = ammoBoxtoSpawnJsons.Current;

            var spawnTimes = new float[ammoBoxToSpawn.Length];

            while (true)
            {
                var waitForSecondsEnumerator = new WaitForSecondsEnumerator(SECONDS_BETWEEN_SPAWNS);

                while (waitForSecondsEnumerator.MoveNext())
                    yield return null;

                if (_numberOfAmmoBoxToSpawn > 0)
                {
                    if (spawnTimes[0] <= 0.0f)
                    {
                        var spawnData = ammoBoxToSpawn[0];

                        //var fromGroupId = ECSGroups.AmmoBoxToRcycleGroups + (uint)spawnData.ammoBoxSpawnData.playerTargetTag;

                        //if (entitiesDB.HasAny<AmmoBoxEntityViewComponent>(fromGroupId))
                        //    ReuseAmmoBox(fromGroupId, spawnData);
                        //else
                        //{
                           var build = _AmmoBoxFactory.Build(spawnData.ammoBoxSpawnData);
                            while (build.MoveNext())
                                yield return null;
                        //}
                        spawnTimes[0] = spawnData.ammoBoxSpawnData.spawnTime;
                        _numberOfAmmoBoxToSpawn--;
                    }
                    spawnTimes[0] -= SECONDS_BETWEEN_SPAWNS;
                }

                yield return null;
            }
        }
        void ReuseAmmoBox(ExclusiveGroupStruct fromGroupId, JSonAmmoBoxSpawnData spawnData)
        {
            

            var (ammoReturn, AmmoBoxViews, count) =
                entitiesDB.QueryEntities<AmmoBoxAttributeComponent, AmmoBoxEntityViewComponent>(fromGroupId);

            if (count > 0)
            {
                ammoReturn[0].returnAmmo = spawnData.ammoBoxSpawnData.ammoReturn;
                ammoReturn[0].playertargetType = spawnData.ammoBoxSpawnData.playerTargetTag;

                               
                var spawnPoint = spawnData.ammoBoxSpawnData.spawnPoints[Random.Range(0, spawnData.ammoBoxSpawnData.spawnPoints.Length - 1)];

                AmmoBoxViews[0].positionComponent.position = new Vector3(spawnPoint.x, 0.7f, spawnPoint.z);

                Svelto.Console.LogDebug("AmmoBox Collider Value " + AmmoBoxViews[0].targetTriggerComponent.hitChange.value.collides);

                _entityFunctions.SwapEntityGroup<AmmoBoxEntityDescriptor>(AmmoBoxViews[0].ID, AmmoBoxAvailable.BuildGroup);
                
                Svelto.Console.LogDebug("reuse AmmoBox " + spawnData.ammoBoxSpawnData.AmmoBoxPrefab);
            }
        }

        static IEnumerator<JSonAmmoBoxSpawnData[]> ReadAmmoBoxSpawningDataServiceRequest()
        {
            var json = Addressables.LoadAssetAsync<TextAsset>("AmmoBoxSpanwingData");

            while (json.IsDone == false)
                yield return null;

            var ammoBoxtoSpawn = JsonHelper.getJsonArray<JSonAmmoBoxSpawnData>(json.Result.text);

            yield return ammoBoxtoSpawn;
        }

        readonly AmmoBoxFactory _AmmoBoxFactory;
        readonly IEntityFunctions _entityFunctions;

        int _numberOfAmmoBoxToSpawn;
        IEnumerator _intervaledTick;
    }
}
