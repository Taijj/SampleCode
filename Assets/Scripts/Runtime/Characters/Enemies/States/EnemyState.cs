using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Base for a state the <see cref="Enemy"/> can be in.
	/// </summary>
	public abstract class EnemyState : MonoBehaviour
	{
		#region LifeCycle
		public struct StateData
		{
			public Enemy owner;
			public Pawn pawn;
			public HeroDetector detector;

			public bool turnsTowardsHero;
			public Func<bool> hasAggro;
		}

		public virtual void Wake(StateData data)
		{
			Type = GetType();
			Owner = data.owner;
			Pawn = data.pawn;
			Detector = data.detector;

			TurnsTowardsHero = data.turnsTowardsHero;
			HasAggro = data.hasAggro;
		}

		public virtual void SetUp() { }
		public virtual void CleanUp() { }



		public Type Type { get; private set; }

		protected Enemy Owner { get; private set; }
		protected Pawn Pawn { get; private set; }
		protected HeroDetector Detector { get; private set; }

		protected bool TurnsTowardsHero { get; private set; }
		protected Func<bool> HasAggro { get; private set; }
		#endregion



		#region Flow
		public virtual void Enter() {}
		public virtual void OnUpdate() {}
		public virtual void OnFixedUpdate() {}
		public virtual void Exit() {}

		protected void Transit(Type targetStateType) => OnTransit?.Invoke(targetStateType);

		public event Action<Type> OnTransit;
		#endregion



		#region Helpers
		protected bool TryTransitDueToDetection(bool isDetectable, EnemyState resultingState)
		{
			if (false == isDetectable)
				return false;

			if (resultingState.IsNull())
				return false;

			bool isWrongFacing = !(TurnsTowardsHero && Detector.IsFacingHero);
			if (isWrongFacing)
				return false;

			Transit(resultingState.GetType());
			return true;
		}

		protected bool TryTransitDueToAggro(EnemyState resultingState)
		{
			if (false == HasAggro())
				return false;

			if (resultingState.IsNull())
				return false;

			Transit(resultingState.GetType());
			return true;
		}
		#endregion
	}
}