using Norsevar.Combat;
using UnityEditor;
using UnityEngine;

namespace Norsevar.Editor
{
    [CustomEditor(typeof(PlayerController))]
    public class TestPlayerControllerEditor : UnityEditor.Editor
    {

        #region Private Fields

        private PlayerCombatBehaviour _playerCombat;
        private float _fovRadius;
        private float _fovAngle;

        #endregion

        #region Unity Methods

        private void OnSceneGUI()
        {
            PlayerController pc = (PlayerController)target;
            _playerCombat = pc.Combat;

            if (!_playerCombat)
                return;

            if (_playerCombat.CurrentAttack != null)
            {
                _fovRadius = _playerCombat.CurrentAttack.Data.FovRadius;
                _fovAngle = _playerCombat.CurrentAttack.Data.FovAngle;
            }

            Vector3 pcPosition = pc.transform.position;

            //Draw Radius
            Handles.color = Color.white;
            Handles.DrawWireArc(pcPosition, Vector3.up, Vector3.forward, 360, _fovRadius);

            //Draw FOV Lines
            Handles.color = Color.green;
            Vector3 viewAngleA = DirFromAngle(pc, -_fovAngle / 2);
            Vector3 viewAngleB = DirFromAngle(pc, _fovAngle / 2);
            Handles.DrawLine(pcPosition, pcPosition + viewAngleA * _fovRadius);
            Handles.DrawLine(pcPosition, pcPosition + viewAngleB * _fovRadius);

        }

        #endregion

        #region Private Methods

        private static Vector3 DirFromAngle(Component pc, float angleInDegrees)
        {
            return pc.transform.rotation *
                   new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        #endregion

    }
}
