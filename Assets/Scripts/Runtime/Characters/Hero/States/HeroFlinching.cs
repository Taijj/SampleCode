
using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State the <see cref="Hero"/> is in after they took damage.
	/// </summary>
	public class HeroFlinching : HeroState
	{
		#region Fields
		[SerializeField] private float _force;
		[SerializeField] private float _duration;
		[SerializeField] private EventReference _sound;

		public AttackInfo Info { set; private get; }
		private float CompletedTime { get; set; }
		#endregion



		#region Entering
		public override void Enter()
		{
			Invul.BlinkDamage();
			Pawn.Animator.Trigger(AnimatorHashes.FLINCH);
			if (Info.Flinch == FlinchKind.Volume)
				FlinchVolumetric();
			else
				FlinchDefault();

			Level.Cameraman.Shake.Do(CameraShake.Normal);
			Game.Audio.Play(_sound, Pawn.Transform);
			CompletedTime = Time.time + _duration;
		}

		private void FlinchVolumetric()
		{
			Pawn.Face(Info.Contact.normal);
			Pawn.Push(-Info.Contact.normal, _force);
		}

		private void FlinchDefault()
		{
			int sign = Info.Direction.x.Sign();
			Pawn.Face(Vector2.left * sign);
			Pawn.Push( new Vector2(sign, 1f), _force);
		}
		#endregion



		#region Misc
		public override void OnUpdate()
		{
			if (Time.time >= CompletedTime)
			{
				Invul.StartInvul();
				Transit(typeof(HeroMoving));
			}
		}

		public override void OnFixedUpdate() => Pawn.OnFixedUpdate();
		public override void Exit() => Pawn.Rigidbody.WakeUp();
		#endregion
	}
}