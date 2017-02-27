using System;
using System.Collections.Generic;
namespace GtkTest
{
	public class Rigidbody
	{
		public bool enabled = true;
		public GameObject gameObject;
		public Collider collider;
		public Vector2 velocity = Vector2.zero;
		public Vector2 pendingVelocity = Vector2.zero;
		public float mass = 1.0f;
		public float dragAdd = 0f;
		public float dragMul = 0;
		public float pendingDragAdd = 0;
		public float pendingDragMultiply = 0;

		public bool isKinematic = false;

		public bool useGravity = true;

		public bool receiveTorque = true;
		public float angularSpeed = 0;
		public float pendingAngularSpeed = 0;
		public float angularDragAdd = 0;
		public float angularDragMul = 0.2f;
		public float pendingAngularDragAdd = 0;
		public float pendingAngularDragMul = 0;
		public float pendingRotation = 0;

		public float restitutionCoef = 0;
		public bool restitutionMultiply = false;
		public float frictionCoef = 0;
		public bool frictionMultiply = false;

		public static List<Rigidbody> allRigidbodies = new List<Rigidbody>();

		public float inertia = 0;

		public Vector2 pendingMovement = Vector2.zero;

		public Rigidbody (GameObject go)
		{
			gameObject = go;
			collider = go.collider;
			go.rigidbody = this;
			allRigidbodies.Add (this);
			if (collider != null)
			inertia = collider.GetInertia (mass);
		}

		public void Clear(){
			pendingVelocity = Vector2.zero;
			pendingRotation = 0;
			pendingAngularSpeed = 0;
			pendingMovement = Vector2.zero;
			pendingAngularDragMul = 0;
			pendingAngularDragAdd = 0;
			pendingDragAdd = 0;
			pendingDragMultiply = 0;
			velocity = Vector2.zero;
			angularSpeed = 0;
		}


