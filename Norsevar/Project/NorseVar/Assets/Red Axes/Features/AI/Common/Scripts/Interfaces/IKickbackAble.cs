using UnityEngine;

namespace Norsevar.AI
{

    public interface IKickbackAble
    {

        #region Public Methods

        void Kickback(Vector3 dirToTarget, float? damageInfo);

        #endregion

    }

}
