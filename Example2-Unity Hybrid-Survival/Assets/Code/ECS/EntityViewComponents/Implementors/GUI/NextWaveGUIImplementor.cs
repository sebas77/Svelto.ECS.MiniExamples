using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.Implementors.HUD
{
    public class NextWaveGUIImplementor : MonoBehaviour, IImplementor, IWaveComponent
    {

        public bool startNextWave
        {
            get => _nextWave;

            set
            {
                _nextWave = value;
                if (_nextWave == true)
                {
                    _text.text = "Wave " + _wave;
                    _wave++;
                }
            }
        }

        void Awake()
        {
            _text = GetComponent<Text>();
            _wave = 1;
        }

        bool _nextWave;
        int _wave;
        Text _text;
    }


}