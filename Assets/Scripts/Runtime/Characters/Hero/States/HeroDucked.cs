
using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// State the <see cref="Hero"/> is in while
	/// ducked.
	/// </summary>
	public class HeroDucked : HeroState
    {
		public static readonly int IS_DUCKING_HASH = Animator.StringToHash("IsDucking");

		[SerializeField] private Transform[] _scaledTransforms;
		[SerializeField, Range(0f, 1f)] private float _normalizedSize;
		[SerializeField] private EventReference _sound;

		public override void Enter()
		{
			Input.OnShootFire = ShootFire;
			Input.OnShootIce = ShootIce;
			Input.OnSpecial = Arsenal.PerformSpecial;

			Game.Audio.Play(_sound, Pawn.Transform);
			Pawn.Animator.Set(IS_DUCKING_HASH, true);

			for (int i = 0; i < _scaledTransforms.Length; i++)
				_scaledTransforms[i].localScale = new Vector3(1f, _normalizedSize, 1f);
		}

		public override void OnUpdate()
		{
			Input.OnUpdate();
			Pawn.Face(Input.Move);

			if (false == Input.IsDownHeld && false == Arsenal.IsBreathing)
			{
				Transit(typeof(HeroMoving));
				return;
			}
			
			UpdateBreathing();			
		}

		public override void OnFixedUpdate()
		{
			Pawn.Move(Vector2.zero, 0f);
			Pawn.OnFixedUpdate();
		}

		public override void Exit()
		{
			Input.OnShootFire = null;
			Input.OnShootIce = null;
			Input.OnSpecial = null;

			Pawn.Animator.Set(IS_DUCKING_HASH, false);
			for (int i = 0; i < _scaledTransforms.Length; i++)
				_scaledTransforms[i].localScale = Vector3.one;
		}
	}
}