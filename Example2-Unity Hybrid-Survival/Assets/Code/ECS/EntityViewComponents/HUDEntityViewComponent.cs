using Svelto.ECS.Hybrid;

namespace Svelto.ECS.Example.Survive.HUD
{
    public struct HUDEntityViewComponent : IEntityViewComponent
    {
        public IAnimationComponent       HUDAnimator;
        public IDamageHUDComponent       damageImageComponent;
        public IHealthSliderComponent    healthSliderComponent;
        public IScoreComponent           scoreComponent;
        public IWaveDataComponent        waveDataComponent;
        public IAnnouncementHUDComponent announcementHUDComponent;
        public IAmmoComponent            ammoComponent;
        public EGID                   ID { get; set; }
    }
}