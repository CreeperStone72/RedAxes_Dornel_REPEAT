using UnityEngine;
using UnityEngine.Rendering;

namespace Norsevar.Currencies
{

    public class CurrencyManager : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private SerializedDictionary<CurrencyType, Currency> currencies;

        #endregion

        #region Public Methods

        public void AddCurrency(EnemyData enemyData)
        {
            Analytics.AddMoney(enemyData.amount);
            currencies[enemyData.currencyType].AddCurrency(enemyData.amount);
        }

        #endregion

    }

}
