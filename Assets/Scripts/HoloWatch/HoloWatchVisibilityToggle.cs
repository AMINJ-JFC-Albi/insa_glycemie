using UnityEngine;
using UnityEngine.InputSystem;

namespace HoloWatch {
    public class HoloWatchVisibilityToggle : MonoBehaviour{
        [SerializeField] private InputActionReference controllerActionButton;
        private Canvas canvas;
        
        private void Start() {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false;
        }
        
        private void OnEnable() {
            controllerActionButton.action.performed += ToggleHoloWatch;
        }

        private void OnDisable() {
            controllerActionButton.action.performed -= ToggleHoloWatch;
        }

        private void ToggleHoloWatch(InputAction.CallbackContext obj) {
            canvas.enabled = !canvas.enabled;
        }
    }
}
