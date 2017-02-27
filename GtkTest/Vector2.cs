using System;
using System.Drawing;
using Cairo;
using System.Linq;
using GtkTest;

namespace GtkTest
{
	public struct Vector2
	{

		public float x;
		public float y;

		public static Vector2 zero = new Vector2(0, 0);
		public static Vector2 one = new Vector2(1,1);
		public static Vector2 up = new Vector2(0,1);
		public static Vector2 down = new Vector2(0,-1);
		public static Vector2 right = new Vector2 (1,0);
		public static Vector2 left = new Vector2(-1,0);
		public static Vector2 nan = new Vector2(float.NaN, float.NaN);
		public static Vector2 infinity = new Vector2(1.0f/0.0f, 1.0f/0.0f);
		public static Vector2 negativeInfinity = new Vector2(-1.0f/0.0f, -1.0f/0.0f);

		public Vector2 (float x, float y){
			this.x = x;
			this.y = y;
		}
		public Vector2 (double x, double y){
			this.x = (float)x;
			this.y = (float)y;
		}

		public float sqrMagnitude {
			get{
				return x * x + y * y;
			}
		}

		public static bool HasNaN(Vector2 v){
			return (float.IsNaN (v.x) || float.IsNaN (v.y));
		}
		public static bool HasInfinity(Vector2 v){
			return (float.IsInfinity (v.x) || float.IsInfinity (v.y));
		}

		public float magnitude{
			get{
				return (float)Math.Sqrt ((double)(x * x + y * y));;
			}
		}

		public void Normalize(){
			float mag = this.magnitude;
				if (mag == 0 || float.IsNaN(mag))
				return;
			x /= mag;
			y /= mag;
		}

		public Vector2 normalized {
			get {
				float mag = this.magnitude;
				if (mag == 0)
					return this;
				return new Vector2 (x / mag, y / mag);
			}
		}

		public float mean {
			get {
				return (x + y) / 2;
			}
		}

		public static  Vector2 operator +(Vector2 a, Vector2 b){
			return new Vector2 (a.x + b.x, a.y + b.y);
		}

		public static  Vector2 operator -(Vector2 a, Vector2 b){
			return new Vector2 (a.x - b.x, a.y - b.y);
		}

		public static  Vector2 operator *(Vector2 a, Vector2 b){
			return new Vector2 (a.x * b.x, a.y * b.y);
		}

		public static  Vector2 operator /(Vector2 a, Vector2 b){
			return new Vector2 (a.x / b.x, a.y / b.y);
		}


		public static  Vector2 operator *(Vector2 a, float b){
			return new Vector2 (a.x * b, a.y * b);
		}
		public static  Vector2 operator *(Vector2 a, int b){
			return new Vector2 (a.x * b, a.y * b);
		}
		public static  Vector2 operator *(Vector2 a, double b){
			return new Vector2 (a.x * b, a.y * b);
		}
		public static  Vector2 operator *(float b, Vector2 a){
			return new Vector2 (a.x * b, a.y * b);
		}
		public static  Vector2 operator *(int b, Vector2 a){
			return new Vector2 (a.x * b, a.y * b);
		}
		public static  Vector2 operator *(double b, Vector2 a){
			return new Vector2 (a.x * b, a.y * b);
		}

		public static  Vector2 operator /(Vector2 a, float b){
			return new Vector2 (a.x / b, a.y / b);
		}
		public static  Vector2 operator /(Vector2 a, int b){
			return new Vector2 (a.x / b, a.y / b);
		}
		public static  Vector2 operator /(Vector2 a, double b){
			return new Vector2 (a.x / b, a.y / b);
		}


		public static  Vector2 operator /(float a, Vector2 b){
			return new Vector2 (a /b.x, a/b.y);
		}
		public static  Vector2 operator /(int a, Vector2 b){
			return new Vector2 (a /b.x, a/b.y);
		}
		public static  Vector2 operator /(double a, Vector2 b){
			return new Vector2 (a /b.x, a/b.y);
		}

		public static  Vector2 operator -(Vector2 a){
			return -1*a;
		}
		public static  bool operator ==(Vector2 a, Vector2 b){
			return (a.x == b.x && a.y == b.y);
		}
		public static bool operator !=(Vector2 a, Vector2 b){
			return (a.x != b.x || a.y != b.y);
		}

		public static float SqrDistance(Vector2 a, Vector2 b){
			return (a-b).sqrMagnitude;
		}
		public static float Distance(Vector2 a, Vector2 b){
			return (a-b).magnitude;
		}

