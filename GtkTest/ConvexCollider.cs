using System;

namespace GtkTest
{
	public class ConvexCollider : Collider
	{
		public Vector2[] points;
		public Vector2[] deltas;
		public Vector2[] pivotizedPoints;
		public float[] lengths;
		public int len;

		public ConvexCollider (GameObject go, Vector2[] ps)
		{
			gameObject = go;
			points = ps;
			go.collider = this;
			Collider.allCollider.Add (this);
			Collider.count++;
			len = ps.Length;

			deltas = new Vector2[len];
			deltas [0] = points [0] - points [len - 1];

			lengths = new float[len];
			lengths [0] = deltas [0].magnitude;
			pivot = points [0];
			for (int i = 1; i < len; i++) {
				pivot += points [i];
				deltas [i] = points [i] - points [i - 1];
				lengths [i] = deltas [i].magnitude;
			}
			pivot /= len;
			if (pivot == Vector2.zero)
				pivotizedPoints = points;
			else {
				pivotizedPoints = new Vector2[len];
				for (int i = 0; i < len; i++) {
					pivotizedPoints [i] = points [i] - pivot;
				}
			}


		}

		public override float GetInertia (float mass)
		{
			int last = len - 1;
			float bot = Math.Abs (Vector2.Cross (pivotizedPoints [last], pivotizedPoints [0]));
			float top = bot * (
			                Vector2.Dot (pivotizedPoints [last], pivotizedPoints [last]) +
			                Vector2.Dot (pivotizedPoints [last], pivotizedPoints [0]) +
			                Vector2.Dot (pivotizedPoints [0], pivotizedPoints [0]));

			for (int i = 1; i < len; i++) {
				float cross = Math.Abs (Vector2.Cross (pivotizedPoints [i - 1], pivotizedPoints [i]));
				bot += cross;
				top += cross  * (
					Vector2.Dot (pivotizedPoints [i-1], pivotizedPoints [i-1]) +
					Vector2.Dot (pivotizedPoints [i-1], pivotizedPoints [i]) +
					Vector2.Dot (pivotizedPoints [i], pivotizedPoints [i]));
			}
			return mass / 6 * top / bot;
		}

		public override bool IsPointInCollider (Vector2 point, bool isPointLocal = false)
		{
			Vector2 localPoint;
			if (isPointLocal)
				localPoint = point;
			localPoint = gameObject.transform.WorldToLocalPoint (point);
			for (int i = 1; i < len; i++) {
				if (Vector2.Cross (points [i] - points [i - 1], localPoint - points [i - 1]) > 0)
					return false;
			}
			return true;
		}

		public override Vector2 GetNormal (Vector2 point, bool isPointLocal = false)
		{

			return base.GetNormal (point, isPointLocal);
		}
	}
}

