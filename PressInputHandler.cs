using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HyperTools
{
    /*
     * This handles both Mouse and Touch inputs!
     * (Let's call them Presses for simplicity.)
     *
     * To use this bad boy, just add it to the root GameObject you want to pick up raycast Presses from,
     * then just add listeners for any of the 3 Press events.
     *
     * Treat the Interactable property like you would on a UI element, so it can be disabled as needed for gane logic.
     *
     * The Target property is optional, and can be used to raycast Presses on Graphics have that Target GameObject as an
     * ancestor. This is so you can have a PressInputHandler on a totally different GameObject for organization reasons,
     * or for working easily with presses on other GameObjects created at runtime.
     */
    public class PressInputHandler : MonoBehaviour
    {
        public bool Interactable;
        public GameObject Target;

        private bool _isInteracting;
        private float _interactionDuration;

        private Camera _mainCamera;

        public event Action<Vector3> OnPressStart; // <input position>
        public event Action<Vector3, float> OnPressHeld; // <input position, held duration in seconds>
        public event Action<Vector3> OnPressEnd; // <input position>

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (!Interactable)
            {
                return;
            }

            // Detect the start of interaction
            if (IsInputStart(out Vector3 inputPosition))
            {
                if (IsOverUIElement(inputPosition) || IsOverGameObject(inputPosition))
                {
                    _isInteracting = true;
                    _interactionDuration = 0f; // Reset duration
                    OnPressStart?.Invoke(inputPosition);
                }
            }
            // Detect ongoing interaction
            else if (IsInputHold(out inputPosition))
            {
                if (_isInteracting)
                {
                    _interactionDuration += Time.deltaTime;
                    OnPressHeld?.Invoke(inputPosition, _interactionDuration);
                }
            }
            else if (IsInputEnd(out inputPosition))
            {
                if (_isInteracting)
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

            if (Input.GetMouseButtonDown(0))
            {
                inputPosition = Input.mousePosition;
                return true;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                inputPosition = Input.GetTouch(0).position;
                return true;
            }

            return false;
        }

        // Check if the input is still held (mouse hold or touch stationary/moved)
        private bool IsInputHold(out Vector3 inputPosition)
        {
            inputPosition = Vector3.zero;

            if (Input.GetMouseButton(0))
            {
                inputPosition = Input.mousePosition;
                return true;
            }

            if (Input.touchCount > 0)
            {
                inputPosition = Input.GetTouch(0).position;
                var touchPhase = Input.GetTouch(0).phase;
                return touchPhase == TouchPhase.Stationary || touchPhase == TouchPhase.Moved;
            }

            return false;
        }

        // Check if the input has ended (mouse release or touch ended)
        private bool IsInputEnd(out Vector3 inputPosition)
        {
            inputPosition = Vector3.zero;

            if (Input.GetMouseButtonUp(0))
            {
                inputPosition = Input.mousePosition;
                return true;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                inputPosition = Input.GetTouch(0).position;
                return true;
            }

            return false;
        }

        // Check if the input started over this GameObject (using GraphicRaycaster)
        private bool IsOverUIElement(Vector2 inputPosition)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = inputPosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);

            var ancestor = Target ?? gameObject;
            if (results.Count > 0 && Utilities.HasAncestorGameObject(results[0].gameObject, ancestor))
            {
                return true;
            }

            return false;
        }

        // Check if the input started over this GameObject (using raycasting)
        private bool IsOverGameObject(Vector3 inputPosition)
        {
            if (_mainCamera == null)
            {
                return false;
            }

            Ray ray = _mainCamera.ScreenPointToRay(inputPosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                return hit.transform == transform;
            }

            return false;
        }
    }
}
