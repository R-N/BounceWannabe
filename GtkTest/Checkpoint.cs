using System;

namespace GtkTest
{
	public class Checkpoint : Behaviour
	{
		public Checkpoint (Vector2 pos) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			float wPer2 = 0.25f;
			float hPer2 = 2;
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
			gameObject.collider.isTrigger = true;
			gameObject.renderer.SetColor (1, 1, 0);
		}

		public override void OnTriggerEnter(Collision col){
			if (col.collider.gameObject.name == "Player") {
				Player.checkPoint = gameObject.transform.position;
				gameObject.renderer.SetColor (0, 1, 0);
			}
		}
	}
}

