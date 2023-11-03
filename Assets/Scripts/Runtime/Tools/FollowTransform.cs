using UnityEngine;

namespace Taijj.SampleCode
{
	public class FollowTransform : MonoBehaviour
	{
		[SerializeField] private Transform _transformToFollow;
		[SerializeField] private Vector3 _offset;

		void Update()
		{
			if (_transformToFollow)
			{
				transform.position = _transformToFollow.position + _offset;
			}
		}
	}
}
