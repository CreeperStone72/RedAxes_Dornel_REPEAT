using UnityEngine;

namespace Norsevar.Status_Effect_System
{
    public abstract class BaseEffectData : ScriptableGameObject
    {

        #region Private Fields

        private float _remainingDuration;

        #endregion

        #region Protected Fields

        protected IEffectable effectable;

        #endregion

        #region Serialized Fields

        [SerializeField] private EStatusEffectType type;
        [SerializeField] protected float duration;
        [SerializeField] [Range(1, 10)] private int maxStack = 1;
        [SerializeField] private EAuraType auraType;
        [SerializeField] private Sprite sprite;

        #endregion

        #region Properties

        public EStatusEffectType Type => type;
        public bool IsActive => _remainingDuration > 0f;
        public int StackCount { get; private set; }
        public EAuraType AuraType => auraType;
        public Sprite Sprite => sprite;

        #endregion

        #region Protected Methods

        protected abstract void DisableEffect();

        protected abstract void EnableEffect();

        #endregion

        #region Public Methods

        public virtual void AddStack(BaseEffectData data)
        {
            if (StackCount + 1 >= maxStack)
                _remainingDuration = data.duration;
            else
                StackCount++;
        }

        public void Destroy()
        {
            DisableEffect();
            Destroy(this);
        }

        public void Initialize(IEffectable pEffectable)
        {
            _remainingDuration = duration;
            effectable = pEffectable;
            EnableEffect();
        }

        public virtual void TickEffect()
        {
            if (_remainingDuration > 0)
                _remainingDuration -= Time.deltaTime;
        }

        #endregion

    }
}
