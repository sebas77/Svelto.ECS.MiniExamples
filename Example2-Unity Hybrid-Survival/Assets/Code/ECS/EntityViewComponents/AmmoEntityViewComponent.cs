using Svelto.ECS.Hybrid;

namespace Svelto.ECS.Example.Survive.Weapons
{
    public struct AmmoEntityViewComponent : IEntityViewComponent
    {
        public IAmmoComponent ammoComponent;
        public IAmmoTriggerComponent triggerComponent;
        
        public EGID            ID { get; set; }
    }
}