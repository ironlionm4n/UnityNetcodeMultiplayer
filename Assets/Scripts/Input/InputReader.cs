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
        
        public Vector2 AimPosition { get; private set; }
        
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
            if (context.performed)
            {
                var inputValue = context.ReadValue<Vector2>();
                Debug.Log(inputValue);
                PlayerMoving?.Invoke(inputValue);
            }
            else if (context.canceled)
            {
                PlayerMoving?.Invoke(Vector2.zero);
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            var mousePosition = context.ReadValue<Vector2>();
            AimPosition = mousePosition;
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
