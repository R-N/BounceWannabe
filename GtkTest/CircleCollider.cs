using System;

namespace GtkTest
{
	public class CircleCollider : Collider
	{
		float _radius;
		public float sqrRadius;

		public float radius{
			get{
				return _radius;
			}
			set{
				_radius = value;
				sqrRadius = _radius * _radius;
			}
		}

		public CircleCollider (GameObject go, float r) 
		{
			gameObject = go;
			go.collider = this;
			radius = r;
			Collider.allCollider.Add (this);
			Collider.count++;
			inertiaConstant = 0.5f;
		}

		public override float area{
			get{
				return MyMath.FloatPI * radius * radius;
			}
		}

		public override float GetInertia (float mass)
		{
			return inertiaConstant * mass * _radius * _radius;
		}

		public override bool IsPointInCollider(Vector2 point, bool isPointLocal = false){
			if (isPointLocal)
				return point.sqrMagnitude <= sqrRadius;
			return gameObject.transform.WorldToLocalPoint(point).sqrMagnitude <= sqrRadius;
		}

		public override Vector2 GetNormal (Vector2 point, bool isPointLocal = false)
		{
			if (isPointLocal)
				return gameObject.transform.LocalToWorldDirection(point.normalized);
			return gameObject.transform.LocalToWorldDirection(gameObject.transform.WorldToLocalPoint (point).normalized);
		}
		public override Vector2 GetLocalNormal (Vector2 point, bool isPointLocal = false)
		{
			if (isPointLocal)
				return point.normalized;
			return gameObject.transform.WorldToLocalPoint (point).normalized;
		}
	}
}

