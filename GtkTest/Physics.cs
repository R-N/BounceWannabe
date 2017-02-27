using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace GtkTest
{
	public static class Physics
	{
		public static Vector2 gravity = new Vector2(0, -19.62f);
		public static float groundedVelocityThreshold = -0.2635143f;

		public static void Update(){

			int b = GameObject.allGameObjects.Count;
			//behaviour late updates
			for (int i = 0; i < b; i++) {
				GameObject.allGameObjects [i].FixedUpdate ();
			}
			int c = Rigidbody.allRigidbodies.Count;
			for (int i = 0; i < c; i++) {
				Rigidbody.allRigidbodies [i].FixedUpdate ();
			}
			//behaviour late updates
			for (int i = 0; i < b; i++) {
				GameObject.allGameObjects [i].FixedUpdate2 ();
			}

			int d = Collider.allCollider.Count;
			for (int i = d - 1; i >= 0; i--) {


				if (Collider.allCollider[i] == null) {
					Collider.allCollider.RemoveAt (i);
					Collider.count--;
					continue;
				}
			}

			//double getctTime = 0;
			//double ucTime = 0;
			d = Collider.allCollider.Count;

			for (int i = d - 1; i >= 0; i--) {
				Collider k = Collider.allCollider [i];
				//Stopwatch s0 = Stopwatch.StartNew ();
				List<Collision>[] ret = GetCollisionsAndTriggers (k);
				//s0.Stop ();
				//getctTime += s0.Elapsed.TotalSeconds;
				if (ret[0].Count > 0)
					k.gameObject.GetCollision (ret[0]);
				if (ret [1].Count > 0)
					k.gameObject.GetTrigger (ret [1]);
				//s0.Restart ();
				k.gameObject.UpdateCollisions ();
				/*s0.Stop ();
				ucTime += s0.Elapsed.TotalSeconds;*/
			}

			/*Console.WriteLine ("getcttime " + getctTime);
			Console.WriteLine ("uctime " + ucTime);
*/
			//behaviour late updates
			for (int i = 0; i < b; i++) {
				GameObject.allGameObjects [i].FixedUpdate3 ();
			}
		}

		public static Collision CheckCollision(Collider col){

			if (col.isTrigger)
				return null;
			int c = Collider.allCollider.Count;
			for (int i = 0; i < c; i++) {
				Collider k = Collider.allCollider [i];

				Vector2 ret = Vector2.zero;
				if (k != col && !k.isTrigger) {
					Collision s = col.CheckIntersection(k);
					if (s == null)
						continue;
					s.collider = k;
					return s;
				}
			}
			return null;
		}


		public static Collision CheckIntersection(this Collider a, Collider b){
			if (a == b)
				return null;
			if (!a.enabled || !b.enabled || !a.gameObject.enabled || !b.gameObject.enabled)
				return null;
			if ((a.gameObject.rigidbody.isKinematic || a.isTrigger) && (b.gameObject.rigidbody.isKinematic && b.isTrigger))
				return null;
			Type aT = a.GetType ();
			Type bT = b.GetType ();
			if (aT == typeof(CircleCollider) && bT == typeof(CircleCollider)) {
				return CircleCircleIntersection ((CircleCollider)a, (CircleCollider)b);
			} else if (aT == typeof(CircleCollider) && bT == typeof(ConvexCollider)) {
				return CircleConvexIntersection ((CircleCollider)a, (ConvexCollider)b);

			} else if (aT == typeof(ConvexCollider) && bT == typeof(CircleCollider)) {
				return CircleConvexIntersection ((CircleCollider)b, (ConvexCollider)a, false);
			} 
			return null;
		}

		public static Collision CircleCircleIntersection(CircleCollider a, CircleCollider b){
			
			Vector2 delta = b.gameObject.transform.position - a.gameObject.transform.position;
			float realRadA = a.gameObject.transform.LocalToWorldLength (a.radius);
			float realRadB = b.gameObject.transform.LocalToWorldLength (b.radius);
			float mag = delta.magnitude;
			if (Vector2.HasNaN (delta))
				a.gameObject.Throw ("delta has nan \n "
					+ a.gameObject.name + " apos " + a.gameObject.transform.position
					+ "\n " + b.gameObject.name + " bpos " + b.gameObject.transform.position);
			bool isincol = b.IsPointInCollider (a.gameObject.transform.position);
			if (mag > (realRadA + realRadB) && !isincol)
				return null;
			Collision s = new Collision ();
			s.collider = b;
			s.rejection = (realRadA + realRadB - mag);
			if (float.IsNaN (s.rejection))
				a.gameObject.Throw ("s.rejection is nan\nrealrada " + realRadA + "\nrealradb " + realRadB + "\nmag " + mag);

			s.point = a.gameObject.transform.position + delta * (realRadA - s.rejection/2) / mag;
			s.normal = b.GetNormal (s.point);
			float x = (realRadA + realRadB - mag) / 2;
			s.area = (float)(Math.PI *  x * realRadB * Math.Sqrt (1.0 - (realRadB - x) * (realRadB - x)/ realRadB/realRadB));

			if (isincol) {
				if (float.IsNaN (s.area))
					s.area = a.area;
				else
				s.area = a.area - s.area;
			}
			if (s.area == 0)
				s.area = MyMath.dFloat;

			return s;

		}

		public static Collision  ConvexConvexIntersection(ConvexCollider a, ConvexCollider b){
			Vector2 ret = Vector2.zero;
			int count = 0;
			for (int i = 0; i < a.len; i++) {
				Vector2 w = a.gameObject.transform.LocalToWorldPoint (a.points [i]);
				if (b.IsPointInCollider (w)) {
					ret += w;
					count++;
				}
			}
			for (int i = 0; i < b.len; i++) {
				Vector2 w = b.gameObject.transform.LocalToWorldPoint (b.points [i]);
				if (a.IsPointInCollider (w)) {
					ret += w;
					count++;
				}
			}
			if (count > 0) {
				Collision s = new Collision ();
				s.point = ret / count;
				s.normal = a.GetNormal (s.point);
				return s;
			}
			return null;
		}

		public static Collision CircleConvexIntersection (CircleCollider a, ConvexCollider b, bool ImCircle = true){
			Vector2 localPoint = b.gameObject.transform.WorldToLocalPoint(a.gameObject.transform.position);
			Vector2 ret = Vector2.zero;
			Vector2 nor = Vector2.zero;
			int count = 0;
			float area = 0;
			float realRadA = a.gameObject.transform.LocalToWorldLength (a.radius);
			float radALocalToB = b.gameObject.transform.WorldToLocalLength (realRadA);
			float sqrRadALocalToB = radALocalToB * radALocalToB;

			Vector2 a0 = localPoint - b.points [b.len - 1];
			float proj0 = Vector2.Dot (a0, b.deltas[0]) / b.lengths [0];
			float rej0 = Math.Abs (Vector2.Cross (a0, b.deltas [0]) / b.lengths [0]);
			float rejection = 0;
			if (rej0 <= radALocalToB && 0 <= proj0 && proj0 <= b.lengths [0]) {
				//ret += b.points [b.len - 1] + b.deltas [0] * MyMath.Clamp(proj0 / b.lengths [0], 0, 1);
				Vector2 vrej = Vector2.Reject (a0, b.deltas [0]);
				float vrejMag = vrej.magnitude;
				float worldVrejMag = b.gameObject.transform.LocalToWorldLength (vrejMag);
				float darea = (float)(Math.PI * (realRadA - worldVrejMag) * worldVrejMag * Math.Sqrt (1.0 - worldVrejMag * worldVrejMag / realRadA / realRadA));
				nor += Vector2.Rotate(b.deltas[0].normalized, MyMath.FloatPIPer2) * darea;
				ret += (localPoint - vrej) * darea;
				area += darea;
				rejection = Math.Max (rejection, realRadA - worldVrejMag);
				count++;
			} else if (Vector2.SqrDistance (b.points [0], localPoint) <= sqrRadALocalToB){
				count++;
				float y = radALocalToB - Vector2.Distance (b.points [0], localPoint);

				Vector2 delta = b.points [0] - localPoint;
				Vector2 proji = Vector2.Project (b.deltas [0], delta);
				Vector2 projiPlus1 = Vector2.Project (b.deltas [1], delta);
				Vector2 reji = b.deltas [0] - proji;
				Vector2 rejiPlus1 = b.deltas [1] - projiPlus1;

				float darea = 0.5f * y * y * (reji.magnitude/proji.magnitude + rejiPlus1.magnitude / projiPlus1.magnitude);

				rejection = Math.Max (rejection, realRadA - b.gameObject.transform.LocalToWorldLength(delta.magnitude));
				ret += b.points [1] * darea;
				nor += (Vector2.Reject (a0, b.deltas [1]) + Vector2.Reject (localPoint - b.points [0], b.deltas [1])).normalized * darea;
				area += darea;
			}



			for (int i = 1; i < b.len-1; i++) {
				Vector2 ai = localPoint - b.points [i - 1];
				float proj = Vector2.Dot (ai, b.deltas [i]) / b.lengths [i];
				float rej = Math.Abs (Vector2.Cross (ai, b.deltas [i]) / b.lengths [i]);

				if (rej <= radALocalToB && 0 <= proj && proj <= b.lengths [i]) {
					Vector2 vrej = Vector2.Reject (ai, b.deltas [i]);
					float vrejMag = vrej.magnitude;
					float worldVrejMag = b.gameObject.transform.LocalToWorldLength (vrejMag);
					float darea = (float)(Math.PI * (realRadA - worldVrejMag) * worldVrejMag * Math.Sqrt (1.0 - worldVrejMag * worldVrejMag / realRadA / realRadA));
					nor += Vector2.Rotate (b.deltas [i].normalized, MyMath.FloatPIPer2) * darea;
					ret += (localPoint - vrej) * darea;
					area += darea;
					rejection = Math.Max (rejection, realRadA - worldVrejMag);
					count++;
				} else if (Vector2.SqrDistance (b.points [i], localPoint) <= sqrRadALocalToB) {
					count++;
					float y = radALocalToB - Vector2.Distance (b.points [i], localPoint);

					Vector2 delta = b.points [i] - localPoint;
					Vector2 proji = Vector2.Project (b.deltas [i], delta);
					Vector2 projiPlus1 = Vector2.Project (b.deltas [i+1], delta);
					Vector2 reji = b.deltas [i] - proji;
					Vector2 rejiPlus1 = b.deltas [i+1] - projiPlus1;

					float darea = 0.5f * y * y * (reji.magnitude/proji.magnitude + rejiPlus1.magnitude / projiPlus1.magnitude);

					rejection = Math.Max (rejection, realRadA - b.gameObject.transform.LocalToWorldLength(delta.magnitude));
					ret += b.points [i] * darea;
					nor += (Vector2.Reject (ai, b.deltas [i]) + Vector2.Reject (localPoint - b.points [i], b.deltas [i + 1])).normalized * darea;
					area += darea;
				}
			}

			int last = b.len - 1;
			Vector2 al = localPoint - b.points [last-1];
			float projl = Vector2.Dot (al, b.deltas[last]) / b.lengths [last];
			float rejl = Math.Abs (Vector2.Cross (al, b.deltas [last]) / b.lengths [last]);
			if (rejl <= radALocalToB && 0 <= projl && projl <= b.lengths [last]) {
				Vector2 vrej = Vector2.Reject (al, b.deltas [last]);
				float vrejMag = vrej.magnitude;
				float worldVrejMag = b.gameObject.transform.LocalToWorldLength (vrejMag);
				float darea = (float)(Math.PI * (realRadA - worldVrejMag) * worldVrejMag * Math.Sqrt (1.0 - worldVrejMag * worldVrejMag / realRadA / realRadA));
				nor += Vector2.Rotate(b.deltas[last].normalized, MyMath.FloatPIPer2) * darea;
				ret += (localPoint - vrej) * darea;
				area += darea;
				rejection = Math.Max (rejection, realRadA - worldVrejMag);
				count++;
			} else if (Vector2.SqrDistance (b.points [last], localPoint) <= sqrRadALocalToB){
				count++;
				float y = radALocalToB - Vector2.Distance (b.points [last], localPoint);

				Vector2 delta = b.points [last] - localPoint;
				Vector2 proji = Vector2.Project (b.deltas [last], delta);
				Vector2 projiPlus1 = Vector2.Project (b.deltas [0], delta);
				Vector2 reji = b.deltas [last] - proji;
				Vector2 rejiPlus1 = b.deltas [0] - projiPlus1;

				float darea = 0.5f * y * y * (reji.magnitude/proji.magnitude + rejiPlus1.magnitude / projiPlus1.magnitude);


				rejection = Math.Max (rejection, realRadA - b.gameObject.transform.LocalToWorldLength(delta.magnitude));
				ret += b.points [last] * darea;
				//nor += (Vector2.Reject (al, b.deltas [last]) + Vector2.Reject (localPoint - b.points [last], b.deltas [0])).normalized * darea;
				//nor +=
				area += darea;
			}

			bool inCol = b.IsPointInCollider (a.gameObject.transform.position);
			if (count > 0 || (inCol && b.isTrigger && b.hacked)) {
				if (inCol)
					area = a.area - area;
			

				if (area <= 0)
					area = MyMath.dFloat;
				Collision s = new Collision ();
				s.point = b.gameObject.transform.LocalToWorldPoint (ret / area);
				s.normal = (ImCircle ? -1 : 1) * a.GetNormal (s.point);
				//s.normal = (ImCircle ? -1 : 1) * b.gameObject.transform.LocalToWorldDirection(nor / area).normalized;

				if (!ImCircle)
					s.normal *= -1;

				//Console.WriteLine ("normal " + s.normal);
				s.area = area;
				s.collider = ImCircle ? (Collider)b : (Collider)a;
				s.rejection = rejection;
				//Console.WriteLine ("Collide " + MainWindow.frameCount);
				//Console.WriteLine ("normal " + s.normal);
				return s;
			}
			return null;
		}

		public static Vector2 LineLineIntersection(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2){
			Vector2 p2Minp1 = b2 - b1;
			Vector2 rMinp1 = a2 - b1;
			Vector2 v = Vector2.Project (rMinp1, p2Minp1);
			Vector2 q = b1 - a1 + v;
			float vMag = v.magnitude;
			if (vMag < 0 || p2Minp1.magnitude < vMag || Vector2.Cross (p2Minp1, rMinp1) > 0)
				return Vector2.nan;
			return q;
		}



		public static Vector2 PointToLine(Vector2 a1, Vector2 a2, Vector2 b){
			Vector2 d = a2 - a1;
			Vector2 e = b - a1;

			return -1 * Vector2.Reject (e, d);
		}


		public static List<Collision>[] GetCollisionsAndTriggers(this Collider col){
			List<Collision>[] ret = new List<Collision>[2];
			ret [0] = new List<Collision> ();
			ret [1] = new List<Collision> ();
			if (!col.enabled || !col.gameObject.enabled)
				return ret;
			int c = Collider.allCollider.Count;
			for (int i = c-1; i >= 0; i--) {
				Collider k = Collider.allCollider [i];
				if (k == null) {
					Collider.allCollider.RemoveAt (i);
					continue;
				}
				if (k != col) {
					Collision s = col.CheckIntersection (k);
					if (s != null) {
						s.isCollision = !k.isTrigger && !col.isTrigger;
						s.collider = k;
						if (s.isCollision)
							ret [0].Add (s);
						else
							ret [1].Add (s);
					}
				}
			}
				return ret;
		}

		public static List<Collision> GetCollisions(this Collider col){
			if (!col.enabled || !col.gameObject.enabled || col.isTrigger)
				return null;
			int c = Collider.allCollider.Count;
			List<Collision> ret = new List<Collision> ();
			for (int i = c-1; i >= 0; i--) {
				Collider k = Collider.allCollider [i];
				if (k == null) {
					Collider.allCollider.RemoveAt (i);
					continue;
				}
				if (k != col && !k.isTrigger) {
					Collision s = col.CheckIntersection (k);
					if (s != null) {
						s.isCollision = true;
						s.collider = k;
						ret.Add (s);
					}
				}
			}
			if (ret.Count > 0)
				return ret;
			return null;
		}
		public static List<Collision> GetTriggers(this Collider col){
			if (!col.enabled || !col.gameObject.enabled)
				return null;
			int c = Collider.allCollider.Count;
			List<Collision> ret = new List<Collision> ();
			for (int i = c-1; i >= 0; i--) {
				Collider k = Collider.allCollider [i];
				if (k == null) {
					Collider.allCollider.RemoveAt (i);
					continue;
				}
				if (k != col && (col.isTrigger || k.isTrigger)) {
					Collision s = col.CheckIntersection (k);
					if (s != null) {
						s.isCollision = false;
						s.collider = k;
						ret.Add (s);
					}
				}
			}
			if (ret.Count > 0)
				return ret;
			return null;
		}
	}

	public class Collision{
		public Collider collider;
		public Vector2 point;
		public Vector2 normal;
		public float area;
		public float rejection;
		public float[] additionalFloats;
		public bool isCollision = false;
		public Collision(){
		}
	}
}

