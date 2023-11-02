using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;

namespace Taijj.Input
{
    /// <summary>
    /// Helper class to be able to react when the user gives
    /// any type of input.
    ///
    /// See <see cref="UserInput.CreateAnyAction"/> and
    /// <see cref="Model.AnyActionExclusions"/> to learn how
    /// to define inputs the any action should not react to.
    /// </summary>
    public class AnyAction
    {
		#region LifeCycle
		public AnyAction(string[] exclusions)
        {
            Exclusions = exclusions;
            Action = new InputAction();

			UserInput.OnDeviceChanged += Refresh;
            Refresh();
        }

		~AnyAction() => UserInput.OnDeviceChanged -= Refresh;

		public InputAction Action { get; set; }
		#endregion


		#region Refresh
		private void Refresh()
		{
			AddKeyboardControls();
			AddGamepadControls();

			if (Action.enabled)
				Action.Enable();
			else
				Action.Disable();
		}

		private void AddKeyboardControls()
        {
            InputAction action = new InputAction(binding: "/*/<button>", type: InputActionType.Button);
            AddToAnyAction(action.controls);
            action.Dispose();
        }

        private void AddGamepadControls()
        {
            InputAction action = new InputAction(binding: "/<Gamepad>/*/*", type: InputActionType.Button);
            action.AddBinding("/<Gamepad>/<button>");
            AddToAnyAction(action.controls);
            action.Dispose();
        }

        private void AddToAnyAction(ReadOnlyArray<InputControl> controls)
        {
            foreach (InputControl con in controls)
            {
                if (IsExcludedFromAnyAction(con))
                    continue;

                Action.AddBinding(con.path);
            }
        }

        private bool IsExcludedFromAnyAction(InputControl control)
        {
            foreach (string exclusion in Exclusions)
            {
                if (control.path.Contains(exclusion))
                    return true;
            }
            return false;
        }


        private string[] Exclusions { get; set; }
		#endregion
	}
}