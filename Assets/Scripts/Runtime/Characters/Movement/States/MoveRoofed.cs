
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State while the Character is touching a solid ceiling above.
	/// </summary>
	public class MoveRoofed : MoveState
    {
    	public MoveRoofed(MoveData data) : base(data) { }
		public override float ConfigAngle => PhysicsConfig.CEILING_ANGLE;



		public override void Handle(Jump jump) => jump.StopJump();

		public override void Execute()
		{
			Vector2 dir = Addons.GetDirectionFrom(Contacts);
			Vector2 vel = -dir * MoveSpeed * MoveSign;
			vel += Vector2.down * GravityRaw;

			Velocity = vel;
		}
	}
}