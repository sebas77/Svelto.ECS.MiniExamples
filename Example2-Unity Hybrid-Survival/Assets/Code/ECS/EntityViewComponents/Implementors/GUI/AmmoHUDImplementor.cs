using Svelto.ECS.Example.Survive.HUD;
using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.Implementors.HUD
{
    public class AmmoHUDImplementor : MonoBehaviour, IImplementor, IAmmoComponent
    {

        public int ammoCount
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
            _text = GetComponent<Text>();

            _ammo = 100;
            _text.text = "Ammo: " + _ammo;
        }
        
        int _ammo;
        Text _text;
    }

    
}