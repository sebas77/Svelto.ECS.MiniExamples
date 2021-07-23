using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.Implementors.HUD
{
    public class EnemiesLeftHUDImplementor : MonoBehaviour, IImplementor, IEnemiesLeftComponent
    {

        public int enemiesLeft
        {
            get => _enemies;

            set
            {
                _enemies = value;
                _text.text = "Enemies: " + _enemies;
            }
        }

        void Awake()
        {
            _text = GetComponent<Text>();

            _enemies = 0;
            _text.text = "Enemies: " + _enemies;
        }

        int _enemies;
        Text _text;
    }


}