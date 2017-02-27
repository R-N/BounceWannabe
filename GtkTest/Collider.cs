using System;
using System.Collections.Generic;

namespace GtkTest
{
	public class Collider 
	{
		public bool enabled = true;
		public GameObject gameObject = null;
		public static int count = 0;

		public bool isTrigger = false;

		public static List<Collider> allCollider = new List<Collider>();

		public Vector2 pivot = Vector2.zero;

		public float inertiaConstant = 0;

		public bool hacked = false;
		public Collider ()
		{
		}
		public virtual float GetInertia(float mass){
			return 0;
		}


		public bool IsPointInColliderNonVir(Vector2 point, bool isPointLocal = false){
			if (enabled && gameObject.enabled)
				return IsPointInCollider(point, isPointLocal);
			return false;
		}
		public virtual bool IsPointInCollider(Vector2 point, bool isPointLocal = false){
			return false;
		}

		public virtual Vector2 GetNormal(Vector2 point, bool isPointLocal = false){
			return Vector2.zero;
		}
		public virtual Vector2 GetLocalNormal(Vector2 point, bool isPointLocal = false){
			return Vector2.zero;
		}

		public virtual float area{
			get{
				return 0;
			}
		}
	}
}

