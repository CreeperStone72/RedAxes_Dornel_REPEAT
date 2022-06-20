using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Norsevar
{

    public class PlayerInputs : Singleton<PlayerInputs>
    {

        #region Constants and Statics

        private static bool _isUsingGamepad;

        #endregion

        #region Delegates and Events

        public static event Action<bool> OnUsingGamepadChanged;

        #endregion

        #region Private Fields

        private readonly Queue<Action> _attackActionsToConsume = new();
        private readonly Dictionary<string, Action> _attackActions = new();
        private readonly Queue<Action> _dashActionsToConsume = new();
        private readonly Dictionary<string, Action> _dashActions = new();
        private readonly Queue<Action> _chargeCancelToConsume = new();
        private readonly Dictionary<string, Action> _chargeCancel = new();
        private readonly Queue<Action> _chargeStartToConsume = new();
        private readonly Dictionary<string, Action> _chargeStart = new();
        private readonly Queue<Action> _specialPerformedToConsume = new();
        private readonly Dictionary<string, Action> _specialPerformed = new();
        private readonly Queue<Action> _defensiveStartToConsume = new();
        private readonly Dictionary<string, Action> _defensiveStart = new();
        private readonly Queue<Action> _defensiveCancelToConsume = new();
        private readonly Dictionary<string, Action> _defensiveCancel = new();

        private PlayerInputActions _inputs;

        #endregion

        #region Properties

        public static bool IsUsingGamepad
        {
            get => _isUsingGamepad;
            private set
            {
                if (value != _isUsingGamepad)
                    OnUsingGamepadChanged?.Invoke(value);

                _isUsingGamepad = value;
            }
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            _inputs.Player.Enable();
            _inputs.Player.Attack.performed += HandleAttack;
            _inputs.Player.Dash.performed += HandleDash;
            _inputs.Player.ChargeAttack.started += HandleChargeStart;
            _inputs.Player.ChargeAttack.canceled += HandleChargeCancel;
            _inputs.Player.SpecialAttack.performed += HandleSpecialPerformed;
            _inputs.Player.DefensiveAttack.started += HandleDefensiveStart;
            _inputs.Player.DefensiveAttack.canceled += HandleDefensiveCancel;
        }

        private void OnDisable()
        {
            _inputs.Player.Disable();
            _inputs.Player.Attack.performed -= HandleAttack;
            _inputs.Player.Dash.performed -= HandleDash;
            _inputs.Player.ChargeAttack.started -= HandleChargeStart;
            _inputs.Player.ChargeAttack.canceled -= HandleChargeCancel;
            _inputs.Player.SpecialAttack.performed -= HandleSpecialPerformed;
            _inputs.Player.DefensiveAttack.started -= HandleDefensiveStart;
            _inputs.Player.DefensiveAttack.canceled -= HandleDefensiveCancel;
        }

        #endregion

        #region Private Methods

        private static void AddAction(Action action, string key, bool consume, Queue<Action> queue, IDictionary<string, Action> dictionary)
        {
            if (consume)
            {
                queue.Enqueue(action);
                return;
            }

            if (dictionary.ContainsKey(key)) return;
            dictionary.Add(key, action);
        }

        private void HandleAttack(InputAction.CallbackContext obj)
        {
            InvokeEvent(_attackActionsToConsume, _attackActions, obj);
        }

        private void HandleChargeCancel(InputAction.CallbackContext obj)
        {
            InvokeEvent(_chargeCancelToConsume, _chargeCancel, obj);
        }

        private void HandleChargeStart(InputAction.CallbackContext obj)
        {
            InvokeEvent(_chargeStartToConsume, _chargeStart, obj);
        }

        private void HandleDash(InputAction.CallbackContext obj)
        {
            Analytics.AddDash();
            InvokeEvent(_dashActionsToConsume, _dashActions, obj);
        }

        private void HandleDefensiveCancel(InputAction.CallbackContext obj)
        {
            InvokeEvent(_defensiveCancelToConsume, _defensiveCancel, obj);
        }

        private void HandleDefensiveStart(InputAction.CallbackContext obj)
        {
            InvokeEvent(_defensiveStartToConsume, _defensiveStart, obj);
        }

        private void HandleSpecialPerformed(InputAction.CallbackContext obj)
        {
            Analytics.AddGroundSlam();
            InvokeEvent(_specialPerformedToConsume, _specialPerformed, obj);
        }

        private static void InvokeEvent(Queue<Action> actions, Dictionary<string, Action> dictionary, InputAction.CallbackContext ctx)
        {
            if (actions.Count > 0)
            {
                Action action = actions.Dequeue();
                action.Invoke();
                return;
            }

            foreach (KeyValuePair<string, Action> keyValuePair in dictionary)
                keyValuePair.Value.Invoke();

            IsUsingGamepad = ctx.control.device is Gamepad;
        }

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
            _inputs = new PlayerInputActions();
        }

        #endregion

        #region Public Methods

        public void AddAttack(Action action, string key = "", bool consume = false)
        {
            AddAction(action, key, consume, _attackActionsToConsume, _attackActions);
        }

        public void AddAttack(Action action, bool consume)
        {
            AddAttack(action, "", consume);
        }

        public void AddChargeCancel(Action action, string key = "", bool consume = false)
        {
            AddAction(action, key, consume, _chargeCancelToConsume, _chargeCancel);
        }

        public void AddDash(Action action, string key = "", bool consume = false)
        {
            AddAction(action, key, consume, _dashActionsToConsume, _dashActions);
        }

        public void AddDefensiveCancel(Action action, string key = "", bool consume = false)
        {
            AddAction(action, key, consume, _defensiveCancelToConsume, _defensiveCancel);
        }

        public void AddSpecialPerformed(Action action, string key = "", bool consume = false)
        {
            AddAction(action, key, consume, _specialPerformedToConsume, _specialPerformed);
        }

        public InputAction GetMouse()
        {
            return _inputs.Player.Mouse;
        }

        public InputAction GetMovement()
        {
            return _inputs.Player.Movement;
        }

        public PlayerInputActions.PlayerActions GetPlayerActions()
        {
            return _inputs.Player;
        }

        public void PopAttack()
        {
            if (_attackActionsToConsume.Count <= 0) return;
            _attackActionsToConsume.Dequeue();
        }

        public void RemoveAttack(string key = "")
        {
            _attackActions.Remove(key);
        }

        public void RemoveChargeCancel(string key)
        {
            _chargeCancel.Remove(key);
        }

        public void RemoveDash(string key)
        {
            _dashActions.Remove(key);
        }

        public void RemoveDefensiveCancel(string key)
        {
            _defensiveCancel.Remove(key);
        }

        #endregion

    }

}
