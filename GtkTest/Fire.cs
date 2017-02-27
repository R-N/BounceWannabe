using System;

namespace GtkTest
{
	public class Fire : Behaviour
	{
		Vector2 startPoint;
		Vector2 endPoint;
		float length;
		float time = 0;
		float duration;
		float delay;
		float timer = 0;
		bool emitting = false;
		public Fire (Vector2 pos, Vector2 sp, Vector2 ep, float dur, float del)
		{

			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			startPoint = sp;
			endPoint = ep;
			length = Vector2.Distance (sp, ep);
			emitting = true;
			duration = dur;
			delay = del;
		}

		public override void FixedUpdate ()
		{
			if (emitting) {
				timer += MainWindow.fixedDeltaTime;
				if (timer > 0.1f) {
					Random rand = new Random ();
					Vector2 randomPoint = gameObject.transform.LocalToWorldPoint (Vector2.Lerp (startPoint, endPoint, (float)rand.NextDouble ()));
					FireDrop fd = (FireDrop)PoolManager.Insantiate<FireDrop> ();
					if (fd == null)
						fd = new FireDrop ();
					fd.gameObject.rigidbody.dragMul = 0.25f;
					float sizeMul = (float)rand.NextDouble ();
					fd.Reset (randomPoint, -0.25f * Physics.gravity, sizeMul * 0.5f, sizeMul * 2);
					timer -= 0.1f;
				}
				if (delay > 0) {
					time += MainWindow.fixedDeltaTime;
					if (time > duration) {
						time = 0;
						emitting = false;
					}
				}
			} else {
				time += MainWindow.fixedDeltaTime;
				if (time > delay) {
					time = 0;
					emitting = true;
				}
			}
		}
	}
}

