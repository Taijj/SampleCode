using Cinemachine;
using UnityEngine;

namespace Taijj.SampleCode
{
	[System.Serializable]
	public class CameraConfig
	{
		[field: SerializeField] public CinemachineVirtualCamera Machine { get; set; }
		[field: SerializeField] public CameraKind Kind { get; set; }
		[field: SerializeField] public Transform Target { get; set; }
		[field: SerializeField] public bool CanLookAround { get; set; }
	}
}
