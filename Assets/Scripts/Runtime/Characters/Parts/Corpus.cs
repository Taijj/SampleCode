using System;
using System.Collections.Generic;
using UnityEngine;

namespace Taijj.HeartWarming
{
	/// <summary>
	/// Encaspulates Unity collission features, e.g. Colliding and Triggering
	/// </summary>
	public class Corpus : MonoBehaviour, IWaterlogged
	{
		#region LifeCycle
		public const int CONTACT_CAPACITY = 8;

		[SerializeField] private CapsuleCollider2D _collider;

		public void Wake()
		{
			Contacts = new List<Contact>(CONTACT_CAPACITY);
			IsReady = true;
		}

		public CapsuleCollider2D Collider => _collider;
		private bool IsReady { get; set; }
		#endregion



		#region Physics
		public void OnCollisionExit2D(Collision2D collision) => Contacts.Clear();
		public void OnCollisionEnter2D(Collision2D collision) => OnCollisionStay2D(collision);
		public void OnCollisionStay2D(Collision2D collision)
		{
			if (false == IsReady)
				return;

			if (_clearContactsOnNextCollision)
				Contacts.Clear();

			ContactPoint2D[] contacts = collision.contacts;
			for (int i = 0; i < contacts.Length; i++)
			{
				ContactPoint2D unityContact = contacts[i];
				if (false == unityContact.enabled)
					continue;

				Contact con = new Contact(unityContact);
				CheckSnowball(con, out bool isIgnored);
				if (isIgnored)
					continue;

				if (false == Contacts.Contains(con))
					Contacts.Add(con);
			}
			_clearContactsOnNextCollision = false;

			#if UNITY_EDITOR
				DrawContacts();
			#endif
		}

		private void CheckSnowball(Contact contact, out bool isContactIgnored)
		{
			if (contact.collider.TryGet(out SnowBall ball))
				isContactIgnored = false == ball.IsOnTop(contact.point);
			else
				isContactIgnored = false;
		}

		public void OnFixedUpdate() => _clearContactsOnNextCollision = true;
		private bool _clearContactsOnNextCollision;

		public List<Contact> Contacts { get; private set; }
		#endregion



		#region Water
		public void OnEnterWater(MoveData.Modifiers modifiers)
		{
			IsInWater = true;
			OnEnteredWater(modifiers);
		}

		public void OnExitWater()
		{
			OnExitedWater();
			IsInWater = false;
		}

		public Action<MoveData.Modifiers> OnEnteredWater { set; get; }
		public Action OnExitedWater { set; get; }
		[field: SerializeField, ReadOnly] public bool IsInWater { get; private set; }
		#endregion



		#if UNITY_EDITOR
		[Space, Header("Editor")]
		[SerializeField] private bool _drawGizmos;
		[SerializeField] private float _gizmosDuration = 0.5f;

		public void OnValidate()
		{
			if (_collider.IsNull(true)) _collider = GetComponentInParent<CapsuleCollider2D>();
		}

		private void DrawContacts()
		{
			if (false == _drawGizmos)
				return;

			for(int i = 0; i < Contacts.Count; i++)
			{
				Contact con = Contacts[i];
				Note.DrawCross(con.point, 0.1f, ColorAddons.Green, _gizmosDuration);
				Note.DrawRay(con.point, con.normal, ColorAddons.Green, _gizmosDuration);
			}
		}

		public void OnDrawGizmos()
		{
			if (_drawGizmos)
				Note.DrawCross(transform.position, 0.1f, Color.yellow, _gizmosDuration);
		}
		#endif
	}
}