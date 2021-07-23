using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
    public interface IDamageHUDComponent
    {
        float speed      { get; }
        Color flashColor { get; }
        Color imageColor { set; get; }
    }

    public interface IHealthSliderComponent
    {
        int value { set; }
    }

    public interface IScoreComponent
    {
        int score { set; get; }
    }

    public interface IAmmoComponent
    {
        int ammoCount { set; get; }
    }

   
}