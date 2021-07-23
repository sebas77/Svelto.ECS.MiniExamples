using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.AmmoBox
{
    public class AmmoBoxTriggerImplementor : MonoBehaviour, IImplementor, IAmmoBoxTriggerComponent
    {
        public DispatchOnChange<AmmoBoxCollisionData> hitChange { get; set; }

        void OnTriggerEnter(Collider other)
        {
          if (hitChange != null && other.attachedRigidbody != null)
            {
                EntityReferenceHolderImplementor targetEntityViewComponent = other.gameObject.GetComponent<EntityReferenceHolderImplementor>();
                if (targetEntityViewComponent != null)
                   hitChange.value = new AmmoBoxCollisionData(targetEntityViewComponent.reference, true);
            }
    }

        void OnTriggerExit(Collider other)
        {
            if (hitChange != null && other.attachedRigidbody != null)
            {
                EntityReferenceHolderImplementor targetEntityViewComponent = other.gameObject.GetComponent<EntityReferenceHolderImplementor>();
                if (targetEntityViewComponent != null)
                    hitChange.value = new AmmoBoxCollisionData(targetEntityViewComponent.reference, false);
            }
        }
    }
}
