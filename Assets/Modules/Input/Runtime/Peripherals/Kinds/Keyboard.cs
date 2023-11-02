using UnityEngine.InputSystem;
using UnityEngine;

namespace Taijj.Input
{
    [CreateAssetMenu(fileName = "Keyboard", menuName = "Ble/Input/Peripherals/Keyboard")]
    public class Keyboard : Peripheral
    {
        public override bool Includes(InputDevice device)
        {
            return device is UnityEngine.InputSystem.Keyboard;
        }
    }
}