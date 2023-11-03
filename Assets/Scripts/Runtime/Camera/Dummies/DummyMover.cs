using DG.Tweening;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Testing Dummy that simply moves between two positions
	/// using DOTween in a loop.
	/// </summary>
	public class DummyMover : MonoBehaviour
	{
		[SerializeField] private Transform _posOne;
		[SerializeField] private Transform _posTwo;

		public void Awake()
		{
			Sequence seq = DOTween.Sequence();
			seq.Append(transform.DOMove(_posTwo.position, 1f));
			seq.Append(transform.DOMove(_posOne.position, 1f));
			seq.SetLoops(-1);
			seq.Play();
		}
	}
}
