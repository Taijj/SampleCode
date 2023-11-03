using UnityEngine;

namespace Taijj.SampleCode
{
    public class DummyPathMover : MonoBehaviour
    {
		//[SerializeField] private PathCreator _path;


		public void Update()
		{
			Distance += 1 * Time.deltaTime;
			//transform.position = _path.path.GetPointAtDistance(Distance);
		}

		private float Distance { get; set; }
	}
}