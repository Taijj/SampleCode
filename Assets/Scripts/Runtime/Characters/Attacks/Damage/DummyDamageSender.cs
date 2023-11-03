#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Taijj.SampleCode
{
	/// <summary>
	/// Testing component for testing damaging behavior.
	/// </summary>
	[RequireComponent(typeof(DamageSender))]
	public class DummyDamageSender : MonoBehaviour
	{
		[SerializeField] private FlinchKind _flinch;

		public void Awake()
		{
			Sender = GetComponent<DamageSender>();
			AttackInfo info = new AttackInfo();
			info.Flinch = _flinch;
			Sender.Wake(info);
		}

		public void Update() => Sender.OnUpdate();
		private DamageSender Sender { get; set; }



		[Space, Header("Enum Testing")]
		[SerializeField] private AttackAttribute _send;
		[SerializeField] private AttackAttribute _receive;
		[SerializeField, ReadOnly] public bool reacts;

		public void OnValidate()
		{
			int send = Convert.ToInt32(_send);
			int receive = Convert.ToInt32(_receive);
			int combined = send & receive;
			Note.Log($"Send: {send} Receive: {receive} Combined: {combined}");

			reacts = combined != 0;
		}
	}
}
#endif