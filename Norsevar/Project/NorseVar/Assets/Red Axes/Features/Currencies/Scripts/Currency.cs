using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Currencies
{

    [CreateAssetMenu(fileName = "Currency", menuName = "Norsevar/Player/Currency")]
    public class Currency : ScriptableGameObject
    {

        #region Serialized Fields

        [SerializeField] [PropertyOrder(3)] [BoxGroup("Currency")]
        [HorizontalGroup("Currency/Row", 75, MarginLeft = 5, MarginRight = 5, LabelWidth = 130)]
        [PreviewField(75, ObjectFieldAlignment.Center)] [HideLabel]
        private Sprite icon;

        [SerializeField] [PropertyOrder(3)] [VerticalGroup("Currency/Row/Column")]
        private CurrencyType currencyType;

        [SerializeField] [PropertyOrder(3)] [VerticalGroup("Currency/Row/Column")]
        private int currentAmount;

        [SerializeField] [PropertyOrder(3)] [VerticalGroup("Currency/Row/Column")]
        private int maxAmount = 1000;

        [SerializeField] [PropertyOrder(3)] [VerticalGroup("Currency/Row/Column")]
        private int minAmount;

        #endregion

        #region Properties

        public int CurrentAmount => currentAmount;

        public int MaxAmount => maxAmount;

        public int MinAmount => minAmount;

        public CurrencyType CurrencyType => currencyType;

        public Sprite Icon => icon;

        #endregion

        #region Public Methods

        [Button("Add Value")] [HorizontalGroup("Currency/Bottom")] [PropertyOrder(3)]
        public void AddCurrency(int pAmount)
        {
            currentAmount += pAmount;

            if (currentAmount > MaxAmount)
                currentAmount = maxAmount;

            if (currentAmount < MinAmount)
                currentAmount = minAmount;
        }

        #endregion

    }

}
