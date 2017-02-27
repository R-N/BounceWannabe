using System;
using System.Collections.Generic;

namespace GtkTest
{
	public class InGameButton : Behaviour
	{
		public List<Action<bool>> bindings = new List<Action<bool>>();
		bool isOn = false;

		public InGameButton (Vector2 pos) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			Vector2[] points = new Vector2[] {
				new Vector2 (-1, 0),
				new Vector2 (-0.5f, 0.25f),
				new Vector2 (0.5f, 0.25f),
				new Vector2 (1, 0)
			};
			gameObject.collider = new ConvexCollider (gameObject, points);
			gameObject.renderer = new PolygonRenderer (gameObject, points);
			gameObject.rigidbody = new Rigidbody (gameObject);
			gameObject.rigidbody.isKinematic = true;
			gameObject.rigidbody.useGravity = false;

			gameObject.renderer.SetColor (System.Drawing.Color.Gray);

		}

		public void AddBinding(Action<bool> bind){
			bindings.Add (bind);
		}

		public override void OnCollisionEnter(Collision col){
			if (col != null && col.collider != null && col.collider.gameObject != null &&
				col.collider.gameObject.name == "Player") {
				isOn = !isOn;
					foreach (Action<bool> b in bindings) {
						b (isOn);
					}

				gameObject.renderer.SetColor (isOn ? System.Drawing.Color.Green : System.Drawing.Color.Gray);
			}
		}
	}
}

