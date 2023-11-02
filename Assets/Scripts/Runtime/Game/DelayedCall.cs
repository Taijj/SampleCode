
using System;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Cacheable, lightweight helper for delayed calls, that could be used as an alternative
	/// to Coroutines, Invokes or even DOTween's delayed calls.
	///
	/// Work in conjunction with <see cref="Updater"/>.
	/// </summary>
	public class DelayedCall
	{
		#region Main
		public DelayedCall(Action onCompleted, float delay = -1f)
		{
			OnCompleted = onCompleted;
			Delay = delay;
		}

		public void Start()
		{
			ElapsedTime = 0f;
			Game.Updater.Delay(this);
		}

		public void Restart(float newDelay = -1f)
		{
			Stop();
			if(newDelay >= 0f)
				Delay = newDelay;
			Start();
		}

		public void Stop() => Game.Updater.Cancel(this);
		#endregion



		#region Update
		public void SetUpdate(Action value)
		{
			Update = value;
			HasUpdate = value != null;
		}

		public void OnUpdate(float deltaTime)
		{
			ElapsedTime += deltaTime;

			if (ElapsedTime >= Delay)
			{
				Stop();
				OnCompleted();
			}
			else if (HasUpdate)
			{
				Update();
			}
		}
		#endregion



		#region Properties
		public float Delay { get; set; }
		private Action OnCompleted { get; set; }

		public float ElapsedTime { get; private set; }
		private bool HasUpdate { get; set; }
		private Action Update { get; set; }
		#endregion
	}
}