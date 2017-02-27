using System;

namespace GtkTest
{
	public class Transform
	{
		public GameObject gameObject = null;
		public Vector2 position = new Vector2();
		public Vector2 localScale = Vector2.one;
		float _rotation = 0;

		public float rotation {
			get {
				return _rotation;
			}
			set {
				_rotation = value;
				while (_rotation < 0) {
					_rotation += MyMath.TwoPI;
				}
				while (_rotation > MyMath.TwoPI) {
					_rotation -= MyMath.TwoPI;
				}
			}
		}


		public Transform ()
		{
			gameObject = new GameObject ();
			gameObject.transform = this;
		}

		public Transform(GameObject go){
			gameObject = go;
			gameObject.transform = this;
		}

		public void RotateAround(Vector2 pivot, float rad, bool isPivotLocal = false){
			if (isPivotLocal) {
				position += Vector2.Rotate (Vector2.zero, pivot, rad);
			} else {
				position = Vector2.Rotate (position, pivot, rad);
			}
			rotation = rotation + rad;
		}

		public Vector2 forward {
			get {
				return Vector2.Rotate (Vector2.right, rotation);
			}
		}


		public Vector2 WorldToLocalPoint (Vector2 point){
			return Vector2.Rotate(new Vector2((point.x - position.x)/localScale.x, (point.y - position.y)/localScale.y), -rotation);
		}

		public Vector2 LocalToWorldPoint(Vector2 point){
			return position + Vector2.Rotate(point * localScale, rotation);
		}

		public Vector2 WorldToLocalDirection(Vector2 dir){
			return Vector2.Rotate(dir, -rotation);
		}

		public Vector2 LocalToWorldDirection (Vector2 dir){
			return Vector2.Rotate(dir, rotation);
		}

		public Vector2 WorldToLocalVector (Vector2 dir){
			return Vector2.Rotate(dir / localScale, -rotation);
		}
		public Vector2 LocalToWorldVector (Vector2 dir){
			return Vector2.Rotate(dir * localScale, rotation);
		}
		public float WorldToLocalLength (float len){
			return len / localScale.mean;
		}
		public float LocalToWorldLength (float len){
			return len * localScale.mean;
		}
	}
}

