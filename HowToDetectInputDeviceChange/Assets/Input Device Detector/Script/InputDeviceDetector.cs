using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace InputDeviceDetection
{    
    public class InputDeviceDetector : MonoBehaviour
    {
        public static UnityEvent OnSwitchToMouse => instance.onSwitchToMouse;
        public static UnityEvent OnSwitchToKeyboard => instance.onSwitchToKeyboard;
        public static UnityEvent OnSwitchToGamepad => instance.onSwitchToGamepad;

        [SerializeField] UnityEvent onSwitchToMouse;
        [SerializeField] UnityEvent onSwitchToKeyboard;
        [SerializeField] UnityEvent onSwitchToGamepad;

        static InputDeviceDetector instance;

        InputDevice currentDevice;

        Mouse mouse;

        Keyboard keyboard;

        Gamepad gamepad;

        void Awake()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }

            instance = this;
            mouse = Mouse.current;
            keyboard = Keyboard.current;
            gamepad = Gamepad.current;
            HideCursor();
        }

        void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;

            onSwitchToMouse?.RemoveAllListeners();
            OnSwitchToKeyboard?.RemoveAllListeners();
            OnSwitchToGamepad?.RemoveAllListeners();
        }

        void DetectCurrentInputDevice(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                currentDevice = ((InputAction)obj).activeControl.device;

                if (currentDevice == mouse)
                {
                    ShowCursor();
                    onSwitchToMouse?.Invoke();
                }
                else
                {
                    HideCursor();

                    if (currentDevice == keyboard)
                    {
                        onSwitchToKeyboard?.Invoke();
                    }

                    if (currentDevice == gamepad)
                    {
                        onSwitchToGamepad?.Invoke();
                    }
                }
            }
        }

        private static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}