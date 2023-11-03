
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Taijj.SampleCode
{
	/// <summary>
	/// State while the Character is moving along on solid ground.
	/// </summary>
    public class MoveGrounded : MoveState
    {
		#region LifeCycle
		public MoveGrounded(MoveData data) : base(data) {}
		public override float ConfigAngle => 0f;
		#endregion



		#region Main
		public override void Handle(Jump jump)
		{
			if (false == jump.IsJumping)
				jump.ResetCount();
		}

		public override void Execute()
		{
			int sign = MoveSign;
			Vector2 direction = Addons.GetDirectionFrom(Contacts);
			Vector2 signedDirection = direction * sign;

			if (sign < 0 && IsCollidingLeft)
				Velocity = Vector2.zero;
			else if (sign > 0 && IsCollidingRight)
				Velocity = Vector2.zero;
			else
				Velocity = MoveSpeed * signedDirection;

			DetermineSlipperyness();
		}
		#endregion



		#region Slipperyness
		private void DetermineSlipperyness()
		{
			TileConfig touchedTile = null;
			for (int i = 0; i < Contacts.Count; i++)
			{
				Contact contact = Contacts[i];
				if(false == contact.collider.TryGetComponent(out Tilemap map))
				{
					CurrentSlipperyness = 0f;
					return;
				}

				Vector3Int pos = map.WorldToCell(contact.point + Vector2.down * Config.groundCastDistance);
				if(map.TryGetTile(pos, out TileConfig config))
				{
					if (touchedTile.IsNull() || config.state == SnowBox.State.Ice)
						touchedTile = config;
				}
			}

			CurrentSlipperyness = touchedTile.HasReference() ? touchedTile.slipperyness : 0f;
		}

		public float CurrentSlipperyness { get; private set; }
		#endregion
	}
}