using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Interface for an entity, that can be de-/heated.
	/// </summary>
	public interface IHeatable
	{
		/// <summary>
		/// Treat positive amounts as "heating up" and negative ones as "cooling down".
		/// </summary>
		public void Heat(float amount);
	}

	/// <summary>
	/// An entity that manipulates an <see cref="IHeatable"/>, positive or negative.
	/// NOTE: Do not confuse with an actual real life heater!
	/// </summary>
	[System.Serializable]
    public class Heater
    {
		private const int MAX_SIMULTANEOUS_HEATABLES = 200;

		[SerializeField] private Collider2D _aoe;
		[SerializeField] private float _amount;
		[HideInInspector] public bool enabled;

		public void Wake()
		{
			if (false == enabled)
				return;

			HitColliders = new Collider2D[MAX_SIMULTANEOUS_HEATABLES];
			Mask = Layers.ToMask(Layers.SNOWBOX, Layers.SUPPORT);
		}

		public void Heat()
		{
			if (false == enabled)
				return;

			ContactFilter2D filter = new ContactFilter2D
			{
				useLayerMask = true,
				layerMask = Mask,
				useTriggers = true
			};

			int count = _aoe.OverlapCollider(filter, HitColliders);			
			for (int i = 0; i < count; i++)
			{
				if (HitColliders[i].TryGet(out IHeatable heatable))
					heatable.Heat(_amount);
			}
		}

		private Collider2D[] HitColliders { get; set; }
		private LayerMask Mask { get; set; }
    }
}
