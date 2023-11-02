using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles the hero's Fire Dash Special.
	/// </summary>
	public class HeroDashing : HeroState
	{
		#region LifeCycle
		[Space, Header("Parts")]
		[SerializeField] private Rigidbody2D _rigidbody;
		[SerializeField] private Corpus _corpus;
		[SerializeField] private Transform _pivot;
		[SerializeField] private FireDash _dash;
		[Space, Header("Performing")]
		[SerializeField] private float _performSpeed;
		[SerializeField] private float _performDuration;
		[SerializeField] private AnimationCurve _performCurve;
		[SerializeField] private EventReference _performSound;
		[Space, Header("Recoil")]
		[SerializeField] private float _recoilAngle;
		[SerializeField] private float _recoilSpeed;
		[SerializeField] private float _recoilDuration;
		[SerializeField] private AnimationCurve _recoilCurve;
		[SerializeField] private EventReference _recoilSound;

		public override void Wake(StateData data)
		{
			base.Wake(data);
			FilteredContacts = new List<Contact>(Corpus.CONTACT_CAPACITY);
			_dash.Wake(StartRecoil);
		}

		public override void CleanUp() => _dash.End();
		#endregion



		#region Flow
		public override void Enter()
		{
			Pawn.Stop();
			Pawn.Animator.Set(IS_DASHING_HASH, true);
			_dash.Begin();

			IsRecoiling = false;
			StartPerformance();
		}

		public override void OnUpdate() => _dash.OnUpdate();

		public override void OnFixedUpdate()
		{
			ElapsedTime += Time.fixedDeltaTime;
			if (IsRecoiling)
				UpdateRecoiling();
			else
				UpdatePerformance();

			_corpus.Contacts.Clear();
		}

		private void Complete()
		{
			Pawn.OnFixedUpdate();
			Transit(typeof(HeroMoving));
		}

		public override void Exit()
		{
			_pivot.localRotation = Quaternion.identity;
			_dash.End();
			Pawn.Animator.Set(IS_DASHING_HASH, false);
		}



		private static readonly int IS_DASHING_HASH = Animator.StringToHash("IsDashing");
		private Vector2 Direction { get; set; }
		private bool IsRecoiling { get; set; }
		private float ElapsedTime { get; set; }
		#endregion



		#region Performance
		private void StartPerformance()
		{
			Vector2 input = Input.Move;
			if (input == Vector2.zero)
				input = Vector2.right * Pawn.Facing;

			ElapsedTime = 0f;
			Direction = ToClamped8Direction(input);

			float angle = Vector2.SignedAngle(Vector2.up, Direction);
			_pivot.rotation =  Quaternion.Euler(Vector3.forward * angle);

			Game.Audio.Play(_performSound, Pawn.Transform);
		}

		private void UpdatePerformance()
		{
			if (ShouldRecoil())
			{
				StartRecoil();
				return;
			}

			float speed = _performSpeed * Game.Updater.AmplifiedFixedDeltaTime;
			float param = 1f - _performCurve.Evaluate(ElapsedTime/_performDuration);
			_rigidbody.velocity = Direction*speed*param;

			if (ElapsedTime > _performDuration)
				Complete();
		}

		private bool ShouldRecoil()
		{
			if (_corpus.Contacts.Count == 0)
				return false;

			FilterContacts();
			if (FilteredContacts.Count == 0)
				return false;

			Vector2 dir = Addons.GetDirectionFrom(FilteredContacts);
			float dot = Vector2.Dot(Direction, dir);
			float angle = Mathf.Acos(Mathf.Abs(dot)) * Mathf.Rad2Deg;
			return angle > _recoilAngle;
		}

		private void FilterContacts()
		{
			FilteredContacts.Clear();
			for (int i = 0; i < _corpus.Contacts.Count; i++)
			{
				Contact con = _corpus.Contacts[i];
				if (con.collider.TryGetComponent(out Tilemap _))
					continue;

				if (con.collider.TryGet(out IceShard _))
					continue;

				Vector2 pivotToContact = con.point - (Vector2)_pivot.position;
				bool isInFrontOfHero = Vector2.Dot(Direction, pivotToContact) > 0;
				if (isInFrontOfHero)
					FilteredContacts.Add(con);
			}
		}



		private Vector2 ToClamped8Direction(Vector2 input)
		{
			float inputAngle = Vector2.SignedAngle(Vector2.right, input);
			float clampedAngle = Mathf.Round(inputAngle / CLAMP_ANGLE) * CLAMP_ANGLE;
			return NumberAddons.ConvertToDirection(clampedAngle).normalized;
		}

		private List<Contact> FilteredContacts { get; set; }
		private const float CLAMP_ANGLE = 45f;
		#endregion



		#region Recoil
		private void StartRecoil()
		{
			if (IsRecoiling)
				return;

			Direction *= -1f;
			ElapsedTime = 0f;
			IsRecoiling = true;

			Game.Audio.Play(_recoilSound, Pawn.Transform);
		}

		private void UpdateRecoiling()
		{
			float speed = _recoilSpeed * Game.Updater.AmplifiedFixedDeltaTime;
			float param = 1f - _recoilCurve.Evaluate(ElapsedTime/_recoilDuration);
			_rigidbody.velocity = Direction*speed*param;

			if (ElapsedTime > _recoilDuration)
				Complete();
		}
		#endregion
	}
}