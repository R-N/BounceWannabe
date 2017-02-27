using System;

namespace GtkTest
{
	public class Spike : Behaviour
	{
		public Spike (Vector2 pos, float w, float h) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			float wPer2 = w / 2.0f;
			float hPer2 = h / 2.0f;
			Vector2[] points = new Vector2[] {
				new Vector2 (0, hPer2),
				new Vector2 (wPer2, -hPer2),
				new Vector2 (-wPer2, -hPer2)
			};
			gameObject.collider = new ConvexCollider (gameObject, points);
			gameObject.renderer = new PolygonRenderer (gameObject, points);
			gameObject.renderer.SetColor (0.75, 0.75, 0.75);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
		}


		public override void OnCollisionEnter(Collision col){
			if (col.collider.gameObject.name == "Player") {
				Player.instance.GoToCheckpoint ();
			}
		}
	}
}

