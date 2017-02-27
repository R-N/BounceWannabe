using System;

namespace GtkTest
{
	public class WorldText : Behaviour
	{
		public  TextRenderer rend;
		public WorldText (Vector2 pos, string s, float f)
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			rend = new TextRenderer (gameObject, s, f);
			gameObject.renderer = rend;
		}
	}
}

