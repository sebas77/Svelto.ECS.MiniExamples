using Svelto.ECS.Example.Survive.HUD;
using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.Implementors.HUD
{
    public class WaveManagerImplementor : MonoBehaviour, IImplementor, IWaveDataComponent
    {
        public int wave
        {
            get => _wave;
            set
            {
                _wave     = value;
                SetText();
            }
        }

        public int enemies
        {
            get => _enemies;
            set
            {
                _enemies = value;
                SetText();
            }
        }

        void SetText()
        {
            _text.text = "Wave: " + (_wave + 1) + "\nEnemies left: " + _enemies;
        }

        void Awake()
        {
            // Set up the reference.
            _text = GetComponent<Text>();

            _wave = 0;
            _enemies = 0;
        }

        int  _wave;
        int  _enemies;
        Text _text;
    }
}