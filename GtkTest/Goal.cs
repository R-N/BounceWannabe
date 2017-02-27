using System;

namespace GtkTest
{
	public class Goal : Behaviour
	{
		public Goal (Vector2 pos, float s) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			gameObject.collider = new CircleCollider (gameObject, s);
			gameObject.renderer = new CircleRenderer (gameObject, s);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
			gameObject.collider.isTrigger = true;
			gameObject.renderer.SetColor (1, 1, 1, 1);
		}

		public override void OnTriggerEnter(Collision col){
			if (col.collider.gameObject.name == "Player")
				MainWindow.instance.GameOverCairo ();
		}
	}
}

