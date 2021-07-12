using System.Collections;
using Svelto.Common;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Player.Gun
{
    [Sequenced(nameof(PlayerEnginesNames.PlayerGunShootingEngine))]
    public class PlayerGunShootingEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public PlayerGunShootingEngine(IRayCaster rayCaster, ITime time)
        {
            _rayCaster = rayCaster;
            _time      = time;
            _shootTick = Tick();
        }

        public EntitiesDB entitiesDB { set; private get; }
        public void       Ready()    {}
        public void       Step()     { _shootTick.MoveNext(); }
        public string     name       => nameof(PlayerGunShootingEngine);

        IEnumerator Tick()
        {
            void Shoot()
            {
                //probable design issue: player and gun is too coupled
                foreach (var ((playersInputs, weapons, count), _) in entitiesDB
                   .QueryEntities<PlayerInputDataComponent, PlayerWeaponComponent>(Player.Groups))
                {
                    for (var i = 0; i < count; i++)
                    {
                        var input = playersInputs[i];
                        if (input.fire)
                        {
                            var     gunEGID          = weapons[i].weapon.ToEGID(entitiesDB);
                            ref var playerGunComponent = ref entitiesDB.QueryEntity<GunAttributesComponent>(gunEGID);
                            ref var playerGunViewComponent = ref entitiesDB.QueryEntity<GunEntityViewComponent>(gunEGID);
                            ref var ammoComponent = ref entitiesDB.QueryEntity<Weapons.AmmoValueComponent>(gunEGID);

                            playerGunComponent.timer += _time.deltaTime;

                            if (playerGunComponent.timer >= playerGunComponent.timeBetweenBullets && ammoComponent.ammoValue > 0)
                                this.Shoot(ref playerGunComponent, playerGunViewComponent, ref ammoComponent);
                        }
                    }
                }
            }

            while (true)
            {
                Shoot();

                yield return null;
            }
        }

        /// <summary>
        ///     Design note: shooting and find a target are possibly two different responsibilities
        ///     and probably would need two different engines. This is a simple project and doesn't need
        ///     this level of abstraction. Never abstract too early! Early abstraction is the root of all the evil!
        ///     Svelto and ECS help immensely to abstract only when it's actually needed
        /// </summary>
        /// <param name="playerGunEntityView"></param>
        void Shoot(ref GunAttributesComponent playerGunEntityView, in GunEntityViewComponent gunFxComponent, ref Weapons.AmmoValueComponent ammoValueComponent)
        {
            playerGunEntityView.timer = 0;

            ammoValueComponent.ammoValue--;
            entitiesDB.PublishEntityChange<Weapons.AmmoValueComponent>(gunFxComponent.ID);

            Ray shootRay = gunFxComponent.gunFXComponent.shootRay;
            
            //CheckHit returns the EGID of the entity linked to the gameobject hit
            var hit = _rayCaster.CheckHit(shootRay, playerGunEntityView.range, GAME_LAYERS.ENEMY_LAYER
                                              , GAME_LAYERS.SHOOTABLE_MASK | GAME_LAYERS.ENEMY_MASK, out var point
                                              , out var referenceID);

            //invalid entity reference is a valid return from CheckHit, it means that something has been hit but it's not an entity
            if (hit && referenceID != EntityReference.Invalid)
            {
                var damageInfo = new DamageInfo(playerGunEntityView.damagePerShot, point);

                var instanceID = referenceID.ToEGID(entitiesDB);
                {
                    entitiesDB.QueryEntity<DamageableComponent>(instanceID).damageInfo = damageInfo;

                    entitiesDB.PublishEntityChange<DamageableComponent>(instanceID);
                }

                playerGunEntityView.lastTargetPosition = point;
            }
            else
                playerGunEntityView.lastTargetPosition =
                    shootRay.origin + shootRay.direction * playerGunEntityView.range;

            entitiesDB.PublishEntityChange<GunAttributesComponent>(gunFxComponent.ID);
        }

        /// <summary>
        /// Pay attention: _raycaster is a stateless object so in reality it just encapsulate logic to not
        /// repeat the same code over and over. Using an object instead than a static class is interesting
        /// because it would be possible to inject different implementations in the engine. 
        /// </summary>
        readonly IRayCaster _rayCaster;
        readonly ITime       _time;
        readonly IEnumerator _shootTick;

    }
}