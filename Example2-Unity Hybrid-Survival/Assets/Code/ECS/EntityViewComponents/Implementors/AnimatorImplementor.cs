using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.Implementors
{
    public class AnimatorImplementor : MonoBehaviour, IImplementor, IAnimationComponent
    {
        public string playAnimation
        {
            get => _animName;
            set
            {
                _animName = value;
                _anim.SetTrigger(value);
               
            }
        }

        //added a value to handle bool conditions for animation
        public string switchAnim
        {
            get => _animName;

            set 
            {
                _animName = value;
                _switchState = !_switchState;
                _anim.SetBool(value, _switchState);
            }
        }

        public AnimationState animationState { set => _anim.SetBool(value.name, value.state); }

        public bool reset
        {
            set
            {
                if (value) _anim.Rebind();
                _animName = string.Empty;
            }
        }

        void Awake() 
        { 
            _anim = GetComponent<Animator>();
            _switchState = false;
        }
        
        Animator _anim;
        string   _animName;
        bool     _switchState;
    }
}