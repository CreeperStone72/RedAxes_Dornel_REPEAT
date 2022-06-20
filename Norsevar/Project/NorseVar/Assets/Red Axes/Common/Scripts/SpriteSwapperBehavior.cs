using UnityEngine;

namespace Norsevar
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class SpriteSwapperBehavior : MonoBehaviour
    {
        [SerializeField]
        private Sprite _sprite1;
        [SerializeField]
        private Sprite _sprite2;

        [SerializeField] 
        private float _interval = 0.5f;

        private SpriteRenderer _spriteRenderer;
        private bool _flip;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            _spriteRenderer.sprite = _sprite1;
            _flip = false;
            this.ExecuteInSeconds(Show, _interval);
        }

        private void Show()
        {
            _spriteRenderer.sprite = _flip ? _sprite1 : _sprite2;
            _flip = !_flip;
            
            this.ExecuteInSeconds(Show, _interval);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
