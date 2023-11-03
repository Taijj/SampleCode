using UnityEngine.InputSystem;
using UnityEngine;
using Taijj.Input;
using System;

namespace Taijj.SampleCode
{
    [CreateAssetMenu(fileName = "UiMap", menuName = "Ble/Input/Maps/Ui")]
    public class UiMap : ActionMap<InputActions>
    {
        #region Lifecycle
        public override void Wake(object actions)
        {
            base.Wake(actions);
            Actions = Base.Ui;
            AddListeners();
        }

        public override void CleanUp() => RemoveListeners();

        public override bool IsEnabled
        {
            get => Actions.enabled;

            set
            {
                if (value)
                    Actions.Enable();
                else
                    Actions.Disable();
            }
        }

        private InputActions.UiActions Actions { get; set; }
        #endregion



        #region Events
        private void AddListeners()
        {
			Actions.Submit.performed += OnSubmitInput;
			Actions.Cancel.performed += OnCancelInput;
			Actions.Close.performed += OnCloseInput;
        }

        private void RemoveListeners()
        {
			Actions.Submit.performed -= OnSubmitInput;
			Actions.Cancel.performed -= OnCancelInput;
			Actions.Close.performed -= OnCloseInput;
		}

		private void OnSubmitInput(InputAction.CallbackContext _) => OnSubmit?.Invoke();
		private void OnCancelInput(InputAction.CallbackContext _) => OnCancel?.Invoke();
		private void OnCloseInput(InputAction.CallbackContext _) => OnClose?.Invoke();

		public event Action OnSubmit;
		public event Action OnCancel;
		public event Action OnClose;
        #endregion
    }
}
