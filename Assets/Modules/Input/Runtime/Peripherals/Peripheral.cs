using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Taijj.Input
{
    /// <summary>
    /// Object to define and configure a supported input device. Contains
    /// device specific logic and graphical elements.
    /// </summary>
    public abstract class Peripheral : ScriptableObject
    {
        #region Main
        [SerializeField] private Sprite[] _iconSprites;
        [SerializeField] private TMP_SpriteAsset _iconsSpriteAsset;
        [SerializeField] private float _rumbleMultiplier = 1f;

        public abstract bool Includes(InputDevice device);
        #endregion



        #region Graphical
        private const string TAG_FORMAT = "<sprite name=\"{0}\">";

        public Sprite GetIcon(string buttonName)
        {
            for(int i = 0; i < _iconSprites.Length; i++)
            {
                if (_iconSprites[i].name == buttonName)
                    return _iconSprites[i];
            }
            return _iconSprites[0];
        }

        public string GetTag(string buttonName)
        {
            return string.Format(TAG_FORMAT, buttonName);
        }

        public TMP_SpriteAsset IconsSpriteAsset => _iconsSpriteAsset;
        #endregion



        #region Rumble
        private InputDevice _device;
        public InputDevice Device
        {
            get => _device;

            set
            {
                _device = value;
                Pad = _device is Gamepad ? _device as Gamepad : null;
                HasGamepad = Pad != null;
            }
        }

        public void PrepareMotors()
        {
            if (HasGamepad)
                PreparePadMotors();
        }
        protected virtual void PreparePadMotors() {}

        public void SetMotors(float intensity)
        {
            if(HasGamepad)
                SetPadMotors(intensity * RumbleMultiplier);
        }
        protected virtual void SetPadMotors(float intensity)
        {
            Pad.SetMotorSpeeds(intensity, intensity);
        }

        protected float RumbleMultiplier => _rumbleMultiplier;
        protected Gamepad Pad { get; private set; }
        public bool HasGamepad { get; private set; }
        #endregion
    }
}