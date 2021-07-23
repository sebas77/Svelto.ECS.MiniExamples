using System;
using Svelto.Common;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Enemies
{
    [Sequenced(nameof(EnemyEnginesNames.EnemyAttackEngine))]
    public class EnemyAttackEngine : IReactOnAddAndRemove<EnemyEntityViewComponent>
                                   , IReactOnSwap<EnemyEntityViewComponent>, IQueryingEntitiesEngine, IStepEngine
    {
        public EnemyAttackEngine(ITime time)
        {
            _time                 = time;
            _onCollidedWithTarget = OnCollidedWithTarget;
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready() {}

        /// <summary>
        ///     The Add and Remove callbacks are enabled by the IReactOnAddAndRemove interface
        ///     They are called when:
        ///     an Entity is built in a group  (no swap case)
        ///     an Entity is removed from a group (no swap case)
        /// </summary>
        /// <param name="entityViewComponent">the up to date entity</param>
        /// <param name="previousGroup">where the entity is coming from</param>
        public void Add(ref EnemyEntityViewComponent entityViewComponent, EGID egid)
        {
            //for each new enemy entity added, we register a new DispatchOnChange
            //DispatchOnChange is a simple solution to let implementors communicate with engine
            //An Implementor can communicate only with an appointed engine and the engine can broadcast the information
            //if necessary.
            //set what callback must be called when the implementor dispatch the value change
            entityViewComponent.targetTriggerComponent.hitChange = new DispatchOnChange<EnemyCollisionData>(egid, _onCollidedWithTarget);
        }

        public void Remove(ref EnemyEntityViewComponent entityViewComponent, EGID egid)
        {
            //for safety we clean up the dispatcher on change in entity removal
            entityViewComponent.targetTriggerComponent.hitChange = null;
        }

        /// <summary>
        ///     MovedTo callbacks are enabled by the IReactOnSwap interface
        ///     They are called on entity swap (when leaving a group and moving to the new one)
        /// </summary>
        /// <param name="entityViewComponent"></param>
        public void MovedTo
            (ref EnemyEntityViewComponent entityViewComponent, ExclusiveGroupStruct previousGroup, EGID egid)
        {
            //If the enemy is dead, we pause the collision triggering, it will be renabled if the GO is recycled
            if (egid.groupID.FoundIn(Dead.Groups))
                entityViewComponent.targetTriggerComponent.hitChange.PauseNotify();
            else
                entityViewComponent.targetTriggerComponent.hitChange.ResumeNotify();
        }

        /// <summary>
        /// once an enemy enters in a trigger, we set the trigger data built inside the implementor and sent
        /// through the DispatchOnChange
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="enemyCollisionData"></param>
        void OnCollidedWithTarget(EGID sender, EnemyCollisionData enemyCollisionData)
        {
            entitiesDB.QueryEntity<EnemyAttackComponent>(sender).entityInRange = enemyCollisionData;
        }

        public void Step()
        {
            foreach (var ((enemiesAttackData, enemiesCount), _) in entitiesDB.QueryEntities<EnemyAttackComponent>(
                AliveEnemies.Groups))
            {
                for (var enemyIndex = 0; enemyIndex < enemiesCount; enemyIndex++)
                {
                    ref var enemyAttackComponent = ref enemiesAttackData[enemyIndex];
                    ref var enemyCollisionData   = ref enemyAttackComponent.entityInRange;

                    if (enemyCollisionData.collides == true)
                    {
                        enemyAttackComponent.timer += _time.deltaTime;

                        if (enemyAttackComponent.timer >= enemyAttackComponent.timeBetweenAttack)
                        {
                            enemyAttackComponent.timer = 0.0f;

                            //if this fails, it means that the target entity doesn't exist anymore. 
                            //Unluckily Unity doesn't trigger OnTriggerExit on RB disabled and any way to find
                            //a work around it, would have been much more complex than just this if
                            if (enemyCollisionData.otherEntityID.ToEGID(entitiesDB, out var otherEntityID) == true)
                            {
                                if(otherEntityID.groupID.FoundIn(Player.Player.Groups))
                                DamageTargetInsideRange(otherEntityID, enemyAttackComponent.attackDamage);
                            }
                        }
                    }
                }
            }
        }

        void DamageTargetInsideRange(in EGID otherEntityID, int attackDamage)
        {
           entitiesDB.QueryEntity<DamageableComponent>(otherEntityID).damageInfo =
                new DamageInfo(attackDamage, Vector3.zero);

            entitiesDB.PublishEntityChange<DamageableComponent>(otherEntityID);
        }

        public string name => nameof(EnemyAttackEngine);

        readonly Action<EGID, EnemyCollisionData> _onCollidedWithTarget;
        readonly ITime                            _time;
    }
}