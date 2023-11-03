using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Checks, if the main Camera's position is inside the defined box
	/// and sends de/activation signals to it's owner accordingly.
	/// </summary>
    public class CameraDetector : MonoBehaviour
    {
		#region LifeCycle
		[Span("Width", "Height")]
		[SerializeField] private Vector2 _detectionBox;
		[SerializeField, ReadOnly] bool _isDetected;

		public void Wake(Action activate, Action deactivate)
		{
			CameraTransform = Level.Cameraman.Camera.transform;
			OwnTransform = transform;

			HalfWidth = _detectionBox.x/2f;
			HalfHeight = _detectionBox.y/2f;
			Activate = activate;
			Deactivate = deactivate;
		}

		public void SetUp()
		{
			IsDetected = false;
			OnUpdate();
		}

		public void Respawn() => IsDetected = IsCameraInsideDetectionBox();
		#endregion



		#region Update
		public void OnUpdate()
		{
			bool isDetected = IsCameraInsideDetectionBox();
			if (isDetected == IsDetected)
				return;

			IsDetected = isDetected;
		}

		private bool IsDetected
		{
			get => _isDetected;

			set
			{
				_isDetected = value;
				if (_isDetected)
					Activate();
				else
					Deactivate();
			}
		}

		private bool IsCameraInsideDetectionBox()
		{
			Vector2 camPos = CameraTransform.position;
			Vector2 ownPos = OwnTransform.position;

			bool detectedX = camPos.x.IsBetween(ownPos.x - HalfWidth, ownPos.x + HalfWidth);
			bool detectedY = camPos.y.IsBetween(ownPos.y - HalfHeight, ownPos.y + HalfHeight);
			return detectedX && detectedY;
		}

		private Action Activate { get; set; }
		private Action Deactivate { get; set; }
		#endregion



		#region Misc Properties
		private Transform CameraTransform { get; set; }
		private Transform OwnTransform { get; set; }

		private float HalfWidth { get; set; }
		private float HalfHeight { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnDrawGizmosSelected()
		{
			Gizmos.color = ColorAddons.Lime;
			Gizmos.DrawWireCube(transform.position, _detectionBox);
		}
		#endif
	}
}