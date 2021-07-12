using UnityEngine;
using System.Collections;

namespace Svelto.ECS.Example.Survive.Weapons
{
    public class AmmoVisualEngine : IQueryingEntitiesEngine, IStepEngine
    {

        public AmmoVisualEngine()
        {
        }

        public EntitiesDB entitiesDB { private get; set; }

        public void Ready() { 
            _mainTick = MainTick();
        }

        public void Step() { 
            _mainTick.MoveNext();
        }
        public string name => nameof(AmmoSpawnerEngine);

        IEnumerator MainTick()
        {

            void Rotate()
            {
                foreach (var ((ammo, ammoCount), _) in entitiesDB.QueryEntities<AmmoEntityViewComponent>(
                        AmmoActive.Groups))
                {
                    for (var i = 0; i < ammoCount; ++i) 
                        ammo[i].ammoComponent.rotation *= Quaternion.Euler(Vector3.up * 1);
                }

            }


            while (true)
            {
                Rotate();

                yield return null;
            }
        }

        IEnumerator                  _mainTick;
    }
}