using Svelto.ECS.Hybrid;

namespace Svelto.ECS.Example.Survive.AmmoBox
{
    public struct AmmoBoxEntityViewComponent : IEntityViewComponent
    {
        public IAmmoBoxTriggerComponent targetTriggerComponent;
        public IPositionComponent positionComponent;

        public EGID ID { get; set; }
    }
}
