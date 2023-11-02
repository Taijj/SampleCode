
using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Casts rays to roughly determine what kind of surface
	/// a <see cref="Breath"/> hits. This surface is applied
	/// to an <see cref="EdgeCollider2D"/> which is then used,
	/// e.g. as its damaging or Heating shape.
	/// </summary>
	[Serializable]
    public class BreathShape
    {
		#region LifeCycle
		[SerializeField] private EdgeCollider2D _collider;
		[SerializeField] private FloatRange _range;
		[SerializeField] private float _thickness;
		[SerializeField, Range(3, 15)] private int _rayCount;
		[Space]
		[SerializeField] private LayerMask _mask;

		public void Wake()
		{
			RayDatas = new RayData[_rayCount];
			for(int i = 0; i < _rayCount; i++)
				RayDatas[i] = new RayData();

			Hits = new RaycastHit2D[Mathf.CeilToInt(_range.Delta)];
			Points = new Vector2[_rayCount];
			Transform = _collider.transform;
			Transform.localPosition = Vector3.zero;

			Deactivate();
		}

		public void Activate() => _collider.Activate();
		public void Deactivate() => _collider.Deactivate();

		private Transform Transform { get; set; }
		private Vector2[] Points { get; set; }
		#endregion



		#region Preparations
		private class RayData
		{
			public Vector2 origin;
			public Vector2 direction;

			public int index;
		}

		private void RefreshDatasAndCast(Vector2 position, float rotation, Action<RayData> castMethod)
		{
			Vector2 dir = rotation.ConvertToDirection();
			Vector2 perpend = Vector2.Perpendicular(dir);

			float delta = _thickness/(_rayCount-1);
			Vector2 start = position + dir*_range.Min + perpend*_thickness/2f;
			for (int i = 0; i < _rayCount; i++)
			{
				RayData data = RayDatas[i];
				data.origin = start - perpend*delta * i;
				data.direction = dir;
				data.index = i;

				castMethod(data);
			}
		}

		private RayData[] RayDatas { get; set; }
		#endregion



		#region Casting
		public void Cast(Vector2 muzzlePosition, float rotation)
		{
			Transform.rotation = Quaternion.identity;

			Position = Transform.position;
			RefreshDatasAndCast(muzzlePosition, rotation, CastRay);
			_collider.points = Points;
		}

		private void CastRay(RayData data)
		{
			#if UNITY_EDITOR
				TryDraw(data);
			#endif

			ContactFilter2D filter = new ContactFilter2D
			{
				useTriggers = true,
				layerMask = _mask,
				useLayerMask = true
			};

			Vector2 localOrigin = data.origin - Position;
			int hitCount = Physics2D.Raycast(data.origin, data.direction, filter, Hits, _range.Delta);

			float distance = GetDistance(hitCount);
			distance = Mathf.Max(_range.Min, distance);
			distance += _collider.edgeRadius;

			Points[data.index] = localOrigin + data.direction*distance;
		}

		private float GetDistance(int hitCount)
		{
			if (hitCount == 0)
				return _range.Delta;

			for(int i = 0; i < hitCount; i++)
			{
				Collider2D col = Hits[i].collider;
				bool isWorldTrigger = col.gameObject.layer == Layers.WORLD && col.isTrigger;
				if (isWorldTrigger)
					continue;

				return Hits[i].distance;
			}

			return _range.Delta;
		}

		private RaycastHit2D[] Hits { get; set; }
		private Vector2 Position { get; set; }
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField] private bool _drawRays;

		public void DrawGizmos(Transform transform)
		{
			if(UnityEditor.EditorApplication.isPlaying)
				return;

			if (false == _drawRays)
				return;

			Wake();

			Gizmos.color = Color.yellow.With(0.25f);
			RefreshDatasAndCast(transform.position, transform.rotation.eulerAngles.z, Draw);
			void Draw(RayData data) => Gizmos.DrawRay(data.origin, data.direction * _range.Delta);

			Vector2 start = Vector2.right * (_range.Min+_range.Delta+_collider.edgeRadius);
			_collider.points = new Vector2[] { start + Vector2.up * _thickness/2f, start + Vector2.down * _thickness/2f };
		}

		private void TryDraw(RayData data)
		{
			if (_drawRays)
				Note.DrawRay(data.origin, data.direction * _range.Delta, Color.magenta.With((float)data.index/_rayCount), 0.5f);
		}
		#endif
	}
}