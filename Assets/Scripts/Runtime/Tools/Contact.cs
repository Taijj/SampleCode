
using UnityEngine;

namespace Taijj.HeartWarming
{
    public struct Contact
    {
		public Vector2 point;
		public Vector2 normal;
		public Collider2D collider;
		public float angle;

    	public Contact(ContactPoint2D unityPoint)
    	{
			point = unityPoint.point;
			normal = unityPoint.normal;
			collider = unityPoint.collider;
			angle = Vector2.Angle(normal, Vector2.up);
		}

		public Contact(RaycastHit2D hit)
		{
			point = hit.point;
			normal = hit.normal;
			collider = hit.collider;
			angle = Vector2.Angle(normal, Vector2.up);
		}



		public override bool Equals(object obj)
		{
			bool isContact = obj is Contact;
			if (false == isContact)
				return false;

			Contact other = (Contact)obj;
			return other.point == point;
		}

		public override int GetHashCode() => base.GetHashCode();
	}
}