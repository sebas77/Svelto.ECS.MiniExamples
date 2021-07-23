using System.Collections;
using System.Collections.Generic;
using Svelto.ECS.Example.Survive.Camera;
using Svelto.ECS.Example.Survive.Player.Gun;
using Svelto.ECS.Example.Survive.ResourceManager;
using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Player
{
    public class PlayerSpawnerEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public PlayerSpawnerEngine(GameObjectFactory gameobjectFactory, IEntityFactory entityFactory)
        {
            _gameobjectFactory = gameobjectFactory;
            _entityFactory = entityFactory;
        }
        
        public EntitiesDB entitiesDB { private get; set; }

        public void Ready() { _spawnPlayer = SpawnPlayer(); }

        public void   Step() { _spawnPlayer.MoveNext(); }
        public string name   => nameof(PlayerSpawnerEngine);

        IEnumerator SpawnPlayer()
        {
            IEnumerator<GameObject> playerLoading = _gameobjectFactory.Build("Player");
            
            while (playerLoading.MoveNext()) yield return null;
            
            IEnumerator<GameObject> cameraLoading = _gameobjectFactory.Build("CameraPrefab");
            
            while (cameraLoading.MoveNext()) yield return null;
            
            GameObject player = playerLoading.Current;
            GameObject camera = cameraLoading.Current;
            
            //Get the gameobject "implementors". Implementors can be monobehaviours that can be used with Svelto.ECS
            //unluckily the gun is found in the same prefab of the character, so the best thing to do is to search
            //for all the implementors in the hierarchy and pass them to the BuildEntity. The BuildEntity will then
            //search for the implementors they need (player and guns separately)
            List<IImplementor> implementors = new List<IImplementor>();
            player.GetComponentsInChildren(true, implementors);

            //EghidHolderImplementor is a special framework provided implementor that tracks the EGID 
            //of the entity linked to the implementor
            EntityReferenceHolderImplementor egidHoldImplementor = player.AddComponent<EntityReferenceHolderImplementor>();
            implementors.Add(egidHoldImplementor);
            
            BuildPlayerEntity(implementors, out var playerInitializer);
            BuildGunEntity(playerInitializer, implementors);
            BuildCameraEntity(ref playerInitializer, camera);

            egidHoldImplementor.reference = playerInitializer.reference;
        }

        void BuildPlayerEntity(List<IImplementor> implementors, out EntityInitializer playerInitializer)
        {
            //Build the Svelto ECS entity for the player. Svelto.ECS has the unique feature to let the user decide
            //the ID of the entity (which must be anyway unique). The user may think that using, for example, 
            //the GameObject.GetInstanceID() value as entity ID is a good idea, as it would be simple to fetch the
            //entity from the outcome of unity callbacks (like collisions). While you can do so, you must not think
            //it's the only way to go. For this reason I decide instead to use 0 for this example and show how
            //to handle the situation.
            //ECSGroups.Player is the group where the entity player will be built. I usually expect a 
            //group for entity descriptor. It is the safest way to go, but advanced users may decide to use different
            //groups layout if needed.
            //if the Svelto entity is linked to an external OOP resource, like the GameObject in this case, the
            //relative implementor must be passed to the BuildEntity method.
            //Pure ECS (no OOP) entities do not need implementors passed.
            playerInitializer = _entityFactory.BuildEntity<PlayerEntityDescriptor>(0, Player.BuildGroup, implementors);

            //BuildEntity returns an initializer that can be used to initialise all the entity components generated
            //by the entity descriptor. In this case I am initializing just the Health.
            //being lazy here, it should be read from json file
            playerInitializer.Init(new HealthComponent
            {
                currentHealth = 100
            });
            playerInitializer.Init(new SpeedComponent(6));

        }

        void BuildGunEntity(EntityInitializer playerInitializer, List<IImplementor> implementors)
        {
            //Gun and player are two different entities, but they are linked by the EGID
            //in this case we assume that we know at all the time the ID of the gun and the group where the gun is
            //but this is not often the case when groups must be swapped.
            
            //Note: in the last refactoring I changed the code to not rely on the knowledge that the ID is known
            //as this trick cannot be used to determine an EGID anymore once group compounds are used. 
            //therefore I switched to the use of EntityReferences.
            var init = _entityFactory.BuildEntity<PlayerGunEntityDescriptor>(playerInitializer.EGID.entityID
                                                                           , Survive.Weapons.Gun.BuildGroup, implementors);

            //being lazy here, it should be read from json file
            init.Init(new GunAttributesComponent()
            {
                timeBetweenBullets = 0.15f
              , range              = 100f
              , damagePerShot      = 20
              , ammo               = 100
              ,
            });
            
            playerInitializer.Init(new PlayerWeaponComponent()
            {
                weapon = init.reference
            });
        }

        /// <summary>
        /// This demo has just one camera, but it would be simple to create a camera for each player for a split
        /// screen scenario. 
        /// <param name="playerID"></param>
        /// <param name="gameObject"></param>
        void BuildCameraEntity(ref EntityInitializer playerID, GameObject camera)
        {
            //implementors can be attatched at run time, while not? Check the player spawner engine to 
            //read more about implementors
            var implementor                     = camera.AddComponent<CameraImplementor>();
            var cameraTargetEntityViewComponent = playerID.Get<CameraTargetEntityViewComponent>();
            implementor.offset = camera.transform.position - cameraTargetEntityViewComponent.targetComponent.position;

            var init = _entityFactory.BuildEntity<CameraEntityDescriptor>(playerID.EGID.entityID, Camera.Camera.BuildGroup
                                                            , new[] {implementor});
            
            init.Init(new CameraTargetEntityReferenceComponent() { targetEntity = playerID.reference } );
            
            playerID.Init(new CameraReferenceComponent() { cameraReference = init.reference } );
        }
        
        readonly IEntityFactory    _entityFactory;
        readonly GameObjectFactory _gameobjectFactory;
        IEnumerator                _spawnPlayer;
    }
}