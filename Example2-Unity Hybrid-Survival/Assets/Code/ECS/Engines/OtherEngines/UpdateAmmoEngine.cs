using System.Collections;
using Svelto.Common;
using Svelto.ECS.Example.Survive.Player.Gun;

namespace Svelto.ECS.Example.Survive.HUD
{

    [Sequenced(nameof(EnginesNames.UpdateAmmoEngine))]

    public class UpdateAmmoEngine : IQueryingEntitiesEngine, IStepEngine
    {
        public EntitiesDB entitiesDB { get; set; }
        public void Ready()
        {
            _listenForGunFire = ListenForGunFire();
        }
        public void Step()
        {
            _listenForGunFire.MoveNext();
        }
        public string name => nameof(UpdateAmmoEngine);

        IEnumerator ListenForGunFire()
        {
            var consumer = _consumerFactory.GenerateConsumer<GunAttributesComponent>("AmmoEngine", 10);

            while (entitiesDB.HasAny<HUDEntityViewComponent>(ECSGroups.GUICanvas) == false)
                yield return null;

            var hudEntityView = entitiesDB.QueryUniqueEntity<HUDEntityViewComponent>(ECSGroups.GUICanvas);

            while (true)
            {
                while(consumer.TryDequeue(out var ammo, out var egid))
                {
                    hudEntityView.ammoComponent.ammoCount = ammo.ammo;
                }
                yield return null;
            }
        }
        public UpdateAmmoEngine(IEntityStreamConsumerFactory consumerFactory) { _consumerFactory = consumerFactory; }

        readonly IEntityStreamConsumerFactory _consumerFactory;
        IEnumerator                           _listenForGunFire;
    }

}