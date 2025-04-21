using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace HyperTools
{
    /*
     * This handles both Mouse and Touch inputs!
     * (Let's call them Presses for simplicity.)
     *
     * To use this bad boy, just add listeners for any of the 3 Press events.
     *
     * Treat the interactable property like you would on a UI element, so it can be disabled as needed for game logic.
     *
     */
    public class PressInputHandler : MonoBehaviour
    {
        public bool interactable;

        private bool _isInteracting;
        private float _interactionDuration;

        public event Action<Vector3> OnPressStart; // <input position>
        public event Action<Vector3, float> OnPressHeld; // <input position, held duration in seconds>
        public event Action<Vector3> OnPressEnd; // <input position>

        private void Update()
        {
            if (!interactable)
            {
                return;
            }

            // Detect the start of interaction
            if (IsInputStart(out Vector3 inputPosition))
            {
                _isInteracting = true;
                _interactionDuration = 0f; // Reset duration
                OnPressStart?.Invoke(inputPosition);
            }
            // Detect ongoing interaction
            else if (_isInteracting)
            {
                if  (IsInputHold(out inputPosition))
                {
                    _interactionDuration += Time.deltaTime;
                    OnPressHeld?.Invoke(inputPosition, _interactionDuration);
                }
                else if (IsInputEnd(out inputPosition))
                {
                    _isInteracting = false;
                    OnPressEnd?.Invoke(inputPosition);
                }
            }
        }

        // Check for the start of interaction (click down or touch began)
        private bool IsInputStart(out Vector3 inputPosition)
        {
            inputPosition = Vector3.zero;

            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                inputPosition = Pointer.current.position.ReadValue();
                return true;
            }
            
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            return false;
        }

        // Check if the input is still held (mouse hold or touch stationary/moved)
        private bool IsInputHold(out Vector3 inputPosition)
        {
            inputPosition = Vector3.zero;

            if (Pointer.current != null && Pointer.current.press.isPressed)
            {
                inputPosition = Pointer.current.position.ReadValue();
                return true;
            }

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                var touchPhase = Touchscreen.current.primaryTouch.phase.ReadValue();
                return touchPhase == TouchPhase.Stationary || touchPhase == TouchPhase.Moved;
            }

            return false;
        }

        // Check if the input has ended (mouse release or touch ended)
        private bool IsInputEnd(out Vector3 inputPosition)
        {
            inputPosition = Vector3.zero;

            if (Pointer.current != null && Pointer.current.press.wasReleasedThisFrame)
            {
                inputPosition = Pointer.current.position.ReadValue();
                return true;
            }

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
            {
                inputPosition = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }

            return false;
        }
    }
}
