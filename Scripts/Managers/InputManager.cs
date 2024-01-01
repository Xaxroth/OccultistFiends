using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public enum ButtonMap {Interact, JumpOrAim, PullBack, RotateLeft, RotateRight, Shout, Spit}
    public enum ButtonState : byte {None, Pressed, Hold}
    public enum PlayerDevice{Keyboard, Controller}

    public struct InputData
    {
        private Vector2 _primaryVectorInput;
        private Vector2 _aimVectorInput;

        private byte _interactInput;
        private byte _cancelInput;
        private byte _pullInput;
        private byte _rotateLeftInput;
        private byte _rotateRightInput;
        private byte _shoutInput;
        private byte _spitInput;

        private float _interactInputTime;
        private float _cancelInputTime;
        private float _pullInputTime;
        private float _rotateLeftInputTime;
        private float _rotateRightInputTime;
        private float _shoutInputTime;
        private float _spitInputTime;
        public void SetPrimaryVectorInput(Vector2 input) => _primaryVectorInput = input;
        
        public Vector2 GetPrimaryVectorInput() => _primaryVectorInput;
        
        public void SetAimVectorInput(Vector2 input) => _aimVectorInput = input;
        public Vector2 GetAimVectorInput() => _aimVectorInput;

        public void SetButtonState(ButtonState state, ButtonMap button)
        {
            switch (button)
            {
                case ButtonMap.Interact: _interactInput = (byte)state;
                    _interactInputTime = Time.time; break;
                case ButtonMap.JumpOrAim: _cancelInput = (byte)state;
                    _cancelInputTime = Time.time; break;
                case ButtonMap.PullBack: _pullInput = (byte)state;
                    _pullInputTime = Time.time; break;
                case ButtonMap.RotateLeft: _rotateLeftInput = (byte)state;
                    _rotateLeftInputTime = Time.time; break;
                case ButtonMap.RotateRight: _rotateRightInput = (byte)state;
                    _rotateRightInputTime = Time.time; break;
                case ButtonMap.Shout: _shoutInput = (byte)state;
                    _shoutInputTime = Time.time; break;
                case ButtonMap.Spit: _spitInput = (byte)state;
                    _spitInputTime = Time.time; break;
            }
        }
        
        public ButtonState GetButtonState(ButtonMap button)
        {
            switch (button)
            {
                case ButtonMap.Interact: return (ButtonState)_interactInput;
                case ButtonMap.JumpOrAim: return (ButtonState)_cancelInput;
                case ButtonMap.PullBack: return (ButtonState)_pullInput;
                case ButtonMap.RotateLeft: return (ButtonState)_rotateLeftInput;
                case ButtonMap.RotateRight: return (ButtonState)_rotateRightInput;
                case ButtonMap.Shout: return (ButtonState)_shoutInput;
                case ButtonMap.Spit: return (ButtonState)_spitInput;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
        
        public float GetPressTime(ButtonMap button)
        {
            switch (button)
            {
                case ButtonMap.Interact: return _interactInputTime;
                case ButtonMap.JumpOrAim: return _cancelInputTime;
                case ButtonMap.PullBack: return _pullInputTime;
                case ButtonMap.RotateLeft: return _rotateLeftInputTime;
                case ButtonMap.RotateRight: return _rotateRightInputTime;
                case ButtonMap.Shout: return _shoutInputTime;
                case ButtonMap.Spit: return _spitInputTime;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
        
        /// <summary>
        ///   <para>Checks Button Press within a timewindow</para>
        /// </summary>
        public bool CheckButtonPressTime(ButtonMap button, float timeWindow, ButtonState state = ButtonState.Pressed)
        {
            return (GetButtonState(button) == state && Math.Abs(GetPressTime(button) - Time.time) < timeWindow);
        }
        
        /// <summary>
        ///   <para>Check if button is pressed this frame</para>
        /// </summary>
        public bool CheckButtonPress(ButtonMap button)
        {
            return CheckButtonPressTime(button, .001f, ButtonState.Pressed);
        }
        
        /// <summary>
        ///   <para>Check if button is held down</para>
        /// </summary>
        public bool CheckButtonHold(ButtonMap button)
        {
            return GetButtonState(button) is ButtonState.Pressed or ButtonState.Hold;
        }
    }

    public class InputManager : ManagerInstance<InputManager>
    {
        private InputActions _player1Inputs;
        private InputActions _player2Inputs;
        private InputData _inputDataP1;
        private InputData _inputDataP2;
        private Gamepad _player1Gamepad;
        private Gamepad _player2Gamepad;
        private Coroutine _player1RumbleQueue;
        private Coroutine _player2RumbleQueue;

        private static PlayerDevice P1Device = PlayerDevice.Keyboard;
        private static PlayerDevice P2Device = PlayerDevice.Keyboard;

        public static bool UseRumble = true;

        public delegate void UIBackAction();
        public static event UIBackAction OnUIBack;

        public delegate void UIMenuAction();
        public static event UIMenuAction OnUIMenu;

        public void BindDevices()
        {
            InputSystem.FlushDisconnectedDevices();

            int gamepadCount = Gamepad.all.Count;

            switch (gamepadCount)
            {
                case 0:
                    P1Device = PlayerDevice.Keyboard;
                    P2Device = PlayerDevice.Keyboard;
                    break;
                case 1:
                    P1Device = PlayerDevice.Controller;
                    P2Device = PlayerDevice.Keyboard;
                    break;
                default:
                    P1Device = PlayerDevice.Controller;
                    P2Device = PlayerDevice.Controller;
                    break;
            }

            bool p1IsController = P1Device == PlayerDevice.Controller;
            bool p2IsController = P2Device == PlayerDevice.Controller;

            var player1Device = p1IsController
                ? new InputDevice[] { Gamepad.all[0], Keyboard.current }
                : new InputDevice[] { Keyboard.current };
            var player2Device = p2IsController
                ? new InputDevice[] { Gamepad.all[gamepadCount - 1], Keyboard.current }
                : new InputDevice[] { Keyboard.current };

            if(p1IsController)_player1Gamepad = Gamepad.all[0];
            if(p2IsController)_player2Gamepad = Gamepad.all[gamepadCount - 1];

            var keyboard1 = _player1Inputs.KeyboardScheme.bindingGroup;
            var keyboard2 = _player1Inputs.Keyboard2Scheme.bindingGroup;
            var gamepad = _player1Inputs.ControllerScheme.bindingGroup;
            

            _player1Inputs.asset.bindingMask = InputBinding.MaskByGroups(keyboard1, gamepad);
            _player2Inputs.asset.bindingMask = InputBinding.MaskByGroups(keyboard2, gamepad);

            _player1Inputs.devices = player1Device;
            _player2Inputs.devices = player2Device;
        }

        private void OnEnable()
        {
            _player1Inputs ??= new InputActions();
            _inputDataP1 = new InputData();
            _player2Inputs ??= new InputActions();
            _inputDataP2 = new InputData();

            InputSystem.onDeviceChange += (_, change) =>
            {
                if (change is InputDeviceChange.Added or InputDeviceChange.Reconnected or InputDeviceChange.Disconnected
                    or InputDeviceChange.Removed or InputDeviceChange.Enabled or InputDeviceChange.Disabled)
                    BindDevices();
            };
            
            BindActions();
            BindDevices();
        }

        private void BindActions()
        {
            var actions = _player1Inputs.Player;

            _player1Inputs.UI.Cancel.started += _ => OnUIBack?.Invoke();
            _player1Inputs.UI.Menu.started += _ => OnUIMenu?.Invoke();
            
            _player2Inputs.UI.Cancel.started += _ => OnUIBack?.Invoke();
            _player2Inputs.UI.Menu.started += _ => OnUIMenu?.Invoke();
            
            _player1Inputs.UI.Enable();
            _player2Inputs.UI.Enable();

            actions.Move.performed += ctx => _inputDataP1.SetPrimaryVectorInput(ctx.ReadValue<Vector2>());
            actions.Move.canceled += _ => _inputDataP1.SetPrimaryVectorInput(Vector2.zero);
            
            actions.MoveAim.performed += ctx => _inputDataP1.SetAimVectorInput(ctx.ReadValue<Vector2>());
            actions.MoveAim.canceled += _ => _inputDataP1.SetAimVectorInput(Vector2.zero);
            
            actions.Interact.started += _ => _inputDataP1.SetButtonState(ButtonState.Pressed, ButtonMap.Interact);
            actions.Interact.performed += _ => _inputDataP1.SetButtonState(ButtonState.Hold, ButtonMap.Interact);
            actions.Interact.canceled += _ => _inputDataP1.SetButtonState(ButtonState.None, ButtonMap.Interact);
            
            actions.AimMode.started += _ => _inputDataP1.SetButtonState(ButtonState.Pressed, ButtonMap.JumpOrAim);
            actions.AimMode.performed += _ => _inputDataP1.SetButtonState(ButtonState.Hold, ButtonMap.JumpOrAim);
            actions.AimMode.canceled += _ => _inputDataP1.SetButtonState(ButtonState.None, ButtonMap.JumpOrAim);
            
            actions.Spit.started += _ => _inputDataP1.SetButtonState(ButtonState.Pressed, ButtonMap.Spit);
            actions.Spit.performed += _ => _inputDataP1.SetButtonState(ButtonState.Hold, ButtonMap.Spit);
            actions.Spit.canceled += _ => _inputDataP1.SetButtonState(ButtonState.None, ButtonMap.Spit);
            
            actions.Shout.started += _ => _inputDataP1.SetButtonState(ButtonState.Pressed, ButtonMap.Shout);
            actions.Shout.performed += _ => _inputDataP1.SetButtonState(ButtonState.Hold, ButtonMap.Shout);
            actions.Shout.canceled += _ => _inputDataP1.SetButtonState(ButtonState.None, ButtonMap.Shout);

            actions.RotateLeft.started += _ => _inputDataP1.SetButtonState(ButtonState.Pressed, ButtonMap.RotateLeft);
            actions.RotateLeft.performed += _ => _inputDataP1.SetButtonState(ButtonState.Hold, ButtonMap.RotateLeft);
            actions.RotateLeft.canceled += _ => _inputDataP1.SetButtonState(ButtonState.None, ButtonMap.RotateLeft);
            
            actions.RotateRight.started += _ => _inputDataP1.SetButtonState(ButtonState.Pressed, ButtonMap.RotateRight);
            actions.RotateRight.performed += _ => _inputDataP1.SetButtonState(ButtonState.Hold, ButtonMap.RotateRight);
            actions.RotateRight.canceled += _ => _inputDataP1.SetButtonState(ButtonState.None, ButtonMap.RotateRight);

            actions.Enable();
            
            actions = _player2Inputs.Player;
            
            actions.Move.performed += ctx => _inputDataP2.SetPrimaryVectorInput(ctx.ReadValue<Vector2>());
            actions.Move.canceled += _ => _inputDataP2.SetPrimaryVectorInput(Vector2.zero);
            
            actions.Interact.started += _ => _inputDataP2.SetButtonState(ButtonState.Pressed, ButtonMap.Interact);
            actions.Interact.performed += _ => _inputDataP2.SetButtonState(ButtonState.Hold, ButtonMap.Interact);
            actions.Interact.canceled += _ => _inputDataP2.SetButtonState(ButtonState.None, ButtonMap.Interact);
            
            actions.Jump.started += _ => _inputDataP2.SetButtonState(ButtonState.Pressed, ButtonMap.JumpOrAim);
            actions.Jump.performed += _ => _inputDataP2.SetButtonState(ButtonState.Hold, ButtonMap.JumpOrAim);
            actions.Jump.canceled += _ => _inputDataP2.SetButtonState(ButtonState.None, ButtonMap.JumpOrAim);
            
            actions.PullBack.started += _ => _inputDataP2.SetButtonState(ButtonState.Pressed, ButtonMap.PullBack);
            actions.PullBack.performed += _ => _inputDataP2.SetButtonState(ButtonState.Hold, ButtonMap.PullBack);
            actions.PullBack.canceled += _ => _inputDataP2.SetButtonState(ButtonState.None, ButtonMap.PullBack);

            actions.Enable();
        }

        private void OnDisable()
        {
            _player1Inputs.Player.Disable();
            _player2Inputs.Player.Disable();
            _player1Inputs.UI.Disable();
            _player2Inputs.UI.Disable();
            
            CancelRumble(0);
            CancelRumble(1);
        }

        public InputData GetInputData(byte player) => player == 0 ? _inputDataP1 : _inputDataP2;
        
        /// <summary>
        ///   <para>Sets rumble to the player's gamepad. Strengths are between 0 and 1. </para>
        /// </summary>
        public void StartRumble(byte playerIndex, float leftStrength, float rightStrength, float duration, bool cancelCurrent = true)
        {
            if(!UseRumble) return;
            
            bool p1IsController = P1Device == PlayerDevice.Controller;
            bool p2IsController = P2Device == PlayerDevice.Controller;

            if (playerIndex == 0 && cancelCurrent && _player1RumbleQueue != null)
            {
                StopCoroutine(_player1RumbleQueue);
                _player1RumbleQueue = null;
            }
            else if (cancelCurrent && _player2RumbleQueue != null)
            {
                StopCoroutine(_player2RumbleQueue);
                _player2RumbleQueue = null;
            }
            
            if(playerIndex == 0 ? _player1RumbleQueue != null || !p1IsController : _player2RumbleQueue != null || !p2IsController) return;
            
            var gamepad = playerIndex == 0 ? _player1Gamepad : _player2Gamepad;
            
            gamepad.SetMotorSpeeds(leftStrength, rightStrength);

            if (playerIndex == 0) _player1RumbleQueue = StartCoroutine(StopRumble(playerIndex, duration));
            else _player2RumbleQueue = StartCoroutine(StopRumble(playerIndex, duration));
        }

        public void CancelRumble(byte playerIndex)
        {
            bool p1IsController = P1Device == PlayerDevice.Controller;
            bool p2IsController = P2Device == PlayerDevice.Controller;
            
            if(playerIndex == 0 ? _player1RumbleQueue == null || !p1IsController : _player2RumbleQueue == null || !p2IsController) return;

            if (playerIndex == 0)
            {
                StopCoroutine(_player1RumbleQueue);
                _player1RumbleQueue = null;
                _player1Gamepad.SetMotorSpeeds(0, 0);
            }
            else
            {
                StopCoroutine(_player2RumbleQueue);
                _player2RumbleQueue = null;
                _player2Gamepad.SetMotorSpeeds(0, 0);
            }
        }

        IEnumerator StopRumble(byte playerIndex, float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            if (playerIndex == 0)
            {
                StopCoroutine(_player1RumbleQueue);
                _player1RumbleQueue = null;
                _player1Gamepad.SetMotorSpeeds(0, 0);
            }
            else
            {
                StopCoroutine(_player2RumbleQueue);
                _player2RumbleQueue = null;
                _player2Gamepad.SetMotorSpeeds(0, 0);
            }
        }
    }
}
