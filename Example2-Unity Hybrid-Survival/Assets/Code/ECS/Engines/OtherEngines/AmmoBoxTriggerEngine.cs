using System.Collections;
using UnityEngine;
using System;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS.Example.Survive.Player;
using Svelto.ECS.Example.Survive.Player.Gun;

namespace Svelto.ECS.Example.Survive.AmmoBox
{

    [Sequenced(nameof(EnginesNames.AmmoBoxTriggerEngine))]
    public class AmmoBoxTriggerEngine : IReactOnAddAndRemove<AmmoBoxEntityViewComponent>
                                   , IReactOnSwap<AmmoBoxEntityViewComponent>, IQueryingEntitiesEngine, IStepEngine
    {
        public AmmoBoxTriggerEngine(ITime time, IEntityFunctions entityFunctions, IEntityStreamConsumerFactory consumerFactory, WaitForSubmissionEnumerator waitForSubmission)
        {
            _time = time;
            _onCollidedWithTarget = OnCollidedWithTarget;
            _entityFunctions = entityFunctions;
            _consumerFactory = consumerFactory;
            _waitForSubmission = waitForSubmission;
            _changeID = new FasterList<IEnumerator>();
            _consumer = _consumerFactory.GenerateConsumer<AmmoBoxAttributeComponent>("AmmoBoxTriggerEngine", 0);
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready() { }

        public void Add(ref AmmoBoxEntityViewComponent entityViewComponent, EGID egid)
        {
            entityViewComponent.targetTriggerComponent.hitChange = new DispatchOnChange<AmmoBoxCollisionData>(egid, _onCollidedWithTarget);
        }

        public void Remove(ref AmmoBoxEntityViewComponent entityViewComponent, EGID egid)
        {
            entityViewComponent.targetTriggerComponent.hitChange = null;
        }
        public void MovedTo
          (ref AmmoBoxEntityViewComponent entityViewComponent, ExclusiveGroupStruct previousGroup, EGID egid)
        {
            if (egid.groupID.FoundIn(AmmoBoxUsed.Groups))
            {
                entityViewComponent.targetTriggerComponent.hitChange.value = new AmmoBoxCollisionData(playerID, false);
               

            }
        }
        void OnCollidedWithTarget(EGID sender, AmmoBoxCollisionData collisionData)
        {
            entitiesDB.QueryEntity<AmmoBoxAttributeComponent>(sender).ammoBoxCollisionData = collisionData;
        }
        public void Step()
        {
            foreach (var ((ammoBox, ammoboxEntity, count), _) in entitiesDB.QueryEntities<AmmoBoxAttributeComponent, AmmoBoxEntityViewComponent>(
                AmmoBoxAvailable.Groups))
            {
                for (var i = 0; i < count; i++)
                {
                    ref var ammoBoxComponent = ref ammoBox[i];
                    ref var collisionData = ref ammoBoxComponent.ammoBoxCollisionData;
                    

                    if (collisionData.collides == true)
                    {
                        
                       if (collisionData.otherEntityID.ToEGID(entitiesDB, out var otherEntityID) == true)
                        {

                            if (otherEntityID.groupID.FoundIn(Player.Player.Groups))
                            {
                                ReplenishAmmo(otherEntityID, ammoBoxComponent.returnAmmo);

                                _changeID.Add(ChangeGroup(ammoboxEntity[i].ID));

                                playerID = collisionData.otherEntityID;
                            }
                        }
                    }
                }
            }

            for (uint i = 0; i < _changeID.count; i++)
                if (_changeID[i].MoveNext() == false)
                    _changeID.UnorderedRemoveAt(i--);
        }

        void ReplenishAmmo(in EGID otherEntityID, int ammoReturn)
        {
            
            var weapon = entitiesDB.QueryEntity<PlayerWeaponComponent>(otherEntityID).weapon;
            
            var gunEGID = weapon.ToEGID(entitiesDB);

            ref var playerGunComponent = ref entitiesDB.QueryEntity<GunAttributesComponent>(gunEGID);

            if (ammoReturn > 0)
            {
                playerGunComponent.ammo += ammoReturn;
            }

            entitiesDB.PublishEntityChange<GunAttributesComponent>(gunEGID);
                           
        }

        IEnumerator ChangeGroup(EGID egid)
        {
            _entityFunctions.SwapEntityGroup<AmmoBoxEntityDescriptor>(egid, AmmoBoxUsed.BuildGroup);

            var entityComponent = entitiesDB.QueryEntity<AmmoBoxEntityViewComponent>(egid);

            while (_waitForSubmission.MoveNext())
                yield return null;

            var wait = new WaitForSecondsEnumerator(1);

            while (wait.MoveNext())
            {
                entityComponent.positionComponent.position = new Vector3(entityComponent.positionComponent.position.x,  - 5, entityComponent.positionComponent.position.z);

                yield return null;
            }

            var entityGid = new EGID(egid.entityID, AmmoBoxUsed.BuildGroup);

            var pickUpType = entitiesDB.QueryEntity<AmmoBoxAttributeComponent>(entityGid).playertargetType;

            //getting ready to recycle it
            //is what it should be doing but for some reason, every ammobox that is reused has its collided vaiable reset to true
            //at the AmmoBox spawner reuse AmmoBox enumerator, the collider is correctly set to false but somewhere between being 
            //reused and the add oncollidedWithTarget, the collider is set to true causing the ammobox to always give ammo back to the 
            //player even if the player isnt near the box. it also means the ammo box is never on the map after the first interaction
            //as it is always flying below the map being reused and set to new spawn points and immediately being used. 
            //its a real shame that i couldnt get this to work considering how close i am to having it work but i already spent far too 
            //long on the test then i should have. sorry
            _entityFunctions.SwapEntityGroup<AmmoBoxEntityDescriptor>(
                entityGid, ECSGroups.AmmoBoxToRcycleGroups + (uint)pickUpType);
            yield return null;
                
            
        }

        public string name => nameof(AmmoBoxTriggerEngine);

        readonly Action<EGID, AmmoBoxCollisionData> _onCollidedWithTarget;
        Consumer<AmmoBoxAttributeComponent> _consumer;
        WaitForSubmissionEnumerator _waitForSubmission;
        readonly FasterList<IEnumerator> _changeID;
        readonly IEntityFunctions _entityFunctions;
        readonly IEnumerator _changeGroup;
        readonly ITime _time;
        readonly IEntityStreamConsumerFactory _consumerFactory;
        EntityReference playerID;
    }
}
