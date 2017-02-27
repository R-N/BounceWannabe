using System;

namespace GtkTest
{
	public class Water : Behaviour
	{
		public float density = 1;
		public Water (Vector2 pos, float w, float h) 
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
			gameObject.collider.isTrigger = true;
			gameObject.collider.hacked = true;
			gameObject.renderer = new PolygonRenderer (gameObject, points);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
		}
		public Water (Vector2 pos, Vector2[] points) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			gameObject.collider.isTrigger = true;
			gameObject.collider.hacked = true;
			gameObject.collider = new ConvexCollider (gameObject, points);
			gameObject.renderer = new PolygonRenderer (gameObject, points);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
		}

		public override void OnTriggerEnter(Collision col){

			if (!col.collider.isTrigger && col.collider.gameObject.rigidbody != null && !col.collider.gameObject.rigidbody.isKinematic) {
				int c = (int)(col.collider.gameObject.rigidbody.velocity.magnitude * col.collider.gameObject.rigidbody.mass / density / 10);
				Random rand = new Random ();
				for (int i = 0; i < c; i++) {
					WaterDrop wd = (WaterDrop)PoolManager.Insantiate<WaterDrop> ();
					if (wd == null)
						wd = new WaterDrop ();
					wd.Reset (col.point, new Vector2 (rand.NextDouble () - 0.5, rand.NextDouble () * 0.5).normalized * rand.NextDouble () * col.collider.gameObject.rigidbody.velocity.magnitude, (float)rand.NextDouble () * 0.5f);

				}
				col.collider.gameObject.rigidbody.pendingDragMultiply += 0.5f;
			} 
		}

		public override void OnTriggerStay(Collision col){
			
			if (!col.collider.isTrigger && col.collider.gameObject.rigidbody != null && !col.collider.gameObject.rigidbody.isKinematic) {
				col.collider.gameObject.rigidbody.velocity += Vector2.up * density * col.area / col.collider.gameObject.rigidbody.mass * MainWindow.fixedDeltaTime;
				col.collider.gameObject.rigidbody.pendingDragMultiply += 0.5f;
			} 
		}

		public override void OnTriggerExit(Collision col){

			if (!col.collider.isTrigger && col.collider.gameObject.rigidbody != null && !col.collider.gameObject.rigidbody.isKinematic) {
				int c = (int)(col.collider.gameObject.rigidbody.velocity.magnitude * col.collider.gameObject.rigidbody.mass / density / 10);
				Random rand = new Random ();
				for (int i = 0; i < c; i++){
					WaterDrop wd = (WaterDrop)PoolManager.Insantiate<WaterDrop> ();
					if (wd == null)
						wd = new WaterDrop ();
					wd.Reset (col.point, new Vector2 (rand.NextDouble() - 0.5, rand.NextDouble() * 0.5).normalized * rand.NextDouble() * col.collider.gameObject.rigidbody.velocity.magnitude, (float)rand.NextDouble() * 0.5f);

				}
			}
		}
	}
}

