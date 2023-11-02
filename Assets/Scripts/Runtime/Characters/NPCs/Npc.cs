using FMODUnity;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Controller class for NPC Entities
	/// </summary>
    public class Npc : MonoBehaviour
    {
		#region LifeCycle
		[Space, Header("Parts")]
		[SerializeField] private Pawn _pawn;
		[SerializeField] private CameraDetector _cameraDetector;
		[SerializeField] private CharacterAnimator _animator;
		[SerializeField] private Candle _candle;
		[Space, Header("Logical")]
		[SerializeField] private float _defaultSpeed;
		[SerializeField] private float _fastSpeed;
		[Space, Header("Audio")]
		[SerializeField] private EventReference _panicLoop;
		[SerializeField] private EventReference _igniteSound;
		[SerializeField] private EventReference _extinguishSound;

		private float _moveSpeed;



		public void Wake()
		{
			_pawn.Wake(new Pawn.PawnData());
			_cameraDetector.Wake(Activate, Deactivate);

			_moveSpeed = _defaultSpeed;

			_candle.Wake();
			_candle.OnCandleHit += AdjustMoveSpeed;
		}

		public void SetUp()
		{
			_pawn.SetUp();
			_cameraDetector.SetUp();

			AudioLoop = new AudioLoop(_panicLoop, _pawn.Transform);
		}

		public void OnUpdate()
		{
			_cameraDetector.OnUpdate();

			if(IsActive)
				_candle.OnUpdate();
		}

		public void OnFixedUpdate()
		{
			if (false == IsActive)
				return;

			_pawn.OnFixedUpdate();
			TryFlipByCollision();
			_pawn.Move(new Vector2(_pawn.Facing, 0f), _moveSpeed);
		}

		public void Respawn()
		{
			_pawn.Respawn();
			_cameraDetector.Respawn();

			_animator.ResetSelf();
			_candle.Extinguish();

			AudioLoop.Stop();
		}

		public void CleanUp()
		{
			_pawn.CleanUp();
			_candle.OnCandleHit -= AdjustMoveSpeed;

			AudioLoop.Destroy();
		}



		private void Activate()
		{
			_pawn.Activate();
			_pawn.Enable();
			IsActive = true;

			if (IsCandleOn)
				AudioLoop.Play();
		}

		private void Deactivate()
		{
			_pawn.Deactivate();
			_pawn.Disable();
			IsActive = false;

			AudioLoop?.Stop();
		}

		public void Stop() => _pawn.Stop();

		private bool IsActive { get; set; }
		#endregion



		#region Logic
		private void AdjustMoveSpeed(bool candleIsOn)
		{
			IsCandleOn = candleIsOn;
			if(candleIsOn)
			{
				_moveSpeed = _fastSpeed;
				_animator.Trigger(AnimatorHashes.RUN);

				Game.Audio.Play(_igniteSound, _pawn.Transform);
				AudioLoop.Play();
			}
			else
			{
				_moveSpeed = _defaultSpeed;
				_animator.Trigger(AnimatorHashes.WALK);

				Game.Audio.Play(_extinguishSound, _pawn.Transform);
				AudioLoop.Stop();
			}
		}

		private void TryFlipByCollision()
		{
			if (_pawn.IsCollidingRight)
				_pawn.Face(Vector2.left);
			if (_pawn.IsCollidingLeft)
				_pawn.Face(Vector2.right);
		}
	

		private bool IsCandleOn { get; set; }
		private AudioLoop AudioLoop { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate()
		{
			this.TryAssign(ref _pawn);
			this.TryAssign(ref _cameraDetector);
			this.TryAssign(ref _animator);
			this.TryAssign(ref _candle);
		}
		#endif
	}
}
