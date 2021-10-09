using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace InputDeviceDetection
{    
    public class InputDeviceDetector : MonoBehaviour
    {
        [Header("Options")]
        [SerializeField] bool detectUIInputOnly = true;
        [SerializeField] bool hideCursorAtBeginning = false;

        [Space(10f)][Header("Device Switch Event Triggers")]
        [SerializeField] UnityEvent onSwitchToMouse = default;
        [SerializeField] UnityEvent onSwitchToKeyboard = default;
        [SerializeField] UnityEvent onSwitchToGamepad = default;

        Dictionary<InputDevice, UnityEvent> deviceSwitchTable = new Dictionary<InputDevice, UnityEvent>(); 

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
            if (instance == null)
            {
                instance = this;
            }

            mouse = Mouse.current;
            keyboard = Keyboard.current;
            gamepad = Gamepad.current;

            if (mouse != null) deviceSwitchTable.Add(mouse, OnSwitchToMouse);
            if (keyboard != null) deviceSwitchTable.Add(keyboard, onSwitchToKeyboard);
            if (gamepad != null) deviceSwitchTable.Add(gamepad, onSwitchToGamepad);

            if (hideCursorAtBeginning)
            {
                HideCursor();
            }

            UIInputModule = FindObjectOfType<InputSystemUIInputModule>(true);

        #if UNITY_EDITOR
            if (UIInputModule == null && detectUIInputOnly)
            {
                Debug.LogError("Can NOT find UI Input Module! Please check there is a Event System in the scene and is currently using the UI Input Module!");
            }
        #endif
        }

        void OnEnable()
        {
            InputSystem.onActionChange += DetectCurrentInputDevice;
        }

        void OnDisable()
        {
            InputSystem.onActionChange -= DetectCurrentInputDevice;

            onSwitchToMouse?.RemoveAllListeners();
            onSwitchToKeyboard?.RemoveAllListeners();
            onSwitchToGamepad?.RemoveAllListeners();
        }

        void DetectCurrentInputDevice(object obj, InputActionChange change)
        {
            if (detectUIInputOnly && !UIInputModule.isActiveAndEnabled) return;

            if (change == InputActionChange.ActionPerformed)
            {
                currentDevice = ((InputAction)obj).activeControl.device;
                deviceSwitchTable[currentDevice].Invoke();
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
