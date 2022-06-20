using MoreMountains.Tools;
using UnityEngine;

namespace Norsevar.UI
{
    public abstract class HealthBarListener : MonoBehaviour
    {

        #region Private Fields

        private MMProgressBar _healthBar;
        protected float MaxHealth;
        protected float CurrentHealth;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _healthBar = GetComponent<MMProgressBar>();
        }

        protected abstract void OnEnable();
        protected abstract void OnDisable();

        #endregion

        #region Protected Methods

        protected virtual void OnValueChanged(float value)
        {
            CurrentHealth = value;
            _healthBar.UpdateBar(value, 0, MaxHealth);
        }

        protected virtual void OnMaxHealthChanged(float oldValue, float newValue)
        {
            MaxHealth = newValue;
            _healthBar.UpdateBar(CurrentHealth, 0, newValue);
        }

        #endregion

    }


}
