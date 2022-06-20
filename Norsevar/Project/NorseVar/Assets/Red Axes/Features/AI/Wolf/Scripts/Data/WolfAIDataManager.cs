using UnityEngine;

namespace Norsevar.AI
{

    public class WolfAIDataManager : AIDataManager
    {

        #region Serialized Fields

        [SerializeField] private EnemyEvent onWolfDeath;

        #endregion

        #region Properties

        public EnemyEvent OnWolfDeath => onWolfDeath;

        #endregion

    }

}
