
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State while the Character is touching a wall.
	/// </summary>
	public class MoveWalled : MoveState
    {
    	public MoveWalled(MoveData data) : base(data) { }
		public override float ConfigAngle => Config.wallAngle;



		public override void Execute()
		{
			Vector2 direction = Addons.GetDirectionFrom(Contacts);
			if (direction.y < 0)
				direction *= -1f;

			int sign = GetSign();
			if (sign != 0)
				Velocity = new Vector2(MoveSpeed * sign, GravityCapped);
			else
				Velocity = direction * GravityCapped;
		}

		private int GetSign()
		{
			int sign = MoveSign;
			bool wallIsOnLeft = Rigidbody.position.x > Contacts[0].point.x;
			bool isTowardsWall = wallIsOnLeft && sign < 0 || false == wallIsOnLeft && sign > 0;
			if (isTowardsWall)
				sign = 0;
			return sign;
		}
	}
}