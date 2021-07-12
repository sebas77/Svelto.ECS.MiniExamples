using UnityEngine;

namespace Svelto.ECS.Example.Survive.Player.Gun
{
    public struct GunAttributesComponent : IEntityComponent
    {
        public float   timeBetweenBullets;
        public float   range;
        public int     damagePerShot;
        public float   timer;
        public Vector3 lastTargetPosition;

        //public int     ammoLeft;
    }
}