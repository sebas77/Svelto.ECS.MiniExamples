using Svelto.ECS.Example.Survive.HUD;
using Svelto.ECS.Hybrid;
using UnityEngine;
using UnityEngine.UI;

namespace Svelto.ECS.Example.Survive.Implementors.HUD
{
    public class AnnouncementHUDImplementor : MonoBehaviour, IImplementor, IAnnouncementHUDComponent
    {
        public Color TargetColor = new Color(1f, 1f, 1f, 0.5f);

        public float speed { get; set; }
        public Color targetColor { get; set; }
        public Color textColor { get => _text.color; set => _text.color = value; }

        void Awake() { _text = GetComponent<Text>(); }

        Text _text;
    }
}
