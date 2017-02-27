using System;
using Gdk;

namespace GtkTest
{
	public class Player : Behaviour
	{

		public static Vector2 checkPoint;
		public float accel = 80;
		public static Player instance = null;
		public bool forceVelocity = false;
		bool spaceDown = false;

		int groundCounter = 0;
		public Player (Vector2 pos) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			gameObject.collider = new CircleCollider (gameObject, 1);
			gameObject.renderer = new CircleRenderer (gameObject, 1);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.restitutionCoef = 1f;
			instance = this;
			checkPoint = pos;
		}

		public void GoToCheckpoint(){
			gameObject.rigidbody.Clear ();
			gameObject.transform.position = Player.checkPoint;
		}

		public override void FixedUpdate(){
			if (gameObject.transform.position.y < -40) {
				GoToCheckpoint ();
			}

			Vector2 axis = new Vector2 (0, 0);
			if (Button.GetButton(Key.a))
				axis.x -= 1;
			if (Button.GetButton(Key.d))
				axis.x += 1;
			if (Button.GetButton(Key.Return)){
				gameObject.rigidbody.velocity = Vector2.zero;	
			}

			if (groundCounter > 0) {
				if (Button.GetButton (Key.w))
					gameObject.rigidbody.pendingVelocity += Vector2.up * 0.5f * accel / gameObject.rigidbody.mass;
			}
			axis.Normalize ();
			gameObject.rigidbody.pendingVelocity += axis * accel * MainWindow.fixedDeltaTime / gameObject.rigidbody.mass;
			groundCounter--;
		}

		public override void OnCollisionEnter(Collision col){
			if (Vector2.Dot (col.normal, Physics.gravity) < -0.5f && gameObject.rigidbody.velocity.y <= 0) {
				groundCounter = 1;
			}
		}
		public override void OnCollisionStay(Collision col){
			if (Vector2.Dot (col.normal, Physics.gravity) < -0.5f && gameObject.rigidbody.velocity.y <= 0) {
				
					groundCounter = 1;
			}
		}
	}
}

