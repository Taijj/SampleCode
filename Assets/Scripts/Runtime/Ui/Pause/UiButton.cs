using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Button with underline and Cursor position to signify Selection State
	/// </summary>
	public class UiButton : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private Button _button;
		[SerializeField] private Image _selectionLine;
		[SerializeField] private Image _cursor;
		[Space]
		[SerializeField] private EventReference _selectSound;
		[SerializeField] private EventReference _submitSound;
		[SerializeField] private EventReference _cancelSound;
		[SerializeField] private EventReference _invalidSound;

		public void Wake() => Deselect();
		public void CleanUp() => _button.onClick.RemoveAllListeners();

		public void AddListener(UnityAction action) => _button.onClick.AddListener(action);
		public void RemoveListener(UnityAction action) => _button.onClick.RemoveListener(action);
		#endregion



		#region Selection
		/// <summary>
		/// Selects this Button's <see cref="Selectable"/> silently.
		/// </summary>
		public void ForceSelect()
		{
			_isSelectionSoundSkipped = true;
			_button.Select();
		}
				
		/// <summary>
		/// Meant to be called by the <see cref="Selectable"/>'s OnSelect event.
		/// Includes graphical updates and sound.
		/// </summary>
		public void Select()
		{
			_selectionLine.Activate();
			_cursor.Activate();

			if(false == _isSelectionSoundSkipped)
				Game.Audio.Play(_selectSound);
			_isSelectionSoundSkipped = false;
		}

		private bool _isSelectionSoundSkipped;
		#endregion



		#region Misc Unity Events
		public void Deselect()
		{
			_selectionLine.Deactivate();
			_cursor.Deactivate();
		}

		public void PlaySubmitSound() => Game.Audio.Play(_button.IsInteractable() ? _submitSound : _cancelSound);
		public void PlayCancelSound() => Game.Audio.Play(_cancelSound);		
		#endregion
	}
}
