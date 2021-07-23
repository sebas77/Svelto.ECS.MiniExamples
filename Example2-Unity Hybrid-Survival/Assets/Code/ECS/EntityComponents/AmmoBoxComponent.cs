using UnityEngine;
using Svelto.ECS.Example.Survive.Player;

namespace Svelto.ECS.Example.Survive.AmmoBox
{
  public struct AmmoBoxAttributeComponent : IEntityComponent
    {
        public AmmoBoxCollisionData ammoBoxCollisionData;
        public int returnAmmo;
        public PlayerTargetType playertargetType;
    }
}