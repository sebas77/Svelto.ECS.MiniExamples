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

    public interface IWaveDataComponent
    {
        int wave { set; get; }
        int enemies { set; get; }
    }

    public interface IAnnouncementHUDComponent
    {
        float speed { get; set; }
        Color textColor { get; set; }
        Color targetColor { get; set; }
    }

    public interface IAmmoComponent
    {
        int ammo { set; get; }
    }
}