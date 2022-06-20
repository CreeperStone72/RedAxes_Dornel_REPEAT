using Norsevar.AI;
using UnityEngine;

namespace Norsevar.UI
{
    public class AiHealthListener : HealthBarListener
    {

        #region Serialized Fields

        [SerializeField] private HealthBehaviour healthBehaviour;

        #endregion

        #region Unity Methods

        protected override void OnEnable()
        {
            healthBehaviour.OnInit += HandleOnInit;
        }

        protected override void OnDisable()
        {
            healthBehaviour.OnInit -= HandleOnInit;
            healthBehaviour.HealthResource.OnCurrentValueChanged -= OnValueChanged;
        }

        #endregion

        #region Private Methods

        private void HandleOnInit()
        {
            healthBehaviour.HealthResource.OnCurrentValueChanged += OnValueChanged;
            MaxHealth = healthBehaviour.HealthResource.BaseStatDefinition.Cap;
        }

        #endregion

        #region Protected Methods

        protected override void OnValueChanged(float value)
        {
            base.OnValueChanged(value);

            if (value <= 0)
                Destroy(transform.parent.gameObject);
        }

        #endregion

    }
}
