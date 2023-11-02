using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Simple <see cref="Pawn"/> without any kind of physics.
	/// </summary>
	public class PawnFlying : Pawn
	{
		public override void Move(Vector2 rawDirection, float rawSpeed)
		{
			Rigidbody.velocity = rawDirection * rawSpeed * Game.Updater.AmplifiedFixedDeltaTime;
		}

		public override void Push(Vector2 normalizedDirection, float force) => throw new System.Exception("Flying Pawns cannot be pushed!");
		public override void Stop() => Rigidbody.velocity = Vector2.zero;
	}
}