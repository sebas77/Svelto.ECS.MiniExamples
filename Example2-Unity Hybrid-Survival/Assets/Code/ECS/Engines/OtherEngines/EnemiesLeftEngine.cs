using System.Collections;
using Svelto.Common;
using Svelto.ECS.Example.Survive.Enemies;

namespace Svelto.ECS.Example.Survive.HUD
{
    [Sequenced(nameof(EnginesNames.UpdateEnemiesLeftEngine))]
    public class UpdateEnemiesLeftEngine : IReactOnSwap<EnemyEntityViewComponent>, IQueryingEntitiesEngine, IStepEngine
    {
        public UpdateEnemiesLeftEngine()
        {
            //this should really be put in a json file to be accessed by the enemy spawner as well as this engine
            //prefarably, it should be stored in a struct only entity that also contains the counter current wave
            //i wasnt sure how to make a struct only entity and i also thought i was taking up to much time as it 
            //this is why both the enemy spawner and this engine do the next wave calculation instead of the enemy 
            //spawner indicating when to start next wave
            deadEnemies = 0;
            NUMBER_OF_ENEMIES_TO_SPAWN[0] = 3;
            NUMBER_OF_ENEMIES_TO_SPAWN[1] = NUMBER_OF_ENEMIES_TO_SPAWN[0] * 2;
            NUMBER_OF_ENEMIES_TO_SPAWN[2] = NUMBER_OF_ENEMIES_TO_SPAWN[0] * 3;
            NUMBER_OF_ENEMIES_TO_SPAWN[3] = NUMBER_OF_ENEMIES_TO_SPAWN[1] * 2;
            NUMBER_OF_ENEMIES_TO_SPAWN[4] = NUMBER_OF_ENEMIES_TO_SPAWN[1] * 3;
            NUMBER_OF_ENEMIES_TO_SPAWN[5] = NUMBER_OF_ENEMIES_TO_SPAWN[2] * 2;
            NUMBER_OF_ENEMIES_TO_SPAWN[6] = NUMBER_OF_ENEMIES_TO_SPAWN[2] * 3;

            currentWave = 0;
            startNextWave = false;
            
        }
        public EntitiesDB entitiesDB { get; set; }
        public void Ready()
        {
            _listenForEnemyDeath = ListenForEnemyDeath();
        }
        public void Step()
        {
            _listenForEnemyDeath.MoveNext();
        }
        public string name => nameof(UpdateEnemiesLeftEngine);
        public void MovedTo
         (ref EnemyEntityViewComponent entityViewComponent, ExclusiveGroupStruct previousGroup, EGID egid)
        {
            if (egid.groupID.FoundIn(Enemies.DeadEnemies.Groups))
            {
                deadEnemies++;
                hudEntityView.enemyleftComponent.enemiesLeft--;
            }
        }


        IEnumerator ListenForEnemyDeath()
        {
           

            while (entitiesDB.HasAny<HUDEntityViewComponent>(ECSGroups.GUICanvas) == false)
                yield return null;

            hudEntityView = entitiesDB.QueryUniqueEntity<HUDEntityViewComponent>(ECSGroups.GUICanvas);
            hudEntityView.enemyleftComponent.enemiesLeft = NUMBER_OF_ENEMIES_TO_SPAWN[currentWave];
            while (true)
            {

                if (deadEnemies >= NUMBER_OF_ENEMIES_TO_SPAWN[currentWave])
                {
                    //only allow current wave to increment up to length of array
                    if (currentWave < NUMBER_OF_ENEMIES_TO_SPAWN.Length-1)
                        currentWave++;

                    WaitForSecondsEnumerator _waitForSeconds = new WaitForSecondsEnumerator(1);

                    while (_waitForSeconds.MoveNext())
                        yield return null;
                    //tell implemetor to increment wave number
                    hudEntityView.wavComponent.startNextWave = true;

                    //bool condition for animation set to true
                    hudEntityView.HUDAnimator.switchAnim = "NextWave";

                    _waitForSeconds.Reset(9);
                    
                    while (_waitForSeconds.MoveNext())
                        yield return null;
                    
                    //calling it again will set it to false
                    hudEntityView.HUDAnimator.switchAnim = "NextWave";

                    hudEntityView.enemyleftComponent.enemiesLeft = NUMBER_OF_ENEMIES_TO_SPAWN[currentWave];
                    deadEnemies = 0;

                }
                yield return null;
            }
        }


        int deadEnemies;
        IEnumerator _listenForEnemyDeath;
        int[] NUMBER_OF_ENEMIES_TO_SPAWN = new int[7];
        int currentWave;
        bool startNextWave;
        HUDEntityViewComponent hudEntityView;
    }
}