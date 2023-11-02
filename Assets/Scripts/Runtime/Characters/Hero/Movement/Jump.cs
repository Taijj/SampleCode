using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using System.Linq;
#endif

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Helper and manager for <see cref="Hero"/> jumping.
	/// </summary>
	[Serializable]
    public class Jump
    {
		#region Fields
		private const int MAX_COUNT = 2;
		private const float UNSET_TIME = -1f;

		[Space]
		[SerializeField] private float _force;
		[SerializeField] private float _maxDuration;
		[SerializeField] private float _coyoteDuration;
		[Space]
		[SerializeField, ReadOnly] private int _count;
		#endregion



		#region Jump
		public bool TryJump()
		{
			if (_count >= MAX_COUNT)
				return false;

			_count++;
			JumpDoneTime = Time.time + _maxDuration;
			StopCoyoting();
			return true;
		}

		public void StopJump() => JumpDoneTime = UNSET_TIME;

		public void ResetCount()
		{
			StopCoyoting();
			_count = 0;
		}

		public void OverrideCount(int value)
		{
			StopCoyoting();
			_count = Mathf.Clamp(value, 0, MAX_COUNT);
		}



		private float JumpDoneTime { get; set; }
		public bool IsJumping => JumpDoneTime > Time.time;
		public float Force => _force;
		#endregion



		#region Coyote Time
		public void StartCoyoting()
		{
			IsCoyoting = true;
			CoyoteDoneTime = Time.time + _coyoteDuration;
		}

		public void StopCoyoting()
		{
			CoyoteDoneTime = UNSET_TIME;
			IsCoyoting = false;
		}

		public void UpdateCoyoting()
		{
			if (false == IsCoyoting)
				return;

		 	if(Time.time >= CoyoteDoneTime)
			{
				_count++;
				StopCoyoting();
			}
		}

		private float CoyoteDoneTime { get; set; }
		private bool IsCoyoting { get; set; }
		#endregion




		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField] private bool _recordJump;

		public void StartJumpEditor(Rigidbody2D rigidbody)
		{
			if (false == _recordJump)
				return;

			if (IsRecording)
				return;

			PositionsY = new List<float>();
			Start = rigidbody.position.y;
			IsRecording = true;
		}

		public void StopJumpEditor(Rigidbody2D rigidbody)
		{
			if (false == IsRecording)
				return;

			float highest = PositionsY.Max() - Start;
			Note.Log($"Jump height: {highest}", ColorAddons.Pink);

			Vector2 pos = new Vector2(rigidbody.position.x, Start + highest);
			Note.DrawRay(pos - Vector2.left*2f, Vector2.right * 4f, ColorAddons.Pink, 10f);

			IsRecording = false;
		}

		public void UpdateJumpEditor(Rigidbody2D rigidbody)
		{
			if (false == IsRecording)
				return;

			PositionsY.Add(rigidbody.position.y);
		}

		private bool IsRecording { get; set; }
		private List<float> PositionsY { get; set; }
		private float Start { get; set; }
		#endif
	}
}