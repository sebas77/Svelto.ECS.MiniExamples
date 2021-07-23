using Svelto.ECS.Hybrid;

namespace Svelto.ECS.Example.Survive.Player
{
    public struct PlayerEntityViewComponent : IEntityViewComponent
    {
        public IRigidBodyComponent   rigidBodyComponent;
        public IPositionComponent    positionComponent;
        public IAnimationComponent   animationComponent;
        public ITransformComponent   transformComponent;     
        public EGID                ID { get; set; }
    }
}