		public void FixedUpdate(){
			if (gameObject.enabled && enabled && !isKinematic){
				Vector2 prevVel = velocity;

				velocity += pendingVelocity;
				pendingVelocity = Vector2.zero;
				if (useGravity)
					velocity += Physics.gravity * MainWindow.fixedDeltaTime;
				float da = dragAdd + pendingDragAdd;
				pendingDragAdd = 0;
				Vector2 dragVel = Vector2.zero;
				if (da > 0){
					dragVel +=  Vector2.ClampMagnitude(velocity, da * MainWindow.fixedDeltaTime);
				}
				float dm = MyMath.Clamp(pendingDragMultiply + dragMul, 0, 1);
				pendingDragMultiply = 0;
				if (dm > 0) {
					dragVel += velocity * dm * MainWindow.fixedDeltaTime;
				}
				dragVel = Vector2.ClampMagnitude (dragVel, velocity.magnitude);
				velocity -= dragVel;
				Vector2 moveDir = 0.5f * (prevVel + velocity) * MainWindow.fixedDeltaTime + pendingMovement;
				pendingMovement = Vector2.zero;
				float moveDirMagnitude = moveDir.magnitude;


				float prevAngularSpeed = angularSpeed;

				angularSpeed += pendingAngularSpeed;

				float normalizedAngularSpeed = MyMath.Normalize (angularSpeed);
				float angularSpeedMag = Math.Abs (angularSpeed);

				pendingAngularSpeed = 0;
				float ada = angularDragAdd + pendingAngularDragAdd;
				pendingAngularDragAdd = 0;
				float angularDragVel = 0;
				if (ada > 0){
					angularDragVel += MyMath.Clamp (angularSpeedMag, 0, ada * MainWindow.fixedDeltaTime);
				}
				float adm = MyMath.Clamp(pendingAngularDragMul + angularDragMul, 0, 1);
				pendingAngularDragMul = 0;
				if (adm > 0) {
					angularDragVel += angularSpeedMag * adm * MainWindow.fixedDeltaTime;
				}
				angularDragVel = MyMath.Clamp (angularDragVel, 0, angularSpeedMag);

				angularSpeed -= normalizedAngularSpeed * angularDragVel;

				angularSpeed = MyMath.Clamp (angularSpeed, -20, 20);

				float rotation = 0.5f * (prevAngularSpeed + angularSpeed) * MainWindow.fixedDeltaTime + pendingRotation;
				pendingRotation = 0;

				float startingMovedirmag = moveDirMagnitude;


				if (moveDirMagnitude > 0 || rotation != 0) {
					Vector2 normalizedMoveDir = moveDir / moveDirMagnitude;
					float rotationMag = Math.Abs (rotation);
					Vector2 moveDone = Vector2.zero;
					float normalizedRotation = MyMath.Normalize (rotation);



					gameObject.transform.position += moveDir;
					if (collider == null) {
						gameObject.transform.rotation += rotation;
						return;
					}
					if (rotation != 0)
					gameObject.transform.RotateAround (collider.pivot, rotation, true);

					if (!collider.isTrigger) {
						List<Collision> cols = collider.GetCollisions ();
						if (cols != null) {
							Vector2 worldPivot = gameObject.transform.LocalToWorldPoint (collider.pivot);
							Vector2 accNormal = Vector2.zero;
							float accRad = 0;
							float sumArea = 0;
							int len = cols.Count;
							float sumLength = 0;
							for (int i = len - 1; i >= 0; i--) {
								Collision n = new Collision ();
								n.normal = -cols [i].normal;
								n.area = cols [i].area;
								n.isCollision = true;
								n.rejection = cols [i].rejection;
								n.additionalFloats = cols [i].additionalFloats;
								n.point = cols [i].point;
								n.collider = collider;
								cols [i].collider.gameObject.GetCollision (n);
								gameObject.GetCollision (cols [i]);
								cols [i].additionalFloats = new float[3];
								float dot = Vector2.Dot (normalizedMoveDir, -cols [i].normal);
								Vector2 r = (cols [i].point - worldPivot).normalized;
								float sin = Vector2.Cross (r, cols [i].normal);

								if (dot <= 0 && sin >= 0) {
									cols.RemoveAt (i);
								} else {
									cols [i].additionalFloats [0] = dot;
									//if (dot > 0) 
									sumArea += cols [i].area;

									cols [i].additionalFloats [1] = Vector2.Dot (Vector2.Rotate (r, MyMath.FloatPIPer2) * normalizedRotation, cols [i].normal);
									if (cols [i].additionalFloats [1] >= 0) {
										cols [i].additionalFloats [2] = Vector2.Distance (worldPivot, cols [i].point);
										sumLength += cols [i].area * cols [i].additionalFloats [2];
									} else {
										cols [i].additionalFloats [2] = 0;
									}
								}
							}
							if (cols.Count > 0) {
								moveDir = normalizedMoveDir * moveDirMagnitude;
								Vector2 rejAng = Vector2.zero;
								foreach (Collision s in cols) {
									Vector2 projVel = Vector2.zero;
									Vector2 projAng = Vector2.zero;
									float ang = 0;
									float retractAng = 0;
									Vector2 r = s.point - worldPivot;
									float rMag = r.magnitude;
									float k = inertia / mass / rMag / rMag;
									if (s.area > 0) {
										if (s.additionalFloats [0] > 0) {
											projVel += Vector2.Project (moveDir, -s.normal) * s.area / sumArea;
										}
										if (rotationMag > 0 && sumLength > 0 && s.additionalFloats [2] > 0) {
											ang = Math.Abs (rotation) * s.additionalFloats [2] * s.area / sumLength;
											Vector2 velAng = Vector2.Rotate (r, MyMath.FloatPIPer2).normalized * normalizedRotation * s.additionalFloats [2] * ang;
											projAng = Vector2.Project (velAng, -s.normal);
											rejAng = Vector2.Reject (velAng, -s.normal);
											retractAng = projAng.magnitude / s.additionalFloats [2];
										}
										if (Vector2.HasNaN (projAng))
											gameObject.Throw ("projAng has nan");
									}

									Vector2 proj = projVel + projAng;

									Vector2 vel = proj / MainWindow.fixedDeltaTime;
									float restCoef = restitutionCoef;
									float fricCoef = frictionCoef;
									Vector2 lostVelocity = Vector2.zero;
									float mass2 = s.collider.gameObject.rigidbody.mass;
									if (s.collider.gameObject.rigidbody != null) {
										if (restitutionMultiply)
											restCoef = restitutionCoef * s.collider.gameObject.rigidbody.restitutionCoef;
										else
											restCoef = (restitutionCoef + s.collider.gameObject.rigidbody.restitutionCoef) / 2;

										if (frictionMultiply)
											fricCoef = frictionCoef * s.collider.gameObject.rigidbody.frictionCoef;
										else
											fricCoef = (frictionCoef + s.collider.gameObject.rigidbody.frictionCoef) / 2;
										//Vector2 givenVelocity = mass * vel / (mass * (1 - restCoef) + mass2);
										//lostVelocity = vel * (mass * restCoef + mass2) / (mass + mass2);
										Vector2 givenVelocity = mass * vel * (1 + restCoef) / (mass + mass2);
										lostVelocity = mass2 * vel * (1 + restCoef) / (mass + mass2);

										if (!s.collider.gameObject.rigidbody.isKinematic) {
											s.collider.gameObject.rigidbody.AddVelocity (givenVelocity, s.point);
										} else {
											lostVelocity += givenVelocity;
										}
										AddVelocity (-lostVelocity, s.point);
										float coef = (lostVelocity.magnitude / vel.magnitude);
									} else {
										lostVelocity = vel * (1 + restCoef);
										AddVelocity (-lostVelocity, s.point);
									}
									float frictionVelocityMag = lostVelocity.magnitude * fricCoef;
									if (frictionVelocityMag > 0) {
										Vector2 frictionVelocity = Vector2.ClampMagnitude (-(velocity + pendingVelocity + rejAng / MainWindow.fixedDeltaTime), lostVelocity.magnitude * fricCoef);



										if (s.collider.gameObject.rigidbody != null && !s.collider.gameObject.rigidbody.isKinematic) {
											s.collider.gameObject.rigidbody.AddImpulse (-frictionVelocity * mass, s.point);
										}
										accRad += Vector2.Project (frictionVelocity, rejAng).magnitude * MainWindow.fixedDeltaTime / rMag;
										AddVelocity (frictionVelocity, s.point);
									}


									float diff = proj.magnitude - s.rejection;
									if (diff > 0) {
										float projangMag = projAng.magnitude;
										if (projangMag > 0) {
											projVel = projVel.normalized * (projVel.magnitude + diff / 2);
											projAng = projAng.normalized * (projangMag + diff / 2);
											ang = (projAng + rejAng).magnitude / s.additionalFloats [2];
											retractAng = projAng.magnitude / s.additionalFloats [2];
										} else {
											projVel = projVel.normalized * (projVel.magnitude + diff);

										}
									}

									accRad += retractAng;

									accNormal += projVel;
								}
							}
							gameObject.transform.position -= accNormal;
							gameObject.transform.RotateAround (collider.pivot, normalizedRotation * -accRad, true);
							//angularSpeed += normalizedRotation * -accRad/MainWindow.fixedDeltaTime;

						}
					}
				} else {

					/*List<Collision> cols = collider.GetCollisions ();
					if (cols != null) {
						foreach (Collision s in cols) {
							Vector2 proj = -s.normal * s.rejection;
							if (s.collider.gameObject.rigidbody != null && !s.collider.gameObject.rigidbody.isKinematic) {
								s.collider.gameObject.transform.position += proj / 2;
								gameObject.transform.position -= proj / 2;
							} else {
								gameObject.transform.position -= proj;
							}

						}
					}*/
				}
			}
		}


