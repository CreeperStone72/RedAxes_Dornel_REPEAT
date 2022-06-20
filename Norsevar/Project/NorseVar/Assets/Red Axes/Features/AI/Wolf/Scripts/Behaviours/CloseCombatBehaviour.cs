using GD.Selection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar.AI
{

    [RequireComponent(typeof(ISelector))] [RequireComponent(typeof(IRayProvider))]
    public class CloseCombatBehaviour : AttackBehaviour
    {

        #region Private Fields

        private IRayProvider _rayProvider;
        private ISelector _selector;
        private ISelectionResponse _selectionResponse;
        private Transform _currentSelection;

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("ray")] [SerializeField]
        private Transform rayOrigin;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _selector = GetComponent<ISelector>();
            _rayProvider = GetComponent<IRayProvider>();
            _selectionResponse = new HurtResponse(this);
        }

        #endregion

        #region Public Methods

        public override void Attack()
        {
            if (_currentSelection != null) _selectionResponse.OnDeselect(_currentSelection);
            _selector.Check(_rayProvider.CreateRay(rayOrigin), 0.1f, attackType.HittableLayers);
            _currentSelection = _selector.GetSelection();
            if (_currentSelection != null) _selectionResponse.OnSelect(_currentSelection);
        }

        #endregion

    }

}
