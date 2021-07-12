namespace Svelto.ECS.Example.Survive.Weapons
{
    public struct AmmoValueComponent : IEntityComponent
    {
        public int ammoValue;
    }

    public struct AmmoCollisionComponent : IEntityComponent
    {
        public AmmoCollisionData entityInRange;
        public EGID originEGID;
    }
}