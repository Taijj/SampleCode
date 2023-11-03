
using System;

namespace Taijj.SampleCode
{
    public abstract class UiLoadingScreen : UiDisplay
    {
		public virtual void SetUp(Action onReady)
		{
			OnReady = onReady;
		}

		public abstract LoadingScreenKind Kind { get; }
		protected Action OnReady { get; private set; }
    }
}