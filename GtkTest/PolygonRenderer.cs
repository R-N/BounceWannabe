using System;
using System.Drawing;
using Cairo;

namespace GtkTest
{
	public class PolygonRenderer : Renderer
	{
		 PointF[] pointFs;
		public Vector2[] points;
		public int len;
		public PolygonRenderer (GameObject go, Vector2[] ps)
		{
			gameObject = go;
			go.renderer = this;
			allRenderers.Add (this);
			points = ps;
			len = ps.Length;
			pointFs = new PointF[len];

			float[] boundingBox = new float[4];
			boundingBox [0] = float.PositiveInfinity;
			boundingBox [1] = float.PositiveInfinity;
			boundingBox [2] = float.NegativeInfinity;
			boundingBox [3] = float.NegativeInfinity;

			foreach (Vector2 v in points) {
				pivot += v;
				boundingBox [0] = Math.Min (boundingBox [0], v.x);
				boundingBox [1] = Math.Min (boundingBox [1], v.y);
				boundingBox [2] = Math.Max (boundingBox [2], v.x);
				boundingBox [3] = Math.Max (boundingBox [3], v.y);
			}

			pivot /= ps.Length;

			minBound = new Vector2 (boundingBox [0], boundingBox [1]);
			maxBound = new Vector2 (boundingBox [2], boundingBox [3]);
		}

		 System.Drawing.PointF[] BuildPointFArray(){
			for (int i = 0; i < len; i++) {
				pointFs [i] = Camera.WorldToScreenPosition(gameObject.transform.LocalToWorldPoint (points [i])).ToPointF ();
			}
			return pointFs;
		}

		public override void RealRenderSG(System.Drawing.Graphics g){
			if (Camera.IsRendererOutOfScreen (this))
				return;
			Vector2 screenPos = Camera.WorldToScreenPosition (gameObject.transform.position);
			Brush b = new SolidBrush (color);
			g.FillPolygon (b, BuildPointFArray());
		}
		public override void RealRenderCairo(Cairo.Context context){
			//if (Camera.IsRendererOutOfScreen (this))
			//	return;
			context.MoveTo (Camera.WorldToScreenPosition(gameObject.transform.LocalToWorldPoint(points[len-1])).ToPointD());
			for (int i = 0; i < len; i++) {
				context.LineTo (Camera.WorldToScreenPosition (gameObject.transform.LocalToWorldPoint (points [i])).ToPointD());

			}
			context.ClosePath ();
			if (surf == null) {
				context.SetSourceRGBA (ColorR, ColorG, ColorB, ColorA);
				context.Fill ();
			} else {
				int w = surf.Width;
				int h = surf.Height;
				Vector2 screenMinBound = Camera.WorldToScreenPosition (gameObject.transform.LocalToWorldPoint(minBound));
				Vector2 screenMaxBound = Camera.WorldToScreenPosition (gameObject.transform.LocalToWorldPoint(maxBound));

				int myH = (int)(screenMinBound.y - screenMaxBound.y);


				int posY = (int)screenMaxBound.y;
				while (myH > 0) {
					int posX = (int)screenMinBound.x;
					int myW = (int)(screenMaxBound.x - screenMinBound.x);
					while (myW > 0) {
						if (Camera.IsPointOutOfScreen(new Vector2(posX, posY))
							&& Camera.IsPointOutOfScreen(new Vector2(posX, posY + h))
							&& Camera.IsPointOutOfScreen(new Vector2(posX + w, posY))
							&& Camera.IsPointOutOfScreen(new Vector2(posX + w, posY + h))){

							myW -= w;
							posX += w;
							continue;
						}

						context.SetSourceSurface (surf, posX, posY);
						context.FillPreserve ();
						myW -= w;
						posX += w;
					}
					myH -= h;
					posY += h;
				}
				context.SetSourceRGBA (ColorR, ColorG, ColorB, ColorA);
				context.Stroke ();
				//context.NewPath();
				context.GetSource ().Dispose ();
			}
		}
	}
}

