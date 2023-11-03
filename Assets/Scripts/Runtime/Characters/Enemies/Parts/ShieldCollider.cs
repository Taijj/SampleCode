using System;
using UnityEngine;

namespace Taijj.SampleCode
{
    public class ShieldCollider : MonoBehaviour
    {		
		public void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.layer != Layers.WORLD)
				return;

			OnHitWall();
		}

		public Action OnHitWall { set; private get; }
    }
}