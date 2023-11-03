#if UNITY_EDITOR

using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Debugging Helper for snow box features.
	/// </summary>
    public class SnowBoxTester : MonoBehaviour
    {
		[SerializeField] private Collider2D _collider;

		[Button("HeatUp")]
		public bool heatUp;
		public void HeatUp() => Heat(50);

		[Button("CoolDown")]
		public bool coolDown;
		public void CoolDown() => Heat(-50);



		private void Heat(float amount)
		{
			ContactFilter2D filter = new ContactFilter2D
			{
				useLayerMask = true,
				layerMask = Layers.SNOWBOX.ToMask(),
				useTriggers = true
			};

			Collider2D[] overlaps = new Collider2D[100];
			int count = _collider.OverlapCollider(filter, overlaps);
			for (int i = 0; i < count; i++)
			{
				if (overlaps[i].TryGetComponent(out IHeatable heatable))
					heatable.Heat(amount);
			}
		}
    }
}
#endif