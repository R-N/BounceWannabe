using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GtkTest
{
	public class GameObject 
	{
		public bool enabled = true;
		public string name = "Empty";
		public Transform transform = null;
		public Renderer renderer = null;
		public Rigidbody rigidbody = null;
		public Collider collider = null;
		public List<Behaviour> behaviours = new List<Behaviour>();

		public static List<GameObject> allGameObjects = new List<GameObject>();

		public List<Collision> pendingCollisions = new List<Collision>();
		public HashSet<Collider> pendingCollisionColliders = new HashSet<Collider>();
		public Dictionary<Collider, Collision> lastCollisions = new Dictionary<Collider, Collision>();
		public Dictionary<Collider, int> collidingObjects = new Dictionary<Collider, int>();


		public List<Collision> pendingTriggers = new List<Collision>();
		public HashSet<Collider> pendingTriggerColliders = new HashSet<Collider>();
		public Dictionary<Collider, Collision> lastTriggers = new Dictionary<Collider, Collision>();
		public Dictionary<Collider, int> triggeringObjects = new Dictionary<Collider, int>();

		public Behaviour GetBehaviour<T>(){
			foreach (Behaviour b in behaviours) {
				if (b.GetType () == typeof(T))
					return b;
			}
			return null;
		}

		public GameObject(){
			transform = new Transform (this);
			allGameObjects.Add (this);
		}

		public void Destroy(){
			if (rigidbody != null) {
				Rigidbody.allRigidbodies.Remove (rigidbody);
				rigidbody = null;
			}
			if (collider != null) {
				Collider.allCollider.Remove (collider);
				collider = null;
			}
			if (renderer != null) {
				Renderer.allRenderers.Remove (renderer);
				renderer = null;
			}
			behaviours.Clear ();
			allGameObjects.Remove (this);
				
		}

		public void Update(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				behaviours[i].Update ();
			}
		}

		public void Update2(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				if (behaviours[i].enabled)
				behaviours[i].Update2 ();
			}
		}
		public void Update3(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				if (behaviours[i].enabled)
				behaviours[i].Update3 ();
			}
		}
		public void Update4(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				if (behaviours[i].enabled)
				behaviours[i].Update3 ();
			}
		}
		public void FixedUpdate(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				if (behaviours[i].enabled)
				behaviours[i].FixedUpdate ();
			}
		}

		public void FixedUpdate2(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				if (behaviours[i].enabled)
				behaviours[i].FixedUpdate2 ();
			}
		}
		public void FixedUpdate3(){
			int c = behaviours.Count;
			for (int i = 0; i < c; i++) {
				if (behaviours[i].enabled)
				behaviours[i].FixedUpdate3 ();
			}
		}

		public void GetCollision(List<Collision> c0){
			if (!enabled)
				return;
			foreach (Collision c in c0){
				//Console.WriteLine ("get collision");
				if (pendingCollisionColliders.Add(c.collider))
					pendingCollisions.Add (c);
			}
		}
		public void GetTrigger (List<Collision> c0){
			if (!enabled)
				return;
			foreach (Collision c in c0) {
				if (pendingTriggerColliders.Add(c.collider))
					pendingTriggers.Add (c);
			}
		}
		public void GetCollision(Collision c){
			if (!enabled)
				return;
				if (pendingCollisionColliders.Add(c.collider))
					pendingCollisions.Add (c);
			
		}
		public void GetTrigger (Collision c){
			if (!enabled)
				return;
				if (pendingTriggerColliders.Add(c.collider))
					pendingTriggers.Add (c);
			
		}

		public void SetActive(bool isOn){
			enabled = isOn;
		}

		public void UpdateCollisions(){
			if (!enabled)
				return;
			int c = behaviours.Count;
			int d = pendingCollisions.Count;
			for (int j = d-1; j >= 0; j--){
				Collision a = pendingCollisions [j];
				if (collidingObjects.ContainsKey (a.collider)) {
					collidingObjects [a.collider] = collidingObjects[a.collider] + 1;
					for (int i = 0; i < c; i++) {
						if (behaviours[i].enabled)
						behaviours [i].OnCollisionStay (a);
					}
				} else {
					collidingObjects.Add (a.collider, 2);
					for (int i = 0; i < c; i++) {
						if (behaviours[i].enabled)
						behaviours [i].OnCollisionEnter (a);
					}
				}
				if (lastCollisions.ContainsKey (a.collider)) {
					lastCollisions [a.collider] = a;
				} else {
					lastCollisions.Add (a.collider, a);
				}
			}
			d = collidingObjects.Count;
			List<Collider> cols = collidingObjects.Keys.ToList();
			for (int j = d-1; j >= 0; j--){
				Collider a = cols [j];
				if (collidingObjects [a] > 0)
					collidingObjects [a] = collidingObjects [a] - 1;
				else {
					for (int i = 0; i < c; i++) {
						if (behaviours[i].enabled)
						behaviours [i].OnCollisionExit (lastCollisions[a]);
					}
					lastCollisions.Remove (a);
					collidingObjects.Remove (a);
				}
			}

			pendingCollisions.Clear ();
			pendingCollisionColliders.Clear ();



			d = pendingTriggers.Count;
			for (int j = d-1; j >= 0; j--){
				Collision a = pendingTriggers [j];
				if (triggeringObjects.ContainsKey (a.collider)) {
					triggeringObjects [a.collider] = triggeringObjects[a.collider] + 1;
					for (int i = 0; i < c; i++) {
						if (behaviours[i].enabled)
						behaviours [i].OnTriggerStay (a);
					}
				} else {
					triggeringObjects.Add (a.collider, 2);
					for (int i = 0; i < c; i++) {
						if (behaviours[i].enabled)
						behaviours [i].OnTriggerEnter (a);
					}
				}
				//bef
				if (lastTriggers.ContainsKey (a.collider)) {
					lastTriggers [a.collider] = a;
				} else {
					lastTriggers.Add (a.collider, a);
				}
			}

			d = triggeringObjects.Count;
			cols = triggeringObjects.Keys.ToList();
			for (int j = d-1; j >= 0; j--){
				Collider a = cols [j];
				if (triggeringObjects [a] > 0)
					triggeringObjects [a] = triggeringObjects [a] - 1;
				else {
					for (int i = 0; i < c; i++) {
						if (behaviours[i].enabled)
						behaviours [i].OnTriggerExit (lastTriggers[a]);
					}
					lastTriggers.Remove (a);
					triggeringObjects.Remove (a);
				}
			}

			pendingTriggers.Clear ();
			pendingTriggerColliders.Clear ();
		}

		public void AddBehaviour(Behaviour b){
			behaviours.Add (b);
		}

		public static GameObject Instantiate(Vector2 position){
			GameObject go = new GameObject ();
			go.transform.position = position;
			return go;
		}

		public void Throw (string ex){
			string s = "[" + name + ".Throw] " + ex;
			Console.WriteLine (s);
		throw (new Exception(s));
		}
	}
}

