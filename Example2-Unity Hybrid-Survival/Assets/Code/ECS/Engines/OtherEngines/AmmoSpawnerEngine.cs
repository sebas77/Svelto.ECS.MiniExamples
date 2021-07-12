using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Svelto.ECS.Example.Survive.ResourceManager;
using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Hybrid;

namespace Svelto.ECS.Example.Survive.Weapons
{
    public class AmmoSpawnerEngine : IQueryingEntitiesEngine, IStepEngine
    {
        const int MAX_NUMBER_AMMO_CRATES = 5;

        public AmmoSpawnerEngine(GameObjectFactory gameobjectFactory, IEntityFactory entityFactory, IEntityFunctions entityFunctions)
        {
            _gameobjectFactory = gameobjectFactory;
            _entityFactory = entityFactory;
            _entityFunctions = entityFunctions;
        }

        public EntitiesDB entitiesDB { private get; set; }

        public void Ready() { 
            _mainTick = MainTick();
        }

        public void Step() { 
            _mainTick.MoveNext();
        }
        public string name => nameof(AmmoSpawnerEngine);

        IEnumerator MainTick()
        {
            static Vector3 GetPosition()
            {
                Vector3 spawnPos = new Vector3(UnityEngine.Random.Range(-15f, 15f), 1, UnityEngine.Random.Range(-15f, 15f));
                var hitColliders = Physics.OverlapSphere(spawnPos, 0.9f);
                if (hitColliders.Length > 0.1f) spawnPos = GetPosition();

                return spawnPos;
            }

            while (true)
            {
                //wait random range between 2 and 6 seconds
                var waitForSecondsEnumerator = new WaitForSecondsEnumerator(UnityEngine.Random.Range(2f, 6f));
                while (waitForSecondsEnumerator.MoveNext())
                    yield return null;

                //check for dead ammo and reuse them
                if (entitiesDB.HasAny<AmmoEntityViewComponent>(AmmoDocile.BuildGroup))
                {
                    var (ammoValue, ammoCollision, ammoView, count) = entitiesDB.QueryEntities<AmmoValueComponent, AmmoCollisionComponent, AmmoEntityViewComponent>(AmmoDocile.BuildGroup);

                    if (count > 0)
                    {
                        ammoValue[0].ammoValue = 20;
                        ammoCollision[0].entityInRange = new AmmoCollisionData(new EntityReference(), false);
                        ammoView[0].ammoComponent.position = GetPosition();
                        _entityFunctions.SwapEntityGroup<AmmoEntityDescriptor>(ammoView[0].ID, AmmoActive.BuildGroup);
                    }
                }
                //else create a new ammo entity if room
                else if (_ammoCreated < MAX_NUMBER_AMMO_CRATES)
                {
                    IEnumerator<GameObject> ammoLoading = _gameobjectFactory.Build("AmmoCrate");

                    while (ammoLoading.MoveNext()) yield return null;

                    GameObject ammoObject = ammoLoading.Current;

                    //Implementors
                    List<IImplementor> implementors = new List<IImplementor>();
                    ammoObject.GetComponentsInChildren(true, implementors);

                    //EGID
                    EntityReferenceHolderImplementor egidHoldImplementor = ammoObject.AddComponent<EntityReferenceHolderImplementor>();
                    implementors.Add(egidHoldImplementor);

                    //initialise
                    EntityInitializer ammoInitializer = _entityFactory.BuildEntity<AmmoEntityDescriptor>(_ammoCreated++, AmmoActive.BuildGroup, implementors);
                    ammoInitializer.Init(new AmmoValueComponent
                    {
                        ammoValue = 20
                    });
                    ammoObject.transform.position = GetPosition();

                    egidHoldImplementor.reference = ammoInitializer.reference;
                }
                

                yield return null;
            }
        }

        readonly IEntityFactory      _entityFactory;
        readonly GameObjectFactory   _gameobjectFactory;
        readonly IEntityFunctions    _entityFunctions;
        IEnumerator                  _mainTick;
        uint                         _ammoCreated;
    }
}