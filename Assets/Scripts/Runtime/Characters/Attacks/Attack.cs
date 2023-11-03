
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Base for any type of attack any Character might make.
	/// </summary>
    public abstract class Attack : MonoBehaviour
    {
		#region LifeCycle
		[Space, Header("Base")]
		[SerializeField] private Transform _origin;

		public virtual void Wake() {}
		public virtual void CleanUp() {}

		public Transform Origin => _origin;
		#endregion



		#region Firing
		public virtual bool TryPerform()
		{
			if (false == CanPerform)
				return false;
			Perform();
			return true;
		}

		protected abstract void Perform();
		public virtual void Cease() {}

		public virtual bool CanPerform => true;
		protected int Facing => Origin.right.x.Sign();
		#endregion



		#if UNITY_EDITOR
		[Button("MoveToOrigin")]
		public bool buttonTarget;

		public void MoveToOrigin()
		{
			if (_origin.IsNull(true))
			{
				Note.LogWarning("Cannot move to origin, because it is null!");
				return;
			}
			transform.position = _origin.position;
		}
		#endif
	}
}