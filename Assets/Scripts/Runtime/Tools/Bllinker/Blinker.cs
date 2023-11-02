using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Handles blinking of a set of <see cref="SpriteRenderer"/>s, e.g from a character
	/// when it is damaged.
	/// </summary>
    public class Blinker : MonoBehaviour
    {
		#region LifeCycle
		private const string COLOR_PROPERTY_NAME = "_BlinkColor";

		[SerializeField] private SpriteRenderer[] _renderers;

		public void Wake()
		{
			Block = new MaterialPropertyBlock();
			Call = new DelayedCall(OnCompleted);
			Call.SetUpdate(OnUpdate);
		}

		public void CleanUp() => Stop();

		private DelayedCall Call { get; set; }
		#endregion



		#region Control
		public void Do(Blink blink)
		{
			CurrentBlink = blink;

			SetRenderers(Color.black);
			Call.Delay = CurrentBlink.duration;
			Call.Start();
		}

		public void Stop()
		{
			SetRenderers(Color.black);
			Call.Stop();
		}
		#endregion



		#region Update
		private void OnUpdate()
		{
			float elapsedFrac = Call.ElapsedTime / CurrentBlink.duration;
			float param = CurrentBlink.pattern.Evaluate(elapsedFrac);
			SetRenderers(Color.Lerp(Color.black, CurrentBlink.color, param));
		}

		private void OnCompleted()
		{
			SetRenderers(Color.black);
			if (CurrentBlink.isLooping)
				Do(CurrentBlink);
		}

		private void SetRenderers(Color color)
		{
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].GetPropertyBlock(Block);
				Block.SetColor(COLOR_PROPERTY_NAME, color);
				_renderers[i].SetPropertyBlock(Block);
			}
		}

		private Blink CurrentBlink { get; set; }
		private MaterialPropertyBlock Block { get; set; }
		#endregion



		#if UNITY_EDITOR
		public void OnValidate() => this.TryAssign(ref _renderers);
		#endif
	}
}