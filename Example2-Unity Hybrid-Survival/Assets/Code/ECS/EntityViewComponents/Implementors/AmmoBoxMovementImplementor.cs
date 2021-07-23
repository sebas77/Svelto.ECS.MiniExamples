using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.AmmoBox
{
    public class AmmoBoxMovementImplementor : MonoBehaviour, IImplementor, IPositionComponent
    {
        public Vector3 position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        void Awake()
        {
            _transform = transform;
        }

        Transform _transform;
    }
}