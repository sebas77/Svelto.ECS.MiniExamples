namespace Svelto.ECS.Example.Survive.Player
{
    public readonly struct SpeedComponent : IEntityComponent
    {
        public readonly float movementSpeed;

        public SpeedComponent(float speed)
        {
            movementSpeed = speed;
        }
    }
}