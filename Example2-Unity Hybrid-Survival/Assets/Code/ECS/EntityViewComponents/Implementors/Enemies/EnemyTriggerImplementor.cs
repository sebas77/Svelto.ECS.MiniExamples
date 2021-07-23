using Svelto.ECS.Extensions.Unity;
using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Enemies
{
    //Implementors act as bridge between Svelto.ECS Engines and third party platforms.
    //This feature is of fundamental importance to mix ECS with OOP libraries. If you need unity to communicate with the
    //Svelto Engines you don't need to use awkward workarounds, simply create an implementor as Monobehaviour.
    //In this way you could use, inside the implementor, Unity callbacks, like OnTriggerEnter/OnTriggerExit
    //and change data according the Unity callback. Logic should not be used inside these callback,
    //except setting entity components data.
    //This is the case when implementor can and should be Monobehaviours. Here an example:
    public class EnemyTriggerImplementor : MonoBehaviour, IImplementor, IEnemyTriggerComponent
    {
        public DispatchOnChange<EnemyCollisionData> hitChange { get; set; }

        /// <summary>
        /// it's annoying that there isn't a physic matrix dedicated for triggers. This means that OnTriggerEnter
        /// will be executed for any collider entering in the enemy trigger area, including the static ones  
        /// </summary>
        /// <param name="other"></param>
        void OnTriggerEnter(Collider other)
        {
            if (hitChange != null && other.attachedRigidbody != null)
            {
                EntityReferenceHolderImplementor enemyTargetEntityViewComponent = other.gameObject.GetComponent<EntityReferenceHolderImplementor>();
                if (enemyTargetEntityViewComponent != null)
                 hitChange.value = new EnemyCollisionData(enemyTargetEntityViewComponent.reference, true);
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (hitChange != null && other.attachedRigidbody != null)
            {
                EntityReferenceHolderImplementor enemyTargetEntityViewComponent = other.gameObject.GetComponent<EntityReferenceHolderImplementor>();
                if (enemyTargetEntityViewComponent != null)
                    hitChange.value = new EnemyCollisionData(enemyTargetEntityViewComponent.reference, false);
            }
        }
    }
}