using System;

namespace GtkTest
{
	public class FireDrop : Behaviour
	{
		public float startLifetime = 5;
		float lifeTime = 0;
		CircleRenderer rend;
		CircleCollider col;
		float startRadius = 0;
		public FireDrop(){
			gameObject = new GameObject();;
			gameObject.AddBehaviour (this);
			rend = new CircleRenderer (gameObject, MyMath.dFloat);
			col = new CircleCollider (gameObject, 0);
			col.isTrigger = true;
			gameObject.collider = col;
			gameObject.renderer = rend;
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = false;
			gameObject.rigidbody.useGravity = false;
		}


		public void Reset(Vector2 pos, Vector2 vel, float r, float newSLT){
			gameObject.transform.position = pos;
			gameObject.rigidbody.Clear ();
			gameObject.rigidbody.velocity = vel;
			startRadius = r;
			rend.radius = r;
			col.radius = r;
			startLifetime = newSLT;
			lifeTime = startLifetime;
			Random rand = new Random ();
			gameObject.renderer.SetColor (1, rand.NextDouble() * 0.5, 0, 0.5);
		}

		public override void Update(){
			if (lifeTime > 0) {
				gameObject.renderer.SetColor (gameObject.renderer.ColorR,
					gameObject.renderer.ColorG, gameObject.renderer.ColorB, 
					0.5 * lifeTime / startLifetime);
				rend.radius = startRadius * lifeTime / startLifetime;
				col.radius = rend.radius;
				lifeTime -= MainWindow.deltaTime;
			}else
				PoolManager.Destroy (this);
		}

		public override void OnTriggerEnter(Collision col){
			if (col.collider.gameObject.name == "Player") {
				Player.instance.GoToCheckpoint ();
			}
		}
	}
}

