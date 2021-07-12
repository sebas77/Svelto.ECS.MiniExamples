using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS.Example.Survive.Camera;
using Svelto.ECS.Example.Survive.Characters;
using Svelto.ECS.Example.Survive.Enemies;
using Svelto.ECS.Example.Survive.Player;
using Svelto.ECS.Example.Survive.Player.Gun;
using Svelto.ECS.Example.Survive.Sounds;
using Svelto.ECS.Example.Survive.HUD;
using Svelto.ECS.Example.Survive.ResourceManager;
using Svelto.ECS.Example.Survive.Weapons;
using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Schedulers.Unity;
using UnityEngine;

//Note on this example:
//When developing with svelto the user would need to plan a bit what entities need to be used and
//what behaviours these entities will have. Engines can be added over the time, so the key is to have
//a good idea of the entities to work on. All the reasonings should always start from the EntityDescriptors
//Entity Components can be added over the time of course, but it's important to establish what entities
//will come into play.
//The key entities (and relative entity descriptors) for this demo are:
//Player
//PlayerGun
//Enemy
//Camera
//HUD
//Once the first entity descriptors are designed, it becomes simpler to start build these entities and
//write the engines for their behaviours.

//Note: this demo relies heavily on the publisher/consumer pattern. Engines can iterate quickly over entities
//but when they react on entity changes, iterating all the entities can be a waste. Relying too much on the
//publisher/consumer can have some drawbacks, so with Svelto.ECS 3.0 filters can be used instead. This doesn't
//mean that filters are better than the publisher/consumer model. Each tool must be used wisely.

//Main is the Application Composition Root. A Composition Root is the where all the dependencies are 
//created and injected (I talk a lot about this in my articles) A composition root belongs to the Context, but
//a context can have more than a composition root. For example a factory is a composition root.
//Furthermore an application can have more than a context/composition root but this is more advanced and not part of this demo
namespace Svelto.ECS.Example.Survive
{
    /// <summary>
    ///     IComposition root is part of Svelto.Context. Svelto.Context is not formally part of Svelto.ECS, but
    ///     it's helpful to use in an environment where a Context is not present, like in Unity.
    ///     It's a bootstrap! (check MainContext.cs)
    /// </summary>
    public class MainCompositionRoot : ICompositionRoot
    {
        EnginesRoot _enginesRoot;

        public MainCompositionRoot() { QualitySettings.vSyncCount = 1; }

        public void OnContextCreated<T>(T contextHolder) { }

        public void OnContextInitialized<T>(T contextHolder) { CompositionRoot(contextHolder as UnityContext); }

        public void OnContextDestroyed(bool hasBeenActivated)
        {
            //final clean up
            _enginesRoot?.Dispose();
        }

