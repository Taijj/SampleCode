using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem.XInput;

namespace Taijj.Input
{
    [CreateAssetMenu(fileName = "Xbox", menuName = "Ble/Input/Peripherals/Xbox")]
    public class Xbox : Peripheral
    {
        public override bool Includes(InputDevice device)
        {
            #if UNITY_GAMECORE_XBOXSERIES
                return true;
            #else
                return device is XInputController;
            #endif
        }
    }
}