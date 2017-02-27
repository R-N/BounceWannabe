using System;
using Gdk;
namespace GtkTest
{
	public class Camera : Behaviour
	{

		public static Camera instance = null;
		public float zoomSpeed = 1.0f;
		public float angularSPeed = 45 * MyMath.Deg2Rad;

		public bool follow = false;

		public float speed = 200.0f;

		public Camera (Vector2 pos) 
		{
			gameObject = GameObject.Instantiate (pos);
			gameObject.AddBehaviour (this);
			instance = this;
			zoom = 50;
		}

		public override void FixedUpdate3(){
			if (follow) {
				Vector2 playerScreenPos = WorldToScreenPosition(Player.instance.gameObject.transform.position);
				Vector2 delta = Vector2.zero;
				if (playerScreenPos.x > MainWindow.screenResolution.x * 0.75f)
					delta.x += playerScreenPos.x - MainWindow.screenResolution.x * 0.75f;
				else if (playerScreenPos.x < MainWindow.screenResolution.x * 0.25f)
					delta.x += playerScreenPos.x - MainWindow.screenResolution.x * 0.25f;
				if (playerScreenPos.y > MainWindow.screenResolution.y * 0.75f)
					delta.y += playerScreenPos.y - MainWindow.screenResolution.y * 0.75f;
				else if (playerScreenPos.y < MainWindow.screenResolution.y * 0.25f)
					delta.y += playerScreenPos.y - MainWindow.screenResolution.y * 0.25f;
				if (delta != Vector2.zero) {
					delta = ScreenToWorldPosition (WorldToScreenPosition(gameObject.transform.position) + delta);
					//gameObject.transform.position = Vector2.MoveTowards (gameObject.transform.position, delta, speed * MainWindow.fixedDeltaTime / zoom);
					gameObject.transform.position = Vector2.Lerp (gameObject.transform.position, delta, 8 * MainWindow.fixedDeltaTime);
				}
				gameObject.transform.position = Vector2.MoveTowards (gameObject.transform.position, Player.instance.gameObject.transform.position, speed * MainWindow.fixedDeltaTime / zoom);
				/*Vector2 delta = (Player.instance.gameObject.transform.position - gameObject.transform.position);
				float deltaMag = delta.magnitude;
				float deltadeltamag = deltaMag - 250 / zoom;
				if (deltadeltamag > 0) {
					gameObject.transform.position += Vector2.Lerp (Vector2.zero, delta.normalized * deltadeltamag, 8 * MainWindow.fixedDeltaTime);
				}*/
			} else {
				Vector2 axis = new Vector2 (0, 0);
				if (Button.GetButton (Key.Up))
					axis.y += 1;
				if (Button.GetButton (Key.Down))
					axis.y -= 1;
				if (Button.GetButton (Key.Left))
					axis.x -= 1;
				if (Button.GetButton (Key.Right))
					axis.x += 1;
				axis.Normalize ();
				gameObject.transform.position += axis * speed * MainWindow.fixedDeltaTime;
				if (Button.GetButton (Key.period))
					zoom *= (1 + zoomSpeed * MainWindow.fixedDeltaTime);
				if (Button.GetButton (Key.comma))
					zoom /= (1 + zoomSpeed * MainWindow.fixedDeltaTime);
				if (Button.GetButton (Key.semicolon))
					gameObject.transform.rotation += angularSPeed * MainWindow.fixedDeltaTime;
				if (Button.GetButton (Key.apostrophe))
					gameObject.transform.rotation -= angularSPeed * MainWindow.fixedDeltaTime;
			}
		}

		float _zoom = 1;
		public float zoom {
			get {
				return 1/gameObject.transform.localScale.x;
			}
			set {
				_zoom = value;
				gameObject.transform.localScale.x = 1/_zoom;
				gameObject.transform.localScale.y = 1/_zoom;

			}
		}
		public static Vector2 WorldToScreenPosition(Vector2 v){
			Vector2 localPoint = Camera.instance.gameObject.transform.WorldToLocalPoint (v);
			localPoint = new Vector2 (localPoint.x + MainWindow.halfScreenResolution.x, MainWindow.halfScreenResolution.y - localPoint.y);
			return localPoint;
		}

		public static Vector2 ScreenToWorldPosition(Vector2 v){
			v.x = v.x - MainWindow.halfScreenResolution.x;
			v.y = MainWindow.halfScreenResolution.y - v.y;
			return Camera.instance.gameObject.transform.LocalToWorldPoint (v);

		}
			
		public static bool IsPointOutOfScreen(Vector2 screenPoint){

			return (screenPoint.x > MainWindow.screenResolution.x
			|| screenPoint.x < 0
			|| screenPoint.y > MainWindow.screenResolution.y
			|| screenPoint.y < 0);
			
		}
		public static bool IsRendererOutOfScreen(Renderer rend){

			return IsPointOutOfScreen(WorldToScreenPosition(rend.gameObject.transform.LocalToWorldPoint(rend.minBound)))
				&& IsPointOutOfScreen(WorldToScreenPosition(rend.gameObject.transform.LocalToWorldPoint(new Vector2 (rend.minBound.x, rend.maxBound.y))))
					&&IsPointOutOfScreen(WorldToScreenPosition(rend.gameObject.transform.LocalToWorldPoint(new Vector2(rend.maxBound.x, rend.minBound.y))))
					&&IsPointOutOfScreen(WorldToScreenPosition(rend.gameObject.transform.LocalToWorldPoint(rend.maxBound)));
		}
	}
}

