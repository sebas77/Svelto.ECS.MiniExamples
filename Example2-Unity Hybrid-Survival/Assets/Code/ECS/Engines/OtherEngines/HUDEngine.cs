using System.Collections;
using UnityEngine;

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
            _checkForDamage= CheckForDamage();
            _checkForWaveChange = CheckForWaveChange();
            _checkForAmmoChange = CheckForAmmoChange();
            _wavenum = 0;
            _announceActive = true;
        }

        public EntitiesDB entitiesDB { set; private get; }

        public void Ready()
        { }

        public void Step()
        {
            _animateUI.MoveNext();
            _checkForDamage.MoveNext();
            _checkForWaveChange.MoveNext();
            _checkForAmmoChange.MoveNext();
        }
        public string name   => nameof(HUDEngine);

        IEnumerator AnimateUI()
        {
            void AnimateUI()
            {
                var (buffer, count) = entitiesDB.QueryEntities<HUDEntityViewComponent>(ECSGroups.GUICanvas);
                for (int i = 0; i < count; ++i)
                {
                    //Damage flashing
                    var damageComponent = buffer[i].damageImageComponent;

                    damageComponent.imageColor = Color.Lerp(damageComponent.imageColor, Color.clear
                                                          , damageComponent.speed * UnityEngine.Time.deltaTime);

                    //Next wave incoming announcement animation
                    var announcementComponent = buffer[i].announcementHUDComponent;

                    if (_announceActive && announcementComponent.textColor.a >= 0.99f)
                    {
                        _announceActive = false;
                        announcementComponent.targetColor = new Color(1f, 0f, 0f, 0f);
                    }

                    announcementComponent.textColor = Color.Lerp(announcementComponent.textColor, announcementComponent.targetColor, announcementComponent.speed * UnityEngine.Time.deltaTime);
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

        IEnumerator CheckForWaveChange()
        {
            var _consumerWave = _consumerFactory.GenerateConsumer<WaveDataComponent>("HUDEngine", 1);

            while (true)
            {
                while (_consumerWave.TryDequeue(out var wave, out var egid))
                {
                    var (buffer, count) = entitiesDB.QueryEntities<HUDEntityViewComponent>(ECSGroups.GUICanvas);

                    for (int i = 0; i < count; ++i)
                    {
                        //Check for wave change
                        var announcementComponent = buffer[i].announcementHUDComponent;
                        if (wave.waveValue > _wavenum)
                        {
                            _announceActive = true;
                            announcementComponent.speed = 2f;
                            announcementComponent.targetColor = new Color(1f, 0f, 0f, 1f);
                            _wavenum = wave.waveValue;
                        }

                        //update UI
                        var waveImp = buffer[i].waveDataComponent;
                        waveImp.enemies = wave.enemyCount;
                        waveImp.wave = wave.waveValue;
                    }
                }

                yield return null;
            }
        }

        IEnumerator CheckForAmmoChange()
        {
            var _consumerAmmo = _consumerFactory.GenerateConsumer<Weapons.AmmoValueComponent>("HUDEngine", 10);

            while (true)
            {
                while (_consumerAmmo.TryDequeue(out var ammoValue, out var egid))
                {
                    var (buffer, count) = entitiesDB.QueryEntities<HUDEntityViewComponent>(ECSGroups.GUICanvas);

                    for (int i = 0; i < count; ++i)
                    {
                        var ammoImp = buffer[i].ammoComponent;
                        ammoImp.ammo = ammoValue.ammoValue;
                    }
                }
                yield return null;
            }
        }

        readonly IEntityStreamConsumerFactory _consumerFactory;
        readonly IEnumerator                  _animateUI;
        readonly IEnumerator                  _checkForDamage;

        readonly IEnumerator                  _checkForWaveChange;
        readonly IEnumerator                  _checkForAmmoChange;

        int _wavenum;
        bool _announceActive;
    }
}