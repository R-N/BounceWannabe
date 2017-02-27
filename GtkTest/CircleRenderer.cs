using System;
using Gtk;
using System.Drawing;
using Cairo;

namespace GtkTest
{
	public class CircleRenderer : Renderer
	{
		
		public float radius = 1;
		public CircleRenderer (GameObject go, float s)
		{
			gameObject = go;
			go.renderer = this;
			allRenderers.Add (this);
			radius = s;
			minBound = new Vector2 (-radius, -radius);
			maxBound = new Vector2 (radius, radius);
		}


		public override void RealRenderSG(System.Drawing.Graphics g){
			if (Camera.IsRendererOutOfScreen (this))
				return;
			Vector2 screenPos = Camera.WorldToScreenPosition(gameObject.transform.position);
			Brush b = new SolidBrush (color);
			float radPerZoom = gameObject.transform.LocalToWorldLength (radius) *Camera.instance.zoom;
			g.FillEllipse (b, screenPos.x - radPerZoom,
				screenPos.y - radPerZoom, 
				2 * radPerZoom, 2 * radPerZoom);
		}
		public override void RealRenderCairo(Cairo.Context context){
			if (Camera.IsRendererOutOfScreen (this))
				return;
			Vector2 screenPos = Camera.WorldToScreenPosition(gameObject.transform.position);
			float realRad = gameObject.transform.LocalToWorldLength (radius);
			double screenRad = (double)realRad * Camera.instance.zoom;
			context.Arc ((double)screenPos.x, 
				(double)screenPos.y,
				screenRad, 
				0, 2 * Math.PI);
			

			if (surf == null) {
				if (useShadow) {
					LinearGradient g = new LinearGradient (screenPos.x - screenRad, screenPos.y + screenRad, screenPos.x + screenRad, screenPos.y - screenRad);

					//g.AddColorStopRgb (0.75, new Cairo.Color (ColorR, ColorG, ColorB, ColorA));
					g.AddColorStop (0, new Cairo.Color (ColorR * 0.5, ColorG * 0.5, ColorB * 0.5, ColorA * 0.5));
					g.AddColorStop (1, new Cairo.Color (ColorR, ColorG, ColorB, ColorA));

					context.SetSource (g);
					context.Fill ();
					g.Dispose ();
				} else {
					context.SetSourceRGBA (ColorR, ColorG, ColorB, ColorA);
					context.Fill ();
				}
			}else{
				context.SetSourceSurface (surf, (int)(screenPos.x - screenRad), (int)(screenPos.y - screenRad));

				context.Fill ();
				context.GetSource ().Dispose ();
				//surf.Dispose ();
			}
			/*context.SetSourceRGBA (1,1,1,1);
			context.Arc ((double)screenPos.x, 
				(double)screenPos.y,
				(double)realRad * Camera.instance.zoom, 
				MyMath.FixAngleRad(-Math.PI/4 - gameObject.transform.rotation), MyMath.FixAngleRad(Math.PI /4 - gameObject.transform.rotation));
			
			context.Fill ();*/
		}


	}
}

