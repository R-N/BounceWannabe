using System;

namespace GtkTest
{
	public static class MyMath
	{
		public static float Clamp(float x, float a, float b){
			if (x < a)
				return a;
			if (x > b)
				return b;
			return x;
		}
		public static float MoveTowards(float a, float b, float maxDelta){
			float mag = Math.Abs (b - a);
			if (maxDelta > mag) {
				return a + (b > a ? 1 : -1) * maxDelta;
			}
			return b;
		}

		public static float Normalize(float a){
			return (a > 0 ? 1 : (a < 0 ? -1 : 0));
		}

		public static float MoveTowardsUnclamped(float a, float b, float maxDelta){
			return a + (b > a ? 1 : -1) * maxDelta;
		}

		public static float FixAngleRad(float rad){
			while (rad > TwoPI) {
				rad -= TwoPI;
			}
			while (rad < 0){
				rad += TwoPI;
			}
			return rad;
		}
		public static double FixAngleRad(double rad){
			while (rad > TwoPI) {
				rad -= TwoPI;
			}
			while (rad < 0){
				rad += TwoPI;
			}
			return rad;
		}

		public static float FloatPI = (float)Math.PI;
		public static float FloatPIPer2 = FloatPI/2;
		public static float Deg2Rad = FloatPI / 180.0f;
		public static float Rad2Deg = 180.0f/FloatPI;
		public static float TwoPI = 2*FloatPI;
		public static float dFloat = (float)Math.Pow(2, -62);


		public static Vector2 ToVector2(this System.Drawing.PointF p){
			return new Vector2 (p.X, p.Y);
		}
		public static Vector2 ToVector2(this Cairo.PointD p){
			return new Vector2 (p.X, p.Y);
		}

		public static Vector2[] ToVector2Array(this System.Drawing.PointF[] ps){
			int len = ps.Length;
			Vector2[] ret = new Vector2[len];
			for (int i = 0; i < len; i++) {
				ret [i] = ps [i].ToVector2 ();
			}
			return ret;
		}


		public static Vector2[] ToVector2Array(this Cairo.PointD[] ps){
			int len = ps.Length;
			Vector2[] ret = new Vector2[len];
			for (int i = 0; i < len; i++) {
				ret [i] = ps [i].ToVector2 ();
			}
			return ret;
		}


		public static System.Drawing.PointF[] ToPointFArray(this Vector2[] ps){
			int len = ps.Length;
			System.Drawing.PointF[] ret = new System.Drawing.PointF[len];
			for (int i = 0; i < len; i++) {
				ret [i] = ps [i].ToPointF ();
			}
			return ret;
		}
		public static Cairo.PointD[] ToPointDArray(this Vector2[] ps){
			int len = ps.Length;
			Cairo.PointD[] ret = new Cairo.PointD[len];
			for (int i = 0; i < len; i++) {
				ret [i] = ps [i].ToPointD ();
			}
			return ret;
		}
	}
}

