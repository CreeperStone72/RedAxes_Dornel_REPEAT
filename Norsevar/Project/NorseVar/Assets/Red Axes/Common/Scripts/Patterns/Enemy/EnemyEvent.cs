using System;
using Norsevar.Currencies;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{

    public struct EnemyData
    {
        public int id;
        public CurrencyType currencyType;
        public int amount;
        public Vector3 position;
    }

    [CreateAssetMenu(fileName = "EnemyEvent", menuName = "Norsevar/Events/EnemyData")]
    public class EnemyEvent : BaseGameEvent<EnemyData>
    {
    }

    [Serializable]
    public class UnityEnemyEvent : UnityEvent<EnemyData>
    {
    }

}
