
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State while the Character is sliding down an angled slope.
	/// </summary>
	public class MoveSloped : MoveState
    {
    	public MoveSloped(MoveData data) : base(data) { }
		public override float ConfigAngle => Config.slopeAngle;

		public override void Execute()
		{
			Vector2 direction = Addons.GetDirectionFrom(Contacts);
			if (direction.y < 0)
				direction *= -1f;

			Vector2 velocity = direction * Mathf.Lerp(Velocity.y, -SlopeSpeed, Config.slopeFriction);

			int slopeSign = -direction.x.Sign();
			int inputSign = MoveSign;
			if (slopeSign == inputSign)
				velocity = new Vector2(MoveSpeed * inputSign, GravityCapped);

			Velocity = velocity;
		}
	}
}