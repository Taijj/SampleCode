using UnityEngine;

namespace Taijj.SampleCode
{

	/// <summary>
	/// Support Object that shows hints and Tutorials for the Player
	/// </summary>
	public class HintTotem : Interactor
	{
		#region LifeCycle
		[Space, Header("Totem")]
		[SerializeField, TextArea] private string[] _hints;
		[SerializeField] private UiHint _hintUi;
		[SerializeField] private Animator _animator;
		[Space, Header("Input")]
		[SerializeField] private UiMap _uiMap;

		private int _hintIndex;



		public override void Wake()
		{
			base.Wake();
			_hintUi.Deactivate();

			_hintIndex = 0;
			_animator.Randomize();

			_uiMap.OnSubmit += OnSubmit;
		}

		public override void CleanUp()
		{
			base.CleanUp();
			_uiMap.OnSubmit -= OnSubmit;
		}
		#endregion



		#region Interactor
		protected override void Exit()
		{
			base.Exit();
			IsHintOpen = false;
		}

		protected override void Interact()
		{
			IsHintOpen = true;

			_hintUi.Activate();

			Game.Input.PushMap(_uiMap);
			ShowNextHint();
		}
		#endregion



		#region Hint Logic
		private void ShowNextHint()
		{
			_hintUi.ShowHint(_hints[_hintIndex]);
			_hintUi.SetHintPageCounter(_hintIndex + 1, _hints.Length);
			_hintIndex++;
		}

		private void OnSubmit()
		{
			if (!IsHintOpen)
				return;

			if (_hintIndex < _hints.Length)
			{
				ShowNextHint();
			}
			else
			{
				_hintIndex = 0;
				_hintUi.Deactivate();

				Game.Input.PopMap();

				IsHintOpen = false;
				Complete();
			}
		}

		private bool IsHintOpen { get; set; }
		protected override bool IsInteractive => false == IsHintOpen;
		#endregion
	}
}
