using System;
using System.Collections;
using System.Collections.Generic;
using Dev.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static Dev.Input.InputActions;

namespace Dev.Input
{
    public enum DeviceType
    {
        Keyboard = 0,
        Gamepad = 1
    }

    public class InputSystem: IGameplayActions
    {
        public Action<Vector2> OnMovementCall;
        public Action<Vector2> OnAimingCall;
        public Action OnInteractCall;
        public Action OnHerdStateCall;
        public Action<bool> OnHerdTargetCall;
        public Action OnMenuCall;
        public Action<bool> OnSlingOrThrowCall;

        public Action<DeviceType> OnDeviceTypeChanged;
        private DeviceType _currentDevice;

        private InputActions _inputActions = new InputActions();

        private Dictionary<string, Coroutine> _holdProcesses = new Dictionary<string, Coroutine>();

        public DeviceType CurrentDevice { get => _currentDevice; }

        public InputSystem()
        {
            _inputActions.Gameplay.SetCallbacks(this);

#if UNITY_EDITOR || UNITY_STANDALONE
            _currentDevice = DeviceType.Keyboard;
#else
            _currentDevice = DeviceType.Gamepad;
#endif
            SetEnableGameplayMap(true);
        }

        public void SwitchEnableMaps()
        {
            SetEnableGameplayMap(!_inputActions.Gameplay.enabled);
            SetEnableUIMap(!_inputActions.Gameplay.enabled);
        }

        public void SetEnableGameplayMap(bool enable)
        {
            if (_inputActions.Gameplay.enabled == enable) return;
            
            if (enable) _inputActions.Gameplay.Enable();
            else _inputActions.Gameplay.Disable();
        }
        
        public void SetEnableUIMap(bool enable)
        {
            if (_inputActions.UI.enabled == enable) return;
            
            if (enable) _inputActions.UI.Enable();
            else _inputActions.UI.Disable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            CheckDevice(context);
            OnMovementCall?.Invoke(context.ReadValue<Vector2>());
            //Debug.Log($"OnMovement: {context.ReadValue<Vector2>()}");
        }

        public void OnAiming(InputAction.CallbackContext context)
        {
            CheckDevice(context);
            OnAimingCall?.Invoke(Vector2.ClampMagnitude(context.ReadValue<Vector2>(), 1f));
            //Debug.Log($"OnAiming: {context.ReadValue<Vector2>()}");
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            CheckDevice(context);
            OnInteractCall?.Invoke();
            Debug.Log($"OnInteract");
        }

        public void OnHerdState(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            CheckDevice(context);
            OnHerdStateCall?.Invoke();
            Debug.Log($"OnHerdState");
        }

        public void OnHerdTarget(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            CheckDevice(context);
            StartHoldProcess("HerdTarget", OnHerdTargetCall);
        }

        public void OnMenu(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            CheckDevice(context);
            OnMenuCall?.Invoke();
            Debug.Log($"OnMenu");
        }

        public void OnSlingOrThrow(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            CheckDevice(context);
            StartHoldProcess("SlingOrThrow", OnSlingOrThrowCall);
        }

        private void StartHoldProcess(string actionName, Action<bool> actionToCall)
        {
            if (_holdProcesses.ContainsKey(actionName))
            {
                SingletoneServer.Instance.Get<CoroutinesSystem>().EndCouroutine(_holdProcesses[actionName]);
                _holdProcesses.Remove(actionName);
            }

            Coroutine holdProcess = SingletoneServer.Instance.Get<CoroutinesSystem>().BeginCoroutine(YIELD_HoldProcess(actionName, actionToCall));
            _holdProcesses.Add(actionName, holdProcess);
        }

        private IEnumerator YIELD_HoldProcess(string actionName, Action<bool> actionToCall)
        {
            InputAction action = _inputActions.FindAction(actionName);
            actionToCall?.Invoke(true);
            Debug.Log($"{actionName} started");

            while (action.IsPressed())
            {
                yield return null;
            }
            
            actionToCall?.Invoke(false);
            Debug.Log($"{actionName} completed");
        }

        private void CheckDevice(InputAction.CallbackContext context)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            DeviceType lastUsedDevice = CurrentDevice;
            if (context.control.device.description.deviceClass == "Gamepad")
                _currentDevice = DeviceType.Gamepad;
            else
                _currentDevice = DeviceType.Keyboard;
                
            if (lastUsedDevice != _currentDevice)
                OnDeviceTypeChanged?.Invoke(_currentDevice);
#endif
        }
    }
}