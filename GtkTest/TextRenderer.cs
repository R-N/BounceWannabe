using System;
using Gtk;
using System.Drawing;
using Cairo;
using Pango;

namespace GtkTest
{
	public class TextRenderer : Renderer
	{
		public string text;
		public float size;
		public TextRenderer (GameObject go, string s, float si)
		{
			gameObject = go;
			go.renderer = this;
			allRenderers.Add (this);
			text = s;
			size = si;
			SetColor (1, 1, 1);
			float realSize = size * 0.01f;
			minBound.x = 0;
			minBound.y = -realSize;
			maxBound.y = 0;
			maxBound.x = text.Length * realSize;
		}
		public override void RealRenderCairo(Cairo.Context context){
			if (Camera.IsRendererOutOfScreen (this))
				return;
			Vector2 screenPos = Camera.WorldToScreenPosition(gameObject.transform.position);
			double realSize = (double)gameObject.transform.LocalToWorldLength (size);
			context.MoveTo (screenPos.ToPointD ());
			context.SetSourceRGBA (ColorR, ColorG, ColorB, ColorA);
			context.SetFontSize (realSize);
			context.TextPath (text);
			context.Fill ();
		}


	}
}

