
namespace Taijj.SampleCode
{
	/// <summary>
	/// State while the Character is moving freely around in midair.
	/// </summary>
    public class MoveAirborne : MoveState
    {
    	public MoveAirborne(MoveData data) : base(data) {}
		public override float ConfigAngle => -1f;

		public override void Execute()
		{
			Velocity = Velocity.WithY(GravityCapped);
		}
	}
}