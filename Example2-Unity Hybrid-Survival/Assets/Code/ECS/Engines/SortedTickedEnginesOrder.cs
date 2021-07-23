using Svelto.Common;

namespace Svelto.ECS.Example.Survive
{
    public enum EnginesNames
    {
        SurvivalUnsortedEngines
      , UpdateScoreEngine
       , UpdateAmmoEngine
       , DamageUnsortedEngines
        , AmmoBoxTriggerEngine
        , UpdateEnemiesLeftEngine
    }

    public enum PlayerEnginesNames
    {
       PlayerDeathEngine
      , PlayerGunShootingEngine
    }

    public enum EnemyEnginesNames
    {
        EnemyAttackEngine
      , EnemyDeathEngine
      , EnemySpawnEffectOnDamage
    }

    public struct SortedTickedEnginesOrder : ISequenceOrder
    {
        public string[] enginesOrder => new[]
        {
            nameof(EnginesNames.SurvivalUnsortedEngines)
          , nameof(PlayerEnginesNames.PlayerGunShootingEngine)
          , nameof(EnginesNames.UpdateAmmoEngine)
          , nameof(EnemyEnginesNames.EnemyAttackEngine)
          , nameof(EnemyEnginesNames.EnemySpawnEffectOnDamage)
          , nameof(EnginesNames.DamageUnsortedEngines)
          , nameof(EnemyEnginesNames.EnemyDeathEngine)
          , nameof(EnginesNames.UpdateEnemiesLeftEngine)
          , nameof(PlayerEnginesNames.PlayerDeathEngine)
          , nameof(EnginesNames.UpdateScoreEngine)
          , nameof(EnginesNames.AmmoBoxTriggerEngine)
        };
    }
}