using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Weapon that fires <see cref="Shot"/>s in a configurable frequency.
	/// </summary>
	public class Gun : Attack
    {
		#region Configuration
		[Serializable]
		private class SalvoConfig
		{
			public float refireDelay = 0.1f;
			public int maxSalvo = 3;

			public float salvoDuration = 0.3f;
			public float overheatDuration = 1f;
		}

		[Serializable]
		private class ShotConfig
		{
			public Shot prefab;
			public Vector2 velocity;
			[Tooltip("Vertical offset added/subtraced to each alternating Shot's spawn position.")]
			public float alternatingOffset;
			[Tooltip("How far will this Shot survive past the screen borders. Normalized Viewport distance.")]
			public float offScreenTolerance = 0.1f;
		}

		[Space, Header("Gun")]
		[SerializeField] private SalvoConfig _salvo;
		[SerializeField] private ShotConfig _shot;
		#endregion



		#region LifeCycle
		public override void Wake()
		{
			base.Wake();
			WakeShot();
			Game.Updater.AddUpdate(UpdateSalvo);
		}

		public override void CleanUp()
		{
			base.CleanUp();
			Game.Updater.RemoveUpdate(UpdateSalvo);
		}

		protected override void Perform()
		{
			SetSalvo();
			Shoot();
		}
		#endregion



		#region Salvo
		private void SetSalvo()
		{
			float time = Time.time;
			FireRateCoolOffTime = time + _salvo.refireDelay;

			SalvoCounter++;
			SalvoCoolOffTime = time + _salvo.salvoDuration;

			if (SalvoCounter >= _salvo.maxSalvo)
			{
				SalvoCounter = 0;
				OverheatCoolOffTime = time + _salvo.overheatDuration;
			}
		}

		public void UpdateSalvo()
		{
			float time = Time.time;
			if (SalvoCounter > 0 && time >= SalvoCoolOffTime)
			{
				SalvoCounter--;
				SalvoCoolOffTime = time + _salvo.salvoDuration;
			}
		}

		private int SalvoCounter { get; set; }
		private float FireRateCoolOffTime { get; set; }
		private float SalvoCoolOffTime { get; set; }
		private float OverheatCoolOffTime { get; set; }

		public override bool CanPerform
		{
			get
			{
				float time = Time.time;
				if (time < FireRateCoolOffTime)
					return false;

				if (time < OverheatCoolOffTime)
					return false;

				if (SalvoCounter >= _salvo.maxSalvo)
					return false;

				return true;
			}
		}
		#endregion



		#region Shot
		private void WakeShot()
		{
			ShotInfo = new ShotInfo();
			ShotInfo.prefab = _shot.prefab;
		}

		private void Shoot()
		{
			ShotInfo.position = (Vector2)Origin.position + GetOffset();
			ShotInfo.velocity = _shot.velocity * Facing;
			ShotInfo.offscreenTolerance = _shot.offScreenTolerance;
			Level.ShotFactory.Spawn(ShotInfo);
		}

		private Vector2 GetOffset()
		{
			IsOffsetAdded = !IsOffsetAdded;
			return Vector2.up * _shot.alternatingOffset * (IsOffsetAdded ? 1f : -1f);
		}

		// If false, offset is subtracted.
		private bool IsOffsetAdded { get; set; }
		private ShotInfo ShotInfo { get; set; }
		#endregion
	}
}