		public static float Dot(Vector2 a, Vector2 b){
			return a.x * b.x + a.y * b.y;
		}
		public static float Cross(Vector2 a, Vector2 b){
			return a.x * b.y - b.x * a.y;
		}
		public static float CrossNormalized(Vector2 a, Vector2 b){
			float cross = a.x * b.y - b.x * a.y;
			return (cross > 0 ? 1 : (cross < 0 ? -1 : 0));
		}

		public static Vector2 ClampMagnitude(Vector2 a, float mag){
			float mag0  = a.magnitude;
			if (mag0 > mag)
				return a / mag0;
			else
				return a;
		}

		public static Vector2 Project (Vector2 a, Vector2 b){
			if (b == Vector2.zero)
				return Vector2.zero;
			float mag = b.magnitude;
			if (mag <= 0)
				return Vector2.zero;
			if (float.IsNaN (mag))
				Console.WriteLine ("mag is nan");
			return b * Dot (a, b) / mag / mag;
		}

		public static Vector2 Reject(Vector2 a, Vector2 b){
			if (b == Vector2.zero)
				return Vector2.zero;
			float mag = b.magnitude;
			if (mag <= 0)
				return Vector2.zero;
			if (float.IsNaN (mag))
				Console.WriteLine ("mag is nan");
			return a - b * Dot (a, b) / mag / mag;
		}

		public static Vector2 MoveTowards(Vector2 a, Vector2 b, float maxDelta){
			Vector2 d = b - a;
			return a + d.normalized * Math.Min (maxDelta, d.magnitude);
		}
		public static Vector2 MoveTowardsUnclamped(Vector2 a, Vector2 b, float delta){
			return a + (b-a).normalized * delta;
		}


		public static Vector2 Lerp(Vector2 a, Vector2 b, float t){
			return a + (b - a) * MyMath.Clamp (t, 0, 1);
		}
		public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t){
			return a + (b - a) * t;
		}



		public override string ToString(){
			return "(" + x + ", " + y + ")";
		}

		public static Vector2 Rotate(Vector2 p, float rad){
			double cos = Math.Cos (rad);
			double sin = Math.Sin (rad);
			return new Vector2 (p.x * cos - p.y * sin, p.x * sin + p.y * cos);
		}

		public static Vector2 Rotate(Vector2 p, Vector2 pivot, float rad){
			return pivot + Rotate (p - pivot, rad);
		}
		public System.Drawing.PointF ToPointF(){
			return new System.Drawing.PointF (x, y);
		}
		public Cairo.PointD ToPointD(){
			return new Cairo.PointD ((double)x, (double)y);
		}




		public static  System.Drawing.PointF operator +(System.Drawing.PointF a, Vector2 b){
			return new System.Drawing.PointF (a.X + b.x, a.Y + b.y);
		}
		public static  System.Drawing.PointF operator +(Vector2 a, System.Drawing.PointF b){
			return new System.Drawing.PointF (a.x + b.X, a.x + b.Y);
		}

		public static  System.Drawing.PointF operator -(System.Drawing.PointF a, Vector2 b){
			return new System.Drawing.PointF (a.X - b.x, a.Y - b.y);
		}
		public static  System.Drawing.PointF operator -(Vector2 a, System.Drawing.PointF b){
			return new System.Drawing.PointF (a.x - b.X, a.y - b.Y);
		}

		public static  Cairo.PointD operator +(Cairo.PointD a, Vector2 b){
			return new Cairo.PointD (a.X + b.x, a.Y + b.y);
		}
		public static  Cairo.PointD operator +(Vector2 a, Cairo.PointD b){
			return new Cairo.PointD (a.x + b.X, a.x + b.Y);
		}

		public static  Cairo.PointD operator -(Cairo.PointD a, Vector2 b){
			return new Cairo.PointD (a.X - b.x, a.Y - b.y);
		}
		public static  Cairo.PointD operator -(Vector2 a, Cairo.PointD b){
			return new Cairo.PointD (a.x - b.X, a.y - b.Y);
		}


		/*public static  Cairo.PointD operator +(System.Drawing.PointF a, Cairo.PointD b){
			return new Cairo.PointD (a.X + b.Y, a.Y + b.Y);
		}
		public static  Cairo.PointD operator +(Cairo.PointD a, System.Drawing.PointF b){
			return new Cairo.PointD (a.Y + b.X, a.Y + b.Y);
		}

		public static  Cairo.PointD operator -(System.Drawing.PointF a, Cairo.PointD b){
			return new Cairo.PointD (a.X - b.X, a.Y - b.Y);
		}
		public static  Cairo.PointD operator -(Cairo.PointD a, System.Drawing.PointF b){
			return new Cairo.PointD (a.X - b.X, a.Y - b.Y);
		}*/
	}
}

