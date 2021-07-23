using System.Collections;
using System.Collections.Generic;
using Svelto.ECS.Example.Survive.AmmoBox;
using Svelto.ECS.Example.Survive.ResourceManager;
using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive
{
    public class AmmoBoxFactory
    {
        public AmmoBoxFactory(GameObjectFactory gameObjectFactory, IEntityFactory entityFactory)
        {
            _gameobjectFactory = gameObjectFactory;
            _entityFactory = entityFactory;

        }

        public IEnumerator Build(AmmoBoxSpawnData ammoBoxSpawnData)
        {
            var build = _gameobjectFactory.Build(ammoBoxSpawnData.AmmoBoxPrefab, false);

            while (build.MoveNext())
                yield return null;

            GameObject ammoBoxGO = build.Current;

            List<IImplementor> implementors = new List<IImplementor>();
            ammoBoxGO.GetComponentsInChildren(implementors);
            var egidHolderImplementor = ammoBoxGO.AddComponent<EntityReferenceHolderImplementor>();
            implementors.Add(egidHolderImplementor);

            var initializer =
               _entityFactory.BuildEntity<AmmoBoxEntityDescriptor>(new EGID(_AmmoBoxCreated++, AmmoBoxAvailable.BuildGroup)
                                                               , implementors);

            egidHolderImplementor.reference = initializer.reference;

            initializer.Init(new AmmoBoxAttributeComponent()
            {
                returnAmmo = ammoBoxSpawnData.ammoReturn,
                playertargetType = ammoBoxSpawnData.playerTargetTag
            });


            var transform = ammoBoxGO.transform;
            var spawnPoint = ammoBoxSpawnData.spawnPoints;

            ammoBoxGO.SetActive(true);

            //set random spawn points from json file
            transform.position = spawnPoint[Random.Range(0, spawnPoint.Length-1)];
            yield return null;
        }

        readonly IEntityFactory _entityFactory;
        readonly GameObjectFactory _gameobjectFactory;
        uint _AmmoBoxCreated;
    }
}
