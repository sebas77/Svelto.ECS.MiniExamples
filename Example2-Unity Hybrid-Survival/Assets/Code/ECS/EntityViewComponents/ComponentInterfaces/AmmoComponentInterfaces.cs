using UnityEngine;
using System;

namespace Svelto.ECS.Example.Survive.Weapons
{
    public interface IAmmoComponent
    {
        Quaternion rotation { get; set; }

        Vector3 position { get; set; }
    }

    public interface IAmmoTriggerComponent
    {
        DispatchOnChange<AmmoCollisionData> hitChange { get; set; }
    }

    public struct AmmoCollisionData : IEquatable<AmmoCollisionData>
    {
        public EntityReference otherEntityID;
        public readonly bool collides;

        public AmmoCollisionData(EntityReference otherEntityID, bool collides)
        {
            this.otherEntityID = otherEntityID;
            this.collides = collides;
        }

        public bool Equals(AmmoCollisionData other)
        {
            return otherEntityID.Equals(other.otherEntityID) && collides == other.collides;
        }

        public override int GetHashCode()
        {
            return (otherEntityID.GetHashCode() * 397) ^ collides.GetHashCode();
        }
    }
}