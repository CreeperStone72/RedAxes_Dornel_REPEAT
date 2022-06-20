using UnityEngine;

namespace Norsevar.AI
{

    [RequireComponent(typeof(WolfAIDataManager))]
    public class WolfHealthBehaviour : HealthBehaviour
    {

        #region Private Fields

        private EnemyEvent _onDeath;
        private int _id;

        #endregion

        #region Unity Methods

        protected override void Start()
        {
            base.Start();

            WolfAIDataManager aiDataManager = GetComponent<WolfAIDataManager>();
            _onDeath = aiDataManager.OnWolfDeath;
            _id = aiDataManager.Id;
        }

        #endregion

        #region Protected Methods

        protected override bool IsDead()
        {
            if (!base.IsDead()) return false;
            _onDeath.Raise(new EnemyData { id = _id, position = transform.position });
            return true;
        }

        #endregion

    }

}
