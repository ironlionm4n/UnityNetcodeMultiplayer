using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(menuName = "Input/Input Reader", fileName = "NewInputReader")]
    public class InputReader : ScriptableObject, PlayerControls.IPlayerActions
    {
        public event Action<bool> PrimaryFireEvent;
        public event Action<Vector2> PlayerMoving;
        
        private PlayerControls _playerControls;
        private void OnEnable()
        {
            if (_playerControls == null)
            {
                _playerControls = new PlayerControls();
                _playerControls.Player.SetCallbacks(this);
            }
            
            _playerControls.Player.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var inputValue = context.ReadValue<Vector2>();
            PlayerMoving?.Invoke(inputValue);
        }

        public void OnPrimaryFire(InputAction.CallbackContext context)
        {
            // if nothing is listening to this event then an exception will be thrown, the ? character will check that there is at least one listener before invoking.
            if(context.performed)
                PrimaryFireEvent?.Invoke(true);
            else
            {
                PrimaryFireEvent?.Invoke(false);
            }
        }
    }
}
