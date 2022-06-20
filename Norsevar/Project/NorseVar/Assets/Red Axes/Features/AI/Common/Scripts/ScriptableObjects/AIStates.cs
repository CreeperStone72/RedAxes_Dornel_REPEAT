using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    public abstract class AIStates : SerializedScriptableObject
    {

        #region Serialized Fields

        [Range(1, 30)] [SerializeField] private float aggressionRange;

        #endregion

        #region Properties

        public float AggressionRange => aggressionRange;

        #endregion

    }

}
