using System;

namespace GtkTest
{
	public class Circle : Behaviour
	{
		public Circle (Vector2 pos, float s) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			gameObject.collider = new CircleCollider (gameObject, s);
			gameObject.renderer = new CircleRenderer (gameObject, s);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;
		}

	}
}

