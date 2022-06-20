using UnityEngine;

namespace Norsevar.Interaction
{
    public class DropItems : SpawnItems
    {
        [SerializeField] [Range(0f, 1f)] private float dropChance;

        protected override void AttemptSpawnItems(SpawnCondition condition)
        {
            if(Random.Range(0f, 1f) <= dropChance)
                base.AttemptSpawnItems(condition);
        }
    }
}