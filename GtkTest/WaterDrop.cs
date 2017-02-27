using System;

namespace GtkTest
{
	public class WaterDrop : Behaviour
	{
		float lifeTime = 5;

		public WaterDrop(){
			gameObject = new GameObject();;
			gameObject.AddBehaviour (this);
			gameObject.renderer = new CircleRenderer (gameObject, MyMath.dFloat);
			gameObject.renderer.SetColor (0, 1, 1, 0.2f);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = false;
			gameObject.rigidbody.useGravity = true;

		}

		public WaterDrop (Vector2 pos, Vector2 vel, float r)
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			gameObject.renderer = new CircleRenderer (gameObject, r);
			gameObject.renderer.SetColor (0, 1, 1, 0.2f);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = false;
			gameObject.rigidbody.useGravity = true;
			gameObject.rigidbody.velocity = vel;

		}

		public void Reset(Vector2 pos, Vector2 vel, float r){
			gameObject.transform.position = pos;
			gameObject.rigidbody.Clear ();
			gameObject.rigidbody.velocity = vel;
			((CircleRenderer)gameObject.renderer).radius = r;
			lifeTime = 5;
		}

		public override void Update(){
			if (lifeTime > 0)
				lifeTime -= MainWindow.deltaTime;
			else
				PoolManager.Destroy (this);
		}
	}
}

