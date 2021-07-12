using Svelto.ECS.Extensions.Unity;
using UnityEngine;

namespace Svelto.ECS.Example.Survive.HUD
{
    public class HudEntityDescriptor : GenericEntityDescriptor<HUDEntityViewComponent, WaveDataComponent>
    {
    }

    [DisallowMultipleComponent]
    public class HudEntityDescriptorHolder : GenericEntityDescriptorHolder<HudEntityDescriptor>
    {
    }
}