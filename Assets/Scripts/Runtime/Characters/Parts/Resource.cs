using System;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Helper for a ressource, like health and/or mana.
	/// </summary>
	[System.Serializable]
    public class Resource
    {
		[SerializeField] private float _default;
		[SerializeField, ReadOnly] private float _current;
		[Space]
		public bool isInfinite;

		public void TopOff() => Modify(_default);
		public void Add(float amount) => Modify(amount);
		public void Consume(float amount) => Modify(-amount);
		public void EmptyOut() => Modify(float.MinValue);

		private void Modify(float amount)
		{
			if(isInfinite)
				_current = _default;
			else
				_current = Mathf.Clamp(_current + amount, 0, _default);
			OnValueChanged?.Invoke(Normalized);
		}

		public event Action<float> OnValueChanged;

		public float Default => _default;

		public float Normalized
		{
			get
			{
				if (_current == 0f)
					return 0f;
				return _current/_default;
			}
		}

		public bool IsEmpty => _current == 0;
	}
}