using UnityEngine.InputSystem;
using UnityEngine;

namespace Taijj.Input
{
    [CreateAssetMenu(fileName = "Switch", menuName = "Ble/Input/Peripherals/Switch")]
    public class Switch : Peripheral
    {
        public override bool Includes(InputDevice device)
        {
            #if UNITY_SWITCH
                return true;
            #else
                return false;
            #endif
        }
    }
}