        /// <summary>
        ///     Before to start, let's review some of the Svelto.ECS terms:
        ///     - Entity:
        ///     it must be a real and concrete entity that you can explain in terms of game design. The name of each
        ///     entity should reflect a specific concept from the game design domain
        ///     - Engines (Systems):
        ///     Where all the logic lies. Engines operates on Entity Components
        ///     - IEntityComponent:
        ///     It's an Entity Component which can be used with Pure ECS
        ///     - IEntityViewComponent:
        ///     structs implementing this are used to wrap Objects that come from OOP libraries. You will never use it unless
        ///     you are forced to mix your ECS code with OOP code because of external libraries or platforms. These special
        ///     "Hybrid" component can hold only interfaces
        ///     - Implementors:
        ///     The EntityViewComponent exposed interfaces must be implemented by Implementors that are actually the
        ///     Objects you need to wrap.
        ///     - EntityDescriptors:
        ///     Gives a way to formalise your Entity, it also defines the components that must
        ///     be generated once the Entity is built
        /// </summary>
        void CompositionRoot(UnityContext contextHolder)
        {
            //the UnitySumbmissionEntityViewScheduler is the scheduler that is used by the EnginesRoot to know
            //when to submit the entities. Custom ones can be created for special cases.
            var unityEntitySubmissionScheduler = new UnityEntitiesSubmissionScheduler("survival");
            //The Engines Root is the core of Svelto.ECS. You shouldn't inject the EngineRoot,
            //therefore the composition root class must hold a reference or it will be garbage collected.
            _enginesRoot = new EnginesRoot(unityEntitySubmissionScheduler);
            //The EntityFactory can be injected inside factories (or engine acting as factories) to build new entities
            //dynamically
            var entityFactory = _enginesRoot.GenerateEntityFactory();
            //The entity functions is a set of utility operations on Entities, including removing an entity. I couldn't
            //find a better name so far.
            var entityFunctions             = _enginesRoot.GenerateEntityFunctions();
            var entityStreamConsumerFactory = _enginesRoot.GenerateConsumerFactory();

            //wrap non testable unity static classes, so that can be mocked if needed (or implementation can change in general, without changing the interface).
            IRayCaster rayCaster = new RayCaster();
            ITime      time      = new Time();
            //GameObjectFactory allows to create GameObjects without using the Static method GameObject.Instantiate.
            //While it seems a complication it's important to keep the engines testable and not coupled with hard
            //dependencies
            var gameObjectFactory = new GameObjectFactory();

            //Player related engines. ALL the dependencies must be solved at this point through constructor injection.
            var playerShootingEngine       = new PlayerGunShootingEngine(rayCaster, time);
            var playerMovementEngine       = new PlayerMovementEngine(rayCaster);
            var playerAnimationEngine      = new PlayerAnimationEngine();
            var playerDeathEngine          = new PlayerDeathEngine(entityFunctions, entityStreamConsumerFactory);
            var playerInputEngine          = new PlayerInputEngine();
            var playerGunShootingFXsEngine = new PlayerGunShootingFXsEngine(entityStreamConsumerFactory);
            //Spawner engines are factories engines that can build entities
            var playerSpawnerEngine      = new PlayerSpawnerEngine(gameObjectFactory, entityFactory);
            var restartGameOnPlayerDeath = new RestartGameOnPlayerDeathEngine();

            //Player engines
            _enginesRoot.AddEngine(playerMovementEngine);
            _enginesRoot.AddEngine(playerAnimationEngine);
            _enginesRoot.AddEngine(playerShootingEngine);
            _enginesRoot.AddEngine(playerInputEngine);
            _enginesRoot.AddEngine(playerGunShootingFXsEngine);
            _enginesRoot.AddEngine(playerDeathEngine);
            _enginesRoot.AddEngine(playerSpawnerEngine);
            _enginesRoot.AddEngine(restartGameOnPlayerDeath);

            //Factory is one of the few OOP patterns that work very well with ECS. Its use is highly encouraged
            var enemyFactory = new EnemyFactory(gameObjectFactory, entityFactory);
            //Enemy related engines
            var enemyAnimationEngine = new EnemyChangeAnimationOnPlayerDeathEngine();
            var enemyDamageFX        = new EnemySpawnEffectOnDamage(entityStreamConsumerFactory);
            var enemyAttackEngine    = new EnemyAttackEngine(time);
            var enemyMovementEngine  = new EnemyMovementEngine();
            //Spawner engines are factories engines that can build entities
            var enemySpawnerEngine = new EnemySpawnerEngine(enemyFactory, entityFunctions);
            var enemyDeathEngine = new EnemyDeathEngine(entityFunctions, entityStreamConsumerFactory, time
                                                      , new WaitForSubmissionEnumerator(
                                                            unityEntitySubmissionScheduler));

            //enemy engines
            _enginesRoot.AddEngine(enemySpawnerEngine);
            _enginesRoot.AddEngine(enemyAttackEngine);
            _enginesRoot.AddEngine(enemyMovementEngine);
            _enginesRoot.AddEngine(enemyAnimationEngine);
            _enginesRoot.AddEngine(enemyDeathEngine);
            _enginesRoot.AddEngine(enemyDamageFX);
            
            //abstract engines
            var applyDamageEngine        = new ApplyDamageToDamageableEntitiesEngine(entityStreamConsumerFactory);
            var cameraFollowTargetEngine = new CameraFollowingTargetEngine(time);
            var deathEngine              = new DispatchKilledEntitiesEngine();

            //abstract engines (don't need to know the entity type)
            _enginesRoot.AddEngine(applyDamageEngine);
            _enginesRoot.AddEngine(deathEngine);
            _enginesRoot.AddEngine(cameraFollowTargetEngine);

            //hud and sound engines
            var hudEngine         = new HUDEngine(entityStreamConsumerFactory);
            var damageSoundEngine = new DamageSoundEngine(entityStreamConsumerFactory);
            var scoreEngine       = new UpdateScoreEngine(entityStreamConsumerFactory);

            //other engines
            _enginesRoot.AddEngine(damageSoundEngine);
            _enginesRoot.AddEngine(hudEngine);
            _enginesRoot.AddEngine(scoreEngine);

            //Ammo engine
            var ammoSpawnerEngine = new AmmoSpawnerEngine(gameObjectFactory, entityFactory, entityFunctions);
            var ammoCollisionEngine = new AmmoCollisionEngine(entityFunctions);
            var ammoVisualEngine = new AmmoVisualEngine();
            _enginesRoot.AddEngine(ammoSpawnerEngine);
            _enginesRoot.AddEngine(ammoCollisionEngine);
            _enginesRoot.AddEngine(ammoVisualEngine);

            var unsortedEngines = new SurvivalUnsortedEnginesGroup(new FasterList<IStepEngine>(
                new IStepEngine[]
                {
                    playerMovementEngine,
                    playerInputEngine,
                    playerGunShootingFXsEngine,
                    playerSpawnerEngine,
                    playerAnimationEngine,
                    enemySpawnerEngine,
                    enemyMovementEngine,
                    cameraFollowTargetEngine,
                    hudEngine,
                    restartGameOnPlayerDeath,
                    ammoSpawnerEngine,
                    ammoCollisionEngine,
                    ammoVisualEngine
                }
            ));
            
            var unsortedDamageEngines = new DamageUnsortedEngines(new FasterList<IStepEngine>(
                new IStepEngine[]
                {
                    applyDamageEngine,
                    damageSoundEngine,
                    deathEngine
                }
            ));

            //Svelto ECS doesn't provide a tick system, hence it doesn't provide a solution to solve the order of execution
            //However it provides some option if you want to use them like the SortedEnginesGroup.
            _enginesRoot.AddEngine(new TickEnginesGroup(new FasterList<IStepEngine>(new IStepEngine[]
            {
                unsortedEngines
               , playerShootingEngine
               , enemyDamageFX
               , enemyAttackEngine
               , unsortedDamageEngines            
               , playerDeathEngine
               , enemyDeathEngine
               , scoreEngine
            })));

            BuildGUIEntitiesFromScene(contextHolder, entityFactory);
        }

        /// <summary>
        /// An EntityDescriptorHolder is a special Svelto.ECS hybrid class dedicated to the unity platform. Once attached to a gameobject
        /// it automatically retrieves implementors from the hierarchy.
        ///     This pattern is usually useful for guis where complex hierarchy of gameobjects are necessary, but
        /// otherwise you should always create entities in factories. In the mini examples repository is possible
        ///     to find a more advanced GUI example
        /// The gui of this project is ultra simple and is all managed by one entity only. This way won't do
        /// for a complex GUI.
        /// Note that creating an entity to manage a complex gui like this, is OK only for such a simple scenario
        /// otherwise a widget-like design should be adopted.
        /// </summary>
        /// <param name="contextHolder"></param>
        void BuildGUIEntitiesFromScene(UnityContext contextHolder, IEntityFactory entityFactory)
        {
            SveltoGUIHelper.Create<HudEntityDescriptorHolder>(ECSGroups.HUD, contextHolder.transform, entityFactory
                                                            , true);
        }
    }
}