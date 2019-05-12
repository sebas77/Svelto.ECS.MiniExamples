using System;

namespace Svelto.ECS
{
    public struct DynamicEntityDescriptor<TType>:IEntityDescriptor where TType : IEntityDescriptor, new()
    {
        public DynamicEntityDescriptor(IEntityBuilder[] extraEntities)
        {
            DBC.ECS.Check.Require(extraEntities.Length > 0,
                                  "don't use a DynamicEntityDescriptorInfo if you don't need to use extra EntityViews");

            var defaultEntities = EntityDescriptorTemplate<TType>.descriptor.entitiesToBuild;
            var length = defaultEntities.Length;

            entitiesToBuild = new IEntityBuilder[length + extraEntities.Length + 1];

            Array.Copy(defaultEntities, 0, entitiesToBuild, 0, length);
            Array.Copy(extraEntities, 0, entitiesToBuild, length, extraEntities.Length);

            var builder = new EntityBuilder<EntityStructInfoView>
            {
                _initializer = new EntityStructInfoView 
                { 
                    entitiesToBuild = entitiesToBuild,
                    type = typeof(TType)
                }
            };
            entitiesToBuild[entitiesToBuild.Length - 1] = builder;
        }

        public IEntityBuilder[] entitiesToBuild { get; }
    }
}