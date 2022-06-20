using GD.Selection;
using UnityEngine;

namespace Norsevar
{

    public class SelectionManager : MonoBehaviour
    {

        #region Private Fields

        private IRayProvider _rayProvider;

        private ISelector _selector;

        private ISelectionResponse _selectionResponse;

        //currently selected game object
        private Transform _currentSelection;

        #endregion

        #region Unity Methods

        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            //get a ray provider
            _rayProvider = GetComponent<IRayProvider>();

            //get a selector
            _selector = GetComponent<ISelector>();

            //get a selection response
            _selectionResponse = GetComponent<ISelectionResponse>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            //set de-selected
            if (_currentSelection != null) _selectionResponse.OnDeselect(_currentSelection);

            //create/get ray
            _selector.Check(_rayProvider.CreateRay());

            //get current selection (cast ray, do tag comparison)
            _currentSelection = _selector.GetSelection();

            //set selected
            if (_currentSelection != null) _selectionResponse.OnSelect(_currentSelection);
        }

        #endregion

    }

}
