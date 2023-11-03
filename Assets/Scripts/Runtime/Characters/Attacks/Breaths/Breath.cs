using UnityEngine;
using FMODUnity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Taijj.SampleCode
{
	/// <summary>
	/// An attack working with particles and an area of effect, e.g. a cone.
	/// NOTE: Make sure the Rigidbody is set to "Never Sleep"!
	/// </summary>
	public class Breath : Attack
	{
		#region LifeCycle
		[Space, Header("Breath")]
		[SerializeField] private DamageSender _damageSender;
		[SerializeField] private float _damageInterval;
		[SerializeField, Range(0f, 90f)] private float _rotationCap;
		[Space]
		[SerializeField] private Heater _heater;
		[SerializeField] private float _heatInterval;
		[Space]
		[SerializeField] private BreathShape _shape;
		[SerializeField] private BreathVisuals _visuals;
		[SerializeField] private EventReference _sound;

		public override void Wake()
        {
            base.Wake();
			Transform = transform;
			Condition = new BreathCondition(_damageInterval);

			AttackInfo info = new AttackInfo();
			_damageSender.Wake(info, Condition);
			_heater.Wake();
			_shape.Wake();

			_visuals.Hide();
        }

		public override void CleanUp() => Cease();

		private BreathCondition Condition { get; set; }
		private Transform Transform { get; set; }
		#endregion



		#region Main
		protected override void Perform()
		{
			Game.Updater.AddFixed(OnUpdate);
			OnUpdate();

			_visuals.Show();
			_shape.Activate();

			Game.Audio.Play(_sound, Origin);
			IsFiring = true;
		}

		public override void Cease()
		{
			if (false == IsFiring)
				return;
			IsFiring = false;

			_visuals.Hide();
			_shape.Deactivate();
			Game.Updater.RemoveFixed(OnUpdate);
			Game.Audio.Stop(_sound);
		}

		private void OnUpdate()
		{
			int facing = Origin.right.x.Sign();
			float rot = UpdateRotation(facing);
			Vector2 pos = Origin.position;

			Transform.position = pos;
			Transform.rotation = Quaternion.Euler(Vector3.forward * rot);
			_shape.Cast(pos, rot);
			_visuals.OnUpdate(facing);

			Condition.OnUpdate();
			_damageSender.OnUpdate();

			float time = Time.time;
			if(time > NextHeatTime)
			{
				NextHeatTime = time + _heatInterval;
				_heater.Heat();
			}
		}

		private bool IsFiring { get; set; }
		public override bool CanPerform => false == IsFiring;
		private float NextHeatTime { get; set; }
		#endregion



		#region Rotation
		private float UpdateRotation(int facing)
		{
			Vector2 input = new Vector2(RotationInput.x, -RotationInput.y);
			float angle = Vector2.SignedAngle(input, Vector2.right);

			angle = facing == 1 ? ClampRight(angle) : ClampLeft(angle);
			return angle;
		}

		private float ClampRight(float angle) => Mathf.Clamp(angle, -_rotationCap, _rotationCap);

		private float ClampLeft(float angle)
		{
			float cap = 180f - _rotationCap;
			if (angle > 0f && angle < cap)
				return cap;

			if (angle < 0 && angle > -cap)
				return -cap;

			if (angle == 0)
				angle = 180f;

			return angle;
		}

		public Vector2 RotationInput { set; private get; }
		#endregion



		#if UNITY_EDITOR
		private const float GIZMO_RADIUS = 1f;

		public void OnDrawGizmosSelected()
		{
			if (EditorApplication.isPlaying)
				return;

			Vector2 start = (Vector2) (Quaternion.Euler(Vector3.forward * -_rotationCap) * Vector3.right);

			Handles.color = Color.yellow.With(0.25f);
			Handles.DrawSolidArc(transform.position, Vector3.forward, start, _rotationCap*2f, GIZMO_RADIUS);
			Handles.color = Color.yellow;
			Handles.DrawWireArc(transform.position, Vector3.forward, start, _rotationCap*2f, GIZMO_RADIUS);

			_shape.DrawGizmos(transform);
		}

		public void OnValidate()
		{
			if (_damageSender.IsNull(true)) _damageSender = GetComponent<DamageSender>();
			_visuals.OnValidate();
		}
#endif
	}
}