using System;
using UnityEngine;

namespace Taijj.Input
{
    /// <summary>
    /// Object to configure some behavior and support for this input system.
    /// </summary>
    [CreateAssetMenu(fileName = "InputModel", menuName = "Ble/Input/Model")]
    public class Model : ScriptableObject
    {
        #region Configuration
        [Space, Header("Main")]
        [SerializeField] private Peripheral[] _peripherals;
        [SerializeField] private string[] _anyActionExclusions;
        [SerializeField] private bool _isRumbleEnabled;

        [Space, Header("Rumble")]
        [SerializeField] private float _weakIntensity;
        [SerializeField] private float _mediumIntensity;
        [SerializeField] private float _strongIntensity;

        [Space, Header("Debug")]
        [SerializeField] private bool _logOperations;



        public Peripheral[] SupportedPeripherals => _peripherals;
        public string[] AnyActionExclusions => _anyActionExclusions;
        public bool IsRumbleEnabled
        {
            get => _isRumbleEnabled;
            set => _isRumbleEnabled = value;
        }

        public float WeakIntensity => _weakIntensity;
        public float MediumIntensity => _mediumIntensity;
        public float StrongIntensity => _strongIntensity;

        public bool LogOperations => _logOperations;
        #endregion



        #region InputActions
        public const string GENERATED_CLASS = "InputActions";
        public const string GENERATED_SCRIPT = "InputActions.cs";
        public const string INPUT_ASSET_FILE = "InputActions.inputactions";

        public static object InstantiateInputActions()
        {
            Type type = TypeAddons.Find(GENERATED_CLASS);
            return Activator.CreateInstance(type);
        }
        #endregion



        #if UNITY_EDITOR
        private static readonly string[] DEFAULT_ANY_ACTION_EXCLUSIONS = new string[]
        {
            "Mouse", "enter", "anyKey", "escape", "buttonEast", "buttonSouth",
            "acceleration", "gyro", "touch", "orientation", "angular"
        };

        public void AssignDefaults(Peripheral[] supportedPlatforms)
        {
            _peripherals = supportedPlatforms;
            _anyActionExclusions = DEFAULT_ANY_ACTION_EXCLUSIONS;

            _weakIntensity = 0.25f;
            _mediumIntensity = 0.5f;
            _strongIntensity = 0.75f;

            _isRumbleEnabled = true;
        }
        #endif
    }
}