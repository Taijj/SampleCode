using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles attraction of the given rigidbody towards the <see cref="Hero"/>,
	/// for features like e.g. a coin magnet.
	/// </summary>
    [Serializable]
    public class Magnet
    {
		#region LifeCycle
        [SerializeField] private float _range;
		[SerializeField] private float _acceleration;
        [SerializeField] private float _maxSpeed;

        public void Wake(Rigidbody2D rigidbody)
        {
            Rigidbody = rigidbody;
            SquareRange = _range * _range;
        }

		public void Respawn()
		{
			Rigidbody.velocity = Vector2.zero;
			CurrentSpeed = 0f;
		}

        private Rigidbody2D Rigidbody { get; set; }
        public float SquareRange { get; private set; }
		#endregion



		#region Update
		public void Attract(Vector2 toHero)
        {
            CurrentSpeed = Mathf.Min(CurrentSpeed + _acceleration, _maxSpeed);
            Rigidbody.velocity = toHero.normalized * CurrentSpeed;
        }

        private float CurrentSpeed { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void DrawGizmo(Vector2 position)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, _range);
        }
		#endif
    }
}