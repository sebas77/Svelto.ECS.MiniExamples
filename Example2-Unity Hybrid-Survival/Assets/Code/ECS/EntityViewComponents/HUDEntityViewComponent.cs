using Svelto.ECS.Hybrid;

namespace Svelto.ECS.Example.Survive.HUD
{
    public struct HUDEntityViewComponent : IEntityViewComponent
    {
        public IAnimationComponent    HUDAnimator;
        public IDamageHUDComponent    damageImageComponent;
        public IHealthSliderComponent healthSliderComponent;
        public IScoreComponent        scoreComponent;
        public IAmmoComponent         ammoComponent;
        public IEnemiesLeftComponent  enemyleftComponent;
        public IWaveComponent         wavComponent;         
        public EGID                   ID { get; set; }
    }
}