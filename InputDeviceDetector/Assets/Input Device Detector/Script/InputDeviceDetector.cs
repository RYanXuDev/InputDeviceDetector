using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace InputDeviceDetection
{    
    public class InputDeviceDetector : MonoBehaviour
    {
        [Header("Option"), Tooltip("只检测UI输入")]
        [SerializeField] bool detectUIInputOnly = true;

        [Space(10f)][Header("Device Switch Event Triggers")]
        [SerializeField] UnityEvent onSwitchToMouse = default;
        [SerializeField] UnityEvent onSwitchToKeyboard = default;
        [SerializeField] UnityEvent onSwitchToGamepad = default;

        InputDevice currentDevice;

        Mouse mouse;

        Keyboard keyboard;

        Gamepad gamepad;

        InputSystemUIInputModule UIInputModule;

        static InputDeviceDetector instance;

        public static UnityEvent OnSwitchToMouse => instance.onSwitchToMouse;
        public static UnityEvent OnSwitchToKeyboard => instance.onSwitchToKeyboard;
        public static UnityEvent OnSwitchToGamepad => instance.onSwitchToGamepad;

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

            if (detectUIInputOnly)
            {
                UIInputModule = FindObjectOfType<InputSystemUIInputModule>();

            #if UNITY_EDITOR
                if (UIInputModule == null)
                {
                    Debug.LogError("Can NOT find UI Input Module! Please check there is a Event System in the scene and is currently using the UI Input Module!");
                }
            #endif
            }
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
            if (detectUIInputOnly)
            {
                if (!UIInputModule.isActiveAndEnabled) return;
            }

            if (change == InputActionChange.ActionPerformed)
            {
                currentDevice = ((InputAction)obj).activeControl.device;

                if (currentDevice == mouse)
                {
                    onSwitchToMouse?.Invoke();
                }
                else
                {
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

        public static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
