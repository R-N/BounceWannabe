using System;

namespace GtkTest
{
	public class Wall : Behaviour
	{
		public Wall (Vector2 pos, float w, float h) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			float wPer2 = w / 2.0f;
			float hPer2 = h / 2.0f;
			Vector2[] points = new Vector2[] {
				new Vector2 (-wPer2, hPer2),
				new Vector2 (wPer2, hPer2),
				new Vector2 (wPer2, -hPer2),
				new Vector2 (-wPer2, -hPer2)
			};
			gameObject.collider = new ConvexCollider (gameObject, points);
			gameObject.renderer = new PolygonRenderer (gameObject, points);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
		}
		public Wall (Vector2 pos, Vector2[] points) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			gameObject.collider = new ConvexCollider (gameObject, points);
			gameObject.renderer = new PolygonRenderer (gameObject, points);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
		}
		public override void Update3(){
			if (gameObject.renderer.surf != null)
				gameObject.renderer.surf.Dispose ();
			gameObject.renderer.surf = new Cairo.ImageSurface ("Resources/Drawable/brick.png");
		}

	}
}

