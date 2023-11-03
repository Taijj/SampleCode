using BezierSolution;
using DG.Tweening;
using FMODUnity;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State that handles the Falcon <see cref="Enemy"/>'s dive attack.
	/// Uses a <see cref="BezierSpline"/> to calculate the attack arch and
	/// <see cref="Tween"/>s for the movement.
	/// </summary>
	public class FalconDive : EnemyState
	{
		#region LifeCycle
		[SerializeField] private float _anticipateDuration;
		[SerializeField] private float _anticipateDistance;
		[SerializeField] private Ease _anticipateEase;
		[Space]
		[SerializeField] private float _diveDuration;
		[SerializeField] private Ease _diveEase;
		[SerializeField] private EventReference _diveSound;
		[Space]
		[SerializeField] private BezierSpline _spline;

		public override void Wake(StateData data)
		{
			base.Wake(data);
			Transform = transform;
			_spline.Initialize(3);
			WakeTwweens();
		}

		public override void CleanUp()
		{
			Anticipate?.Kill();
			Dive?.Kill();

			Owner.OnActivationChange -= OnActivationChange;
			Owner.OnDied -= OnDied;
		}

		private Transform Transform { get; set; }
		private Vector2 CurrentPosition { get; set; }
		#endregion



		#region Tweens

		private enum Phase
		{
			None = 0,

			Anticipate,
			Dive,
			Rise
		}

		private void WakeTwweens()
		{
			Anticipate = Pawn.Transform.DOMove(Vector2.zero, _anticipateDuration);
			Anticipate.OnComplete(OnAnticipated);

			Dive = DOTween.To(GetParameter, SetParameter, 1f, _diveDuration);
			Dive.SetEase(_diveEase);
			Dive.OnComplete(OnDove);
		}

		public override void Enter()
		{
			CurrentPosition = Pawn.transform.position;
			Transform.position = CurrentPosition;
			CurrentPhase = Phase.Anticipate;

			Pawn.Stop();
			Pawn.Face(Detector.ToHero);
			Pawn.Animator.Set(PHASE_HASH, (int)CurrentPhase);
			UpdatePath(Detector.HeroCenter, out Vector3 startPos);

			_parameter = 0f;
			Anticipate.SetValues((Vector3)CurrentPosition, startPos);

			Owner.OnActivationChange += OnActivationChange;
			Owner.OnDied += OnDied;
		}

		private void OnAnticipated()
		{
			CurrentPhase = Phase.Dive;

			Game.Audio.Play(_diveSound, Transform);
			Pawn.Animator.Set(PHASE_HASH, (int)CurrentPhase);
			Dive.Restart();
		}

		private void OnDove()
		{
			CurrentPhase = Phase.None;

			Pawn.Animator.Set(PHASE_HASH, (int)CurrentPhase);
			Transit(typeof(EnemyPatrolling));
		}

		public void OnDied()
		{
			CurrentPhase = Phase.None;

			Pawn.Animator.Set(PHASE_HASH, (int)CurrentPhase);
			Anticipate.Pause();
			Dive.Pause();
		}



		public override void Exit()
		{
			base.Exit();
			Owner.OnActivationChange -= OnActivationChange;
			Owner.OnDied -= OnDied;
		}

		public void OnActivationChange(bool isActive)
		{
			if (CurrentPhase == Phase.None)
				return;

			if(CurrentPhase == Phase.Anticipate)
			{
				Anticipate.TogglePause();
			}
			else
			{
				Dive.TogglePause();
			}
		}



		private void SetParameter(float value)
		{
			_parameter = value;
			Pawn.Rigidbody.position = _spline.GetPoint(_parameter);

			bool isBeyondApex = _parameter >= 0.5f;
			if (isBeyondApex)
			{
				CurrentPhase = Phase.Rise;
				Pawn.Animator.Set(PHASE_HASH, (int)CurrentPhase);
			}
		}

		private float GetParameter() => _parameter;
		private float _parameter;



		private static readonly int PHASE_HASH = Animator.StringToHash("Phase");
		private Phase CurrentPhase { get; set; }
		private Tween Anticipate { get; set; }
		private Tween Dive { get; set; }
		#endregion



		#region Path
		// The dive path always consists of 3 anchor points with automatically determined
		// spline control points. The control point of the first anchor point determines
		// in which direction the anticipation movement goes.
		private void UpdatePath(Vector2 target, out Vector3 diveStartPosition)
		{
			Vector2 toTarget = target - CurrentPosition;
			int sign = toTarget.x.Sign();

			float midControlDistance = Mathf.Abs(toTarget.x)/2f;
			Vector2 controlMid1 = target + Vector2.left*midControlDistance * sign;
			Vector2 controlMid2 = target + Vector2.right*midControlDistance * sign;

			float endControlDistance = Mathf.Abs(toTarget.y)/2f;
			Vector2 toSecond = (controlMid1 - CurrentPosition).normalized;
			Vector2 first = CurrentPosition - toSecond * _anticipateDistance;
			Vector2 controlFirst = CurrentPosition + toSecond * endControlDistance;
			diveStartPosition = first;

			Vector2 last = CurrentPosition + toTarget + toTarget.WithY(-toTarget.y);
			Vector2 controlLast = last + (controlMid2 - last).normalized * endControlDistance;

			BezierPoint point1 = _spline[0];
			point1.handleMode = BezierPoint.HandleMode.Free;
			point1.position = first;
			point1.precedingControlPointPosition = CurrentPosition;
			point1.followingControlPointPosition = controlFirst;

			BezierPoint point2 = _spline[1];
			point2.handleMode = BezierPoint.HandleMode.Free;
			point2.position = target;
			point2.precedingControlPointPosition = controlMid1;
			point2.followingControlPointPosition = controlMid2;

			BezierPoint point3 = _spline[2];
			point3.handleMode = BezierPoint.HandleMode.Free;
			point3.position = last;
			point3.precedingControlPointPosition = controlLast;
			point3.followingControlPointPosition = CurrentPosition;
		}
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField] private Transform _testTarget;

		[Space]
		[Button(nameof(TestPath))]
		[SerializeField] public bool pathButton;
		public void TestPath()
		{
			Transform = transform;
			CurrentPosition = transform.position;
			_spline.Initialize(3);

			if(_testTarget.HasReference(true))
				UpdatePath(_testTarget.position, out Vector3 _);
		}

		[Button(nameof(TestDive))]
		[SerializeField] public bool diveButton;
		public void TestDive()
		{
			WakeTwweens();
			Transit(GetType());
		}
		#endif
	}
}