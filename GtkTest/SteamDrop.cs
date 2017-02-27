using System;

namespace GtkTest
{
	public class SteamDrop : Behaviour
	{
		public float startLifetime = 5;
		float lifeTime = 0;
		CircleRenderer rend;
		CircleCollider col;
		float startRadius = 0;
		float radius = 0;
		public SteamDrop(){
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
			radius = r;
			startLifetime = newSLT;
			lifeTime = startLifetime;
			Random rand = new Random ();
			gameObject.renderer.SetColor (1, 1, 1, 0.5);
		}

		public override void Update(){
			if (lifeTime > 0) {
				//Console.WriteLine ("lifetime " + lifeTime);
				gameObject.renderer.SetColor (gameObject.renderer.ColorR,
					gameObject.renderer.ColorG, gameObject.renderer.ColorB, 
					0.5 * lifeTime / startLifetime);
				radius = startRadius * lifeTime / startLifetime;
				rend.radius = radius;
				col.radius = radius;
				lifeTime -= MainWindow.deltaTime;
			}else
				PoolManager.Destroy (this);
		}

		public override void OnTriggerStay(Collision col){

			if (!col.collider.isTrigger && col.collider.gameObject.rigidbody != null && !col.collider.gameObject.rigidbody.isKinematic) {

				col.collider.gameObject.rigidbody.AddImpulse (radius * -5 * Physics.gravity * MainWindow.fixedDeltaTime, col.point);
			}
		}
	}
}

