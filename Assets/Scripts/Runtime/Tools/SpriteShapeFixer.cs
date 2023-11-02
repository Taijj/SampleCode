using UnityEngine;

namespace BLE.SevenSins
{
    // Target for custom Editor that helps with
    // SpriteShape workarounds.
    public class SpriteShapeFixer : MonoBehaviour
    {
		#if UNITY_EDITOR
        public enum Mode
        {
            FixUnityCollider = 0,
            RectangularCollider
        }

        public Mode mode;
        [HideInInspector] public float slopesAngle = 45;
        [HideInInspector] public float slopesOffset = 0.1f;
        [HideInInspector] public float height = 1f;
		[HideInInspector] public float borderOffset = 0f;
		[HideInInspector] public float trapezoidOffset = 0f;
		#endif
	}
}