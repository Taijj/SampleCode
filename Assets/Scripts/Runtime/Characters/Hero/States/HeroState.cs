using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Base for a state the <see cref="Hero"/>
	/// can be in.
	/// </summary>
	public abstract class HeroState : MonoBehaviour
	{
		#region LifeCycle
		public struct StateData
		{
			public PawnPhysical pawn;
			public HeroArsenal arsenal;
			public InputProvider input;
			public HeroInvul invul;
		}

		public virtual void Wake(StateData data)
		{
			Type = GetType();
			Pawn = data.pawn;
			Arsenal = data.arsenal;
			Input = data.input;
			Invul = data.invul;
		}

		public virtual void CleanUp() { }



		public Type Type { get; private set; }
		protected PawnPhysical Pawn { get; private set; }
		protected HeroArsenal Arsenal { get; private set; }
		protected InputProvider Input { get; private set; }
		protected HeroInvul Invul { get; private set; }
		#endregion



		#region Flow
		public virtual void Enter() { }
		public virtual void OnUpdate() { }
		public virtual void OnFixedUpdate() { }
		public virtual void Exit() { }

		protected void Transit(Type targetStateType) => OnTransit?.Invoke(targetStateType);

		public event Action<Type> OnTransit;
		#endregion



		#region Shoot & Breathe
		private static readonly int SHOOT_HASH = Animator.StringToHash("Shoot");
		private static readonly int IS_BREATHING_HASH = Animator.StringToHash("IsBreathing");

		protected void ShootFire()
		{
			if(Arsenal.TryShootFire())
				Pawn.Animator.Trigger(SHOOT_HASH, false);
		}

		protected void ShootIce()
		{
			if(Arsenal.TryShootIce())
				Pawn.Animator.Trigger(SHOOT_HASH, false);
		}

		protected void UpdateBreathing()
		{
			Arsenal.BreathRotationInput = Input.BreathDirection;
			Arsenal.IsBreathingFire = Input.IsBreathingFire;
			Arsenal.IsBreathingIce = Input.IsBreathingIce;
			Pawn.Animator.Set(IS_BREATHING_HASH, Arsenal.IsBreathing);
		}
		#endregion
	}
}