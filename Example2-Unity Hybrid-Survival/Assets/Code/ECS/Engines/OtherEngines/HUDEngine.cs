using System.Collections;
using UnityEngine;
using Svelto.ECS.Example.Survive.Player;

namespace Svelto.ECS.Example.Survive.HUD
{
    /// <summary>
    /// Note I have been lazy with this class and not following the perfect abstraction rules. The project is too
    /// simple to abstract too much. However the mistakes are:
    /// - This engine shouldn't be aware of the concept of damage/health
    /// - this engine shouldn't know the players group
    /// - this engine shouldn't really have the responsibility to restart the level (ugh)
    /// </summary>
    public class HUDEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public HUDEngine(IEntityStreamConsumerFactory consumerFactory)
        {
            _consumerFactory = consumerFactory;
            _animateUI = AnimateUI();
            _checkForDamage = CheckForDamage();
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready()
        { }

        public void Step()
        {
            _animateUI.MoveNext();
            _checkForDamage.MoveNext();

        }
        public string name => nameof(HUDEngine);

        IEnumerator AnimateUI()
        {
            void AnimateUI()
            {
                var (buffer, count) = entitiesDB.QueryEntities<HUDEntityViewComponent>(ECSGroups.GUICanvas);
                for (int i = 0; i < count; ++i)
                {
                    var damageComponent = buffer[i].damageImageComponent;

                    damageComponent.imageColor = Color.Lerp(damageComponent.imageColor, Color.clear
                                                          , damageComponent.speed * UnityEngine.Time.deltaTime);
                }
            }

            while (true)
            {
                AnimateUI();

                yield return null;
            }
        }

        IEnumerator CheckForDamage()
        {
            var _consumerHealth = _consumerFactory.GenerateConsumer<HealthComponent>("HUDEngine", 1);

            while (true)
            {
                while (_consumerHealth.TryDequeue(out var health, out var egid))
                {
                    //this is a design mistake as this engine shouldn't be aware of the Player
                    if (Player.Player.Includes(egid.groupID))
                    {
                        //An engine should never assume how many entities will be used, so we iterate over all the
                        //HUDEntityViews even if we know there is just one
                        var guiEntityView = entitiesDB.QueryUniqueEntity<HUDEntityViewComponent>(ECSGroups.GUICanvas);

                        var damageComponent = guiEntityView.damageImageComponent;
                        damageComponent.imageColor = damageComponent.flashColor;

                        guiEntityView.healthSliderComponent.value = health.currentHealth;
                    }
                }

                yield return null;
            }
        }
        


        readonly IEntityStreamConsumerFactory _consumerFactory;
        readonly IEnumerator _animateUI;
        readonly IEnumerator _checkForDamage;
    }
}