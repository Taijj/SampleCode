using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;

namespace Taijj.Input
{
    [CreateAssetMenu(fileName = "Playstation", menuName = "Ble/Input/Peripherals/Playstation")]
    public class Playstation : Peripheral
    {
        public override bool Includes(InputDevice device)
        {
            #if UNITY_PS4 || UNITY_PS5
                return true;
            #else
                return device is DualShockGamepad;
            #endif
        }

        protected override void PreparePadMotors()
        {
            #if UNITY_PS5 && false == UNITY_EDITOR
                int index = ((DualSenseGamepad)Pad).slotIndex;
                PS5Input.PadSetVibrationMode(index, PS5Input.VibrationMode.Compatible);
            #else
                base.PreparePadMotors();
            #endif
        }

        protected override void SetPadMotors(float intensity)
        {
            #if UNITY_PS5 && false == UNITY_EDITOR
                int index = ((DualSenseGamepad)Pad).slotIndex;
                int intens = (int)(intensity * RumbleMultiplier * 200f);
                PS5Input.PadSetVibration(index, intens, intens);
            #else
                base.SetPadMotors(intensity);
            #endif
        }
    }
}