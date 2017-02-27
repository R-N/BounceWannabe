using System;

namespace GtkTest
{
	public class Steam : Behaviour
	{
		Vector2 startPoint;
		Vector2 endPoint;
		float length;
		float timer = 0;
		public Steam (Vector2 pos, Vector2 sp, Vector2 ep)
		{

			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			startPoint = sp;
			endPoint = ep;
			length = Vector2.Distance (sp, ep);
		}

		public override void FixedUpdate ()
		{
			timer += MainWindow.fixedDeltaTime;
			if (timer > 0.1f) {
				Random rand = new Random ();
				Vector2 randomPoint = gameObject.transform.LocalToWorldPoint (Vector2.Lerp (startPoint, endPoint, (float)rand.NextDouble ()));
				SteamDrop sd = (SteamDrop)PoolManager.Insantiate<SteamDrop> ();
				if (sd == null)
					sd = new SteamDrop ();
				sd.gameObject.rigidbody.dragMul = 0.25f;
				float sizeMul = (float)rand.NextDouble ();
				sd.Reset (randomPoint, -0.25f * Physics.gravity, 0.25f + sizeMul * 0.25f, 6);
				timer -= 0.1f;
			}
		}
	}
}

