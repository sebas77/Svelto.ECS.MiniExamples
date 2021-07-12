using Svelto.ECS.Example.Survive.HUD;
using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.Implementors.HUD
{
    public class AmmoHUDImplementor : MonoBehaviour, IImplementor, IAmmoComponent
    {
        public int ammo
        {
            get => _ammo;
            set
            {
                _ammo = value;
                _text.text = "Ammo: " + _ammo;
            }
        }

        void Awake()
        {
            // Set up the reference.
            _text = GetComponent<Text>();
        }

        int _ammo;
        Text _text;
    }
}