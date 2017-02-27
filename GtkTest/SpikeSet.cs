using System;
using System.Collections.Generic;

namespace GtkTest
{
	public class SpikeSet : Behaviour
	{

		Vector2 startPoint;
		Vector2 endPoint;
		float length;
		float time = 0;
		float duration;
		float delay;
		bool emitting = false;
		List<Spike> spikes = new List<Spike>();
		int spikeCount;
		float height;

		public SpikeSet (Vector2 pos, Vector2 sp, Vector2 ep, float dur, float del, float h, int sum)
		{

			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			startPoint = sp;
			endPoint = ep;
			length = Vector2.Distance (sp, ep);
			emitting = true;
			duration = dur;
			delay = del;
			spikeCount = sum;
			height = h;
		}

		public override void Update ()
		{
			if (emitting) {
				if (delay > 0) {
					time += MainWindow.deltaTime;
					if (time > duration) {
						time = 0;
						emitting = false;
						HideSpikes ();
					}
				}
			} else {
				time += MainWindow.deltaTime;
				if (time > delay) {
					time = 0;
					emitting = true;
					ShowSpikes ();
				}
			}
		}
		public void SummonSpikes(){
			float w = length / spikeCount;
			float halfW = w / 2;
			float halfH = height / 2;
			for (int i = 0; i < spikeCount; i++) {
				Vector2 pos = gameObject.transform.LocalToWorldPoint (
					              Vector2.MoveTowards (startPoint, endPoint, halfW + i * w) + Vector2.up * halfH);
				Spike s = new Spike (pos, w, Math.Abs(height));
				if (height < 0)
					s.gameObject.transform.rotation += MyMath.FloatPI;
				spikes.Add (s);
			}
		}

		public void HideSpikes(){
			foreach (Spike s in spikes) {
				s.gameObject.enabled = false;
			}
		}

		public void ShowSpikes(){
			foreach (Spike s in spikes) {
				s.gameObject.enabled = true;
			}
		}
	}
}

