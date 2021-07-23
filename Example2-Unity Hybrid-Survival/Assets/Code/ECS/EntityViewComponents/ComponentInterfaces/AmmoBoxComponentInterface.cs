using System;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.AmmoBox
{
    public interface IAmmoBoxTriggerComponent
    {
        DispatchOnChange<AmmoBoxCollisionData> hitChange { get; set; }
    }

    public struct AmmoBoxCollisionData : IEquatable<AmmoBoxCollisionData>
    {
        public EntityReference otherEntityID;
        public readonly bool collides;
        public AmmoBoxCollisionData(EntityReference otherEntityID, bool collides)
        {
            this.otherEntityID = otherEntityID;

            this.collides = collides;
        }
        public bool Equals(AmmoBoxCollisionData other)
        {
            return otherEntityID.Equals(other.otherEntityID) && collides == other.collides;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (otherEntityID.GetHashCode() * 397) ^ collides.GetHashCode();
            }
        }
    }
}