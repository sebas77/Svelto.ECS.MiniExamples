using System;
using Svelto.Common;
using UnityEngine;
using Svelto.ECS.Example.Survive.Player.Gun;

namespace Svelto.ECS.Example.Survive.Weapons
{
    public class AmmoCollisionEngine : IReactOnAddAndRemove<AmmoEntityViewComponent>, IQueryingEntitiesEngine, IStepEngine
    {
        public AmmoCollisionEngine(IEntityFunctions entityFunctions) {
            _onCollidedWithTarget = OnCollidedWithTarget;
            _entityFunctions = entityFunctions;
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready() {  }

        public void Add(ref AmmoEntityViewComponent ammoEntityViewComponent, EGID egid)
        {
            ammoEntityViewComponent.triggerComponent.hitChange = new DispatchOnChange<AmmoCollisionData>(egid, _onCollidedWithTarget);
        }

        public void Remove(ref AmmoEntityViewComponent ammoEntityViewComponent, EGID egid)
        {

        }

        public void Step()
        {
            foreach (var ((ammoCollisionComponent, count), _) in entitiesDB.QueryEntities<AmmoCollisionComponent>(AmmoActive.Groups))
            {
                for (var i = 0; i < count; i++)
                {
                    var collisionData = ammoCollisionComponent[i].entityInRange;

                    if (collisionData.collides && collisionData.otherEntityID.ToEGID(entitiesDB, out var otherEntityID) && Player.Player.Includes(otherEntityID.groupID))
                    {
                        var weaponEGID = entitiesDB.QueryEntity<Player.PlayerWeaponComponent>(otherEntityID).weapon.ToEGID(entitiesDB);
                        ref var ammoComponent = ref entitiesDB.QueryEntity<AmmoValueComponent>(weaponEGID);
                        ref var placedAmmoComponent = ref entitiesDB.QueryEntity<AmmoValueComponent>(ammoCollisionComponent[i].originEGID);
                        ammoComponent.ammoValue += placedAmmoComponent.ammoValue;

                        var ammoEntityViewComponent = entitiesDB.QueryEntity<AmmoEntityViewComponent>(ammoCollisionComponent[i].originEGID);
                        ammoEntityViewComponent.ammoComponent.position -= new Vector3(0, 10, 0);
                        entitiesDB.PublishEntityChange<AmmoValueComponent>(weaponEGID);

                        _entityFunctions.SwapEntityGroup<AmmoEntityDescriptor>(ammoCollisionComponent[i].originEGID, AmmoDocile.BuildGroup);
                    }
                }
            }
        }

        void OnCollidedWithTarget(EGID sender, AmmoCollisionData ammoCollisionData)
        {
            //sender egid doesn't update when the entity gets swapped to a different group, need to update the reference holder
            if (entitiesDB.Exists<AmmoCollisionComponent>(sender))
            {
                ref var ammoCollisionComponent = ref entitiesDB.QueryEntity<AmmoCollisionComponent>(sender);
                ammoCollisionComponent.entityInRange = ammoCollisionData;
                ammoCollisionComponent.originEGID = sender;
            }
        }

        public string name => nameof(AmmoCollisionEngine);

        readonly IEntityFunctions _entityFunctions;
        readonly Action<EGID, AmmoCollisionData> _onCollidedWithTarget;
    }
}

