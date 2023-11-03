using FMODUnity;
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
    public enum PickupKind
    {
        None,

        OreBronze,
		OreSilver,
		OreGold,

		Health,
		Mana,
		Skill
    }

    /// <summary>
    /// Base class for collectable items.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        #region LifeCycle
        [Header("Base")]
        [SerializeField] private PickupKind _kind = PickupKind.None;
        [SerializeField] private float _value = 1f;
		[SerializeField] private EventReference _collectSound;
        [Space]
        [SerializeField] private GameObject _content;
		[SerializeField] private CameraDetector _detector;
		[SerializeField] private Animator _animator;

        public virtual void Wake()
        {
			Transform = transform;
			HasAnimator = _animator.HasReference(true);
			Deactivate();

			_detector.Wake(Activate, Deactivate);
			_detector.OnUpdate();
        }

		public virtual void Respawn()
		{
			IsCollected = false;
			_detector.Respawn();
		}

		public virtual void CleanUp() { }



		public virtual void OnFixedUpdate() { }
		public virtual void OnUpdate() => _detector.OnUpdate();
		#endregion



		#region Collecting
		public void OnTriggerEnter2D(Collider2D collider) => TryCollect(collider);
        public void OnTriggerStay2D(Collider2D collider) => TryCollect(collider);

        private void TryCollect(Collider2D collider)
        {
			if (false == collider.TryGet(out Pawn pawn))
				return;

			if (pawn.gameObject.layer == Layers.HERO)
				Collect();
        }

        protected virtual void Collect()
        {
            PlaySound(_collectSound);

            IsCollected = true;
			ContentEnabled = false;

			OnCollected?.Invoke(this);
        }

        public event Action<Pickup> OnCollected;
        public bool IsCollected { get; set; }
        #endregion



        #region Activation
		protected virtual void Activate() => ContentEnabled = IsCollected == false;
		protected virtual void Deactivate() => ContentEnabled = false;

        protected bool ContentEnabled
        {
			get => _content.activeSelf;

            set
            {
                _content.SetActive(value);

				if (HasAnimator && value)
					_animator.Randomize();
            }
        }

		private bool HasAnimator { get; set; }
		#endregion



		#region Misc
		protected void PlaySound(EventReference e) => Game.Audio.Play(e, Transform);
		
		public Transform Transform { get; private set; }
		public PickupKind Kind => _kind;
		public float Value => _value;
		#endregion



		#if UNITY_EDITOR
		public virtual void OnValidate()
		{
			if (_content.IsNull(true)) _content = transform.GetChild(0).gameObject;
			this.TryAssign(ref _detector);
			this.TryAssign(ref _animator);
		}

		protected PickupKind KindFromEditor { set => _kind = value; }
		protected float ValueFromEditor { set => _value = value; }
		#endif
	}
}