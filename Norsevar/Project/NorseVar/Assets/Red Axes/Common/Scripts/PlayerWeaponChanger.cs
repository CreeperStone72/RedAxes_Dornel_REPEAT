using Norsevar.Combat;
using UnityEngine;

namespace Norsevar
{
    public class PlayerWeaponChanger : MonoBehaviour
    {
        [SerializeField] private PlayerCombatBehaviour playerCombatBehaviour;
        [SerializeField] private WeaponData axe;
        [SerializeField] private WeaponData hammer;
        [SerializeField] private WeaponData sword;

        public void EquipAxe()
        {
            playerCombatBehaviour.EquipWeapon(axe);
        }

        public void EquipHammer()
        {
            playerCombatBehaviour.EquipWeapon(hammer);
        }

        public void EquipSword()
        {
            playerCombatBehaviour.EquipWeapon(sword);
        }
    }
}