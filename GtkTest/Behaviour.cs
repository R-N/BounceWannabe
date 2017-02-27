using System;
using System.Collections;

namespace GtkTest
{
	public class Behaviour
	{
		public bool enabled = true;
		public GameObject gameObject = null;
		public Behaviour ()
		{
			
		}

		public virtual void Update(){
			
		}

		public virtual void Update2(){

		}
		public virtual void Update3(){

		}
		public virtual void Update4(){

		}
		public virtual void FixedUpdate(){

		}
		public virtual void FixedUpdate2(){

		}
		public virtual void FixedUpdate3(){

		}
		public virtual void FixedUpdate4(){

		}

		public virtual void OnCollisionEnter(Collision col){
		}

		public virtual void OnCollisionStay(Collision col){
		}

		public virtual void OnCollisionExit(Collision col){
		}

		public virtual void OnTriggerEnter(Collision c){
		}

		public virtual void OnTriggerStay(Collision c){
		}

		public virtual void OnTriggerExit(Collision c){
		}
	}
}

