
using System;
using UnityEngine.InputSystem;

namespace Taijj.Input
{
    /// <summary>
    /// Handles device changes and selects the respective <see cref="Peripheral"/>
    /// depending on the last used input device.
    /// </summary>
    public class PeripheralSwitcher
    {
        #region LifeCycle
        public PeripheralSwitcher(Peripheral[] supported) => All = supported;

        public void SetUp(Action onPeripheralChanged)
        {
            OnChanged = onPeripheralChanged;

            InputSystem.onDeviceChange += OnDeviceChange;
            InputSystem.onActionChange += OnActionChange;
            SetDefaultPeripheral();
        }

        public void CleanUp()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
            InputSystem.onActionChange -= OnActionChange;
        }



        public Peripheral Current { get; private set; }
        private Peripheral[] All { get; set; }
        #endregion



        #region Unity Device Change
        private void OnActionChange(object action, InputActionChange change)
        {
            if (change != InputActionChange.ActionStarted)
                return;

            if ((action is InputAction) == false)
                return;

            InputAction input = action as InputAction;
            InputDevice device = input.activeControl.device;
            if(Current.Device != device)
                OnDeviceChange(device, InputDeviceChange.UsageChanged);
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (device is Mouse)
                return;

            if (IsRemoval(change))
            {
                OnDeviceRemoved();
                return;
            }

            if (Current.Device != device)
                SetPeripheralBy(device);
        }



        private bool IsRemoval(InputDeviceChange change)
        {
            return change == InputDeviceChange.Removed
               || change == InputDeviceChange.Removed
               || change == InputDeviceChange.Disabled
               || change == InputDeviceChange.Disconnected;
        }

        private void OnDeviceRemoved()
        {
            foreach (InputDevice device in InputSystem.devices)
            {
                if (device is Mouse)
                    continue;
                OnDeviceChange(device, InputDeviceChange.UsageChanged);
                break;
            }
        }
        #endregion



        #region Peripheral
        private void SetDefaultPeripheral() => Current = All[0];

        private void SetPeripheralBy(InputDevice device)
        {
            SetDefaultPeripheral();
            for(int i = 0; i < All.Length; i++)
            {
                Peripheral peri = All[i];
                if (peri.Includes(device))
                {
                    peri.Device = device;
                    Current = peri;
                    break;
                }
            }
            OnChanged?.Invoke();
        }

        private Action OnChanged { get; set; }
        #endregion
    }
}