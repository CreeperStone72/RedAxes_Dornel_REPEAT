using UnityEngine;

namespace Norsevar.Combat
{
    public class DirectionMarker
    {
        private readonly Transform _owner;
        private readonly Camera _camera;
        private readonly PlayerMovementData _movementData;

        private bool _isActive = true;
        private Vector3 _mouseDirection;
        private readonly Transform _mousePositionMarker;

        public Vector3 MouseDirection => _mouseDirection;

        public DirectionMarker(Transform owner, Camera ownerCamera, PlayerMovementData movementData)
        {
            _owner = owner;
            _camera = ownerCamera;
            _movementData = movementData;

            if (_movementData.MousePositionMarkerPrefab && _movementData.MouseRaycastPlanePrefab)
            {
                _mousePositionMarker = Object.Instantiate(_movementData.MousePositionMarkerPrefab).transform;
                SetActive(false);

                Transform raycastPlane = Object.Instantiate(_movementData.MouseRaycastPlanePrefab).transform;
                raycastPlane.position = _owner.position;
                raycastPlane.parent = _owner;
                Vector3 localPos = raycastPlane.localPosition;
                localPos.y += movementData.MouseMarkerOffset.y;
                raycastPlane.localPosition = localPos;
            }
        }

        public void OnUpdateBasedOnMouse()
        {
            if(!_isActive)
                return;

            Vector2 mouseScreenPos = PlayerInputs.Instance.GetMouse().ReadValue<Vector2>();
            Ray mouseRay = _camera.ScreenPointToRay(mouseScreenPos);

            if (Physics.Raycast(mouseRay, out var hit, 100, _movementData.MousePositionLayerMask))
                UpdateMarker(hit.point);
        }

        public void OnUpdateBasedOnForward()
        {
            if(!_isActive)
                return;
            
            UpdateMarker(_owner.position + _owner.forward);
        }

        private void UpdateMarker(Vector3 position)
        {
            Vector3 ownerPosition = _owner.position;
            position.y = ownerPosition.y;

            _mouseDirection = (position - ownerPosition).normalized;
            _mousePositionMarker.position = ownerPosition + _movementData.MouseMarkerOffset + _mouseDirection * _movementData.MouseMarkerMaxDistance;
            _mousePositionMarker.forward = _mouseDirection;
        }
        
        public void SetActive(bool active)
        {
            _isActive = active;
            _mousePositionMarker.gameObject.SetActive(active);
            _mousePositionMarker.parent = active ? null : _owner;
        }
    }
}