		public void DragVelocity(float d, bool multiply = false){
			if (multiply)
				pendingDragMultiply += d;
			else
				pendingDragAdd += d;
		}


		public void AddImpulse(Vector2 momentum, Vector2 position){
			if (!isKinematic) {
				if (Vector2.HasNaN (momentum))
					gameObject.Throw ("got nan momentum");
				if (momentum == Vector2.zero)
					return;
				Vector2 vel = momentum / mass;
				AddVelocity (vel, position);
				//Console.WriteLine (gameObject.name + " got momentum " + momentum + " at " + position
				//	+ "\nreceivedvel " + pendingVelocity + " , receivedAngularSpeed " + receivedAngularSpeed);

			}
		}

		public void AddVelocity(Vector2 vel, Vector2 position){
			if (vel == Vector2.zero)
				return;
			Vector2 m = (gameObject.transform.LocalToWorldPoint (collider.pivot) - position);
			float r = m.magnitude;
			if (r > 0)
			m /= r;
			float k = inertia / mass / r / r;

			Vector2 receivedLinearAngularSpeed = Vector2.Reject(vel, m);
			Vector2 receivedVel = Vector2.Project(vel, m);
			receivedLinearAngularSpeed /= (1 + k);
			receivedVel += receivedLinearAngularSpeed * k;
			//receivedLinearAngularSpeed *= k;
			pendingVelocity += receivedVel;
			if (receiveTorque) {
				
				pendingAngularSpeed += -Vector2.Cross(m, receivedLinearAngularSpeed)  / r;
				//pendingAngularSpeed += cross * newVelMag / r;
			}
		}

	}
}

