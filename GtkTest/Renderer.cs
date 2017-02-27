using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Cairo;
using System.IO;
using Gtk;

namespace GtkTest
{
	public class Renderer 
	{
		public bool useShadow = false;
		public ImageSurface surf = null;
		public bool enabled  = true;
		public GameObject gameObject = null;
		public static List<Renderer> allRenderers = new List<Renderer>();
		public System.Drawing.Color color = System.Drawing.Color.Black;
		public double ColorR = 0;
		public double ColorG = 0;
		public double ColorB = 0;
		public double ColorA = 1;
		public Vector2 minBound = Vector2.zero;
		public Vector2 maxBound = Vector2.zero;
		public Vector2 pivot = Vector2.zero;
		public Renderer ()
		{
			gameObject = new GameObject ();
			gameObject.renderer = this;
			allRenderers.Add (this);
		}
		public void SetColor(System.Drawing.Color c){
			color = c;
			ColorR = c.R / 256.0;
			ColorG = c.G / 256.0;
			ColorB = c.B / 256.0;
			ColorA = c.A / 256.0;
		}

		public void SetColor(double r, double g, double b, double a){
			ColorR = r;
			ColorG = g;
			ColorB = b;
			ColorA = a;
			color = System.Drawing.Color.FromArgb (
				(int)(a * 255), (int)(r * 255), (int)(g * 255), (int)(b * 255));
		}

		public void SetColor(double r, double g, double b){
			SetColor (r, g, b, 1);
		}

		public  void RenderSG(System.Drawing.Graphics g){
			if (enabled && gameObject.enabled)
				RealRenderSG (g);
		}
		public  void RenderCairo(Cairo.Context context){
			if (enabled && gameObject.enabled)
				RealRenderCairo (context);
		}

		public virtual void RealRenderSG(System.Drawing.Graphics g){
		}
		public virtual void RealRenderCairo(Cairo.Context context){
		}
	}
}

