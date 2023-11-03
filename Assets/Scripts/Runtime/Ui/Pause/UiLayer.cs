using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Base for all Ui Layers
	/// </summary>
	public abstract class UiLayer : MonoBehaviour
	{
		#region LifeCycle
		[SerializeField] private UiButton _default;



		public abstract void Open();

		public abstract void Close();
		#endregion



		#region Logic
		public void Deselect(UiButton selectableButton)
		{
			selectableButton.Deselect();
		}

		public void Select(UiButton selectableButton)
		{
			selectableButton.ForceSelect();
		}

		public void ActivateDefault()
		{
			_default?.ForceSelect();
		}
		#endregion
	}
}