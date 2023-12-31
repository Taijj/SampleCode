using UnityEngine;

namespace Taijj.SampleCode
{
	public class LoadingScreenSimple : UiLoadingScreen
	{
		[SerializeField] private LoadingScreenKind _kind;

		protected override void OnShown()
		{
			base.OnShown();
			OnReady?.Invoke();
		}

		public override LoadingScreenKind Kind => _kind;
	}
}
