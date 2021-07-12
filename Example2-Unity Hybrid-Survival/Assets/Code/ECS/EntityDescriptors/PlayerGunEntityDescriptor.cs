using Svelto.ECS.Example.Survive.Player.Gun;
using Svelto.ECS.Example.Survive.Weapons;

namespace Svelto.ECS.Example.Survive.Player
{
    public class PlayerGunEntityDescriptor : GenericEntityDescriptor<GunEntityViewComponent, GunAttributesComponent, AmmoValueComponent>
    {
    }
}