using UnityEngine.InputSystem;
using UnityEngine;
using Taijj.Input;
using System;

namespace Taijj.HeartWarming
{
    [CreateAssetMenu(fileName = "PresentationMap", menuName = "Ble/Input/Maps/Presentation")]
    public class PresentationMap : ActionMap<InputActions>
    {
		#region Lifecycle
		[SerializeField] private float _skipHoldDuration;

        public override void Wake(object actions)
        {
            base.Wake(actions);
            Actions = Base.Presentation;
			Any = UserInput.CreateAnyAction();
            AddListeners();
        }

        public override void CleanUp() => RemoveListeners();

        public override bool IsEnabled
        {
            get => Actions.enabled;

            set
            {
                if (value)
				{
                    Actions.Enable();
					Any.Action.Enable();
				}
                else
				{
                    Actions.Disable();
					Any.Action.Disable();
				}
            }
        }

		private AnyAction Any { get; set; }
        private InputActions.PresentationActions Actions { get; set; }
        #endregion



        #region Common Events
        private void AddListeners()
        {
			Actions.Next.started += OnNextPressInput;
			Actions.Next.canceled += OnNextReleaseInput;
			Actions.Skip.started += StartSkipping;
			Actions.Skip.canceled += CancelSkipping;

			Any.Action.started += OnAnyPressInput;
			Any.Action.canceled += OnAnyReleaseInput;
		}

        private void RemoveListeners()
        {
			Actions.Next.started -= OnNextPressInput;
			Actions.Next.canceled -= OnNextReleaseInput;
			Actions.Skip.started -= StartSkipping;
			Actions.Skip.canceled -= CancelSkipping;

			Any.Action.started -= OnAnyPressInput;
			Any.Action.canceled -= OnAnyReleaseInput;
		}

		private void OnNextPressInput(InputAction.CallbackContext _) => OnNextPress?.Invoke();
		private void OnNextReleaseInput(InputAction.CallbackContext _) => OnNextRelease?.Invoke();
		private void OnAnyPressInput(InputAction.CallbackContext _) => OnAnyPress?.Invoke();
		private void OnAnyReleaseInput(InputAction.CallbackContext _) => OnAnyRelease?.Invoke();

		public event Action OnNextPress;
		public event Action OnNextRelease;
		public event Action OnAnyPress;
		public event Action OnAnyRelease;
		#endregion



		#region Skipping
		public event Action OnSkipStarted;
		public event Action<float> UpdateSkipProgress;
		public event Action OnSkipCanceled;
		public event Action OnSkipCompleted;

		private void StartSkipping(InputAction.CallbackContext _)
		{
			if (IsSkipping)
				return;

			SkipTimeCounter = 0f;
			IsSkipping = true;
			UpdateSkipProgress?.Invoke(0f);
			OnSkipStarted?.Invoke();
		}

		public override void Update()
		{
			if (IsSkipping == false)
				return;

			SkipTimeCounter += Time.deltaTime;
			UpdateSkipProgress?.Invoke(SkipTimeCounter/ _skipHoldDuration);
			if (SkipTimeCounter >= _skipHoldDuration)
				CompleteSkipping();
		}

		private void CancelSkipping(InputAction.CallbackContext _)
		{
			if (IsSkipping == false)
				return;

			IsSkipping = false;
			UpdateSkipProgress?.Invoke(0f);
			OnSkipCanceled?.Invoke();
		}

		private void CompleteSkipping()
		{
			if (IsSkipping == false)
				return;

			IsSkipping = false;
			UpdateSkipProgress?.Invoke(1f);
			OnSkipCompleted?.Invoke();
		}

		private bool IsSkipping { get; set; }
		private float SkipTimeCounter { get; set; }
		#endregion
	}
}
