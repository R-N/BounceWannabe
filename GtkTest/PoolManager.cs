using System;
using System.Collections.Generic;

namespace GtkTest
{
	public class PoolManager
	{

		static Dictionary<Type, List<Behaviour>> pool = new Dictionary<Type, List<Behaviour>>();
		public PoolManager ()
		{
		}

		public static void Destroy(Behaviour b){
			Type t = b.GetType ();
			if (!pool.ContainsKey (t))
				pool.Add (t, new List<Behaviour> ());
			pool[t].Add (b);
			b.gameObject.enabled = false;
		}

		public static Behaviour Insantiate<T>(){
			Type t = typeof(T);
			if (!pool.ContainsKey(t))
				pool.Add (t, new List<Behaviour> ());
			if (pool [t].Count > 0) {
				Behaviour t1 = pool[t] [0];
				t1.gameObject.enabled = true;
				pool [t].Remove(t1);
				return t1;
			}
			return null;
		}

	}
}

