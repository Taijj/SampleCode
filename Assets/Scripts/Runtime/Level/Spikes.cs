using UnityEngine;

namespace Taijj.SampleCode
{
	public class Spikes : MonoBehaviour
	{
		[SerializeField] private DamageSender _sender;

		public void Awake()
		{
			AttackInfo info = new AttackInfo();
			_sender.Wake(info);
		}

		public void OnCollisionStay2D(Collision2D collision) => OnCollisionEnter2D(collision);
		public void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.collider.isTrigger)
				return;

			_sender.TryDamage(collision.collider);
		}
	}
}