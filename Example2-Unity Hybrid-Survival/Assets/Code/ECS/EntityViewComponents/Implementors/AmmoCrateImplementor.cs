using UnityEngine;
using Svelto.ECS.Hybrid;
using Svelto.ECS.Extensions.Unity;

namespace Svelto.ECS.Example.Survive.Weapons
{
    public class AmmoCrateImplementor : MonoBehaviour, IImplementor, IAmmoComponent, IAmmoTriggerComponent
    {
        public DispatchOnChange<AmmoCollisionData> hitChange { get; set; }

        public Quaternion rotation { get => _transform.rotation; set => _transform.rotation = value; }

        public Vector3 position { get => _transform.position; set => _transform.position = value; }

        void Awake()
        {
            _transform = transform;
        }

        void OnTriggerEnter(Collider other)
        {
            if (hitChange != null && other.attachedRigidbody != null)
            {
                EntityReferenceHolderImplementor otherEntityReferenceImp = other.gameObject.GetComponent<EntityReferenceHolderImplementor>();
                if (otherEntityReferenceImp != null)
                {
                    hitChange.value = new AmmoCollisionData(otherEntityReferenceImp.reference, true);
                }
                    
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (hitChange != null && other.attachedRigidbody != null)
            {
                EntityReferenceHolderImplementor otherEntityReferenceImp = other.gameObject.GetComponent<EntityReferenceHolderImplementor>();
                if (otherEntityReferenceImp != null)
                    hitChange.value = new AmmoCollisionData(otherEntityReferenceImp.reference, false);
            }
        }

        Transform _transform;
    }
}