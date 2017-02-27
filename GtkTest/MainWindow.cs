using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Cairo;
using System.Threading.Tasks;
using System.Threading;
using Gtk;
using Gdk;

namespace GtkTest
{
	public partial class MainWindow : Gtk.Window
	{
		Gdk.Window dAreaWindow = null;
		Gtk.DrawingArea dArea = null;
		System.Drawing.Graphics g = null;
		Gdk.GC gc = null;
		Cairo.Context context = null;
		Cairo.Context cr = null;
		public static float deltaTime = 0;
		public static ulong frameCount = 0;

		public static Vector2 screenResolution = Vector2.zero;
		public static Vector2 halfScreenResolution = Vector2.zero;

		public static Player player = null;

		bool rendered = false;

		Thread gameThread = null;
		Task gameTask = null;

		Surface surf = null;

		public static float fixedDeltaTime = 0.025f;

		public static bool updating = false;

		bool gameRunning = false;

		public static MainWindow instance = null;

		void CleanUp(){
			/*if (cr != null) {
				Surface s = cr.GetTarget ();
				if (s != null)
					((IDisposable)cr.GetTarget ()).Dispose ();
				cr.Dispose ();
			}
			if (surf != null)
			surf.Dispose ();
			if (context != null)
			context.Dispose ();*/
			Application.Quit ();
		}

		public MainWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			instance = this;
			//KeyPressEvent += (o, args) => KeyPressed(o, args);
			dArea = new DrawingArea();
			screenResolution = new Vector2 (800, 600);
			halfScreenResolution = screenResolution / 2;
			dArea.SetSizeRequest (800, 600);
			dArea.AppPaintable = true;
			dArea.CanFocus = true;
			dArea.GrabFocus ();
			dArea.KeyPressEvent += (o2, args2) => KeyPressed(args2.Event);
			dArea.KeyReleaseEvent += (o2, args2) => KeyReleased(args2.Event);
			KeyPressEvent += (o2, args2) => KeyPressed(args2.Event);
			KeyReleaseEvent += (o2, args2) => KeyReleased(args2.Event);
			dArea.ExposeEvent += (o, args) => OnExpose(o, args);
			DeleteEvent += (o, args) => CleanUp();
			dArea.DeleteEvent += (o, args) => CleanUp();
			this.Add (dArea);

			this.Build ();

			Button.RegisterButton (Gdk.Key.Up);
			Button.RegisterButton (Gdk.Key.Down);
			Button.RegisterButton (Gdk.Key.Left);
			Button.RegisterButton (Gdk.Key.Right);
			Button.RegisterButton (Gdk.Key.space);
			Button.RegisterButton (Gdk.Key.Control_L);
			Button.RegisterButton (Gdk.Key.Alt_L);
			Button.RegisterButton (Gdk.Key.Shift_L);
			Button.RegisterButton (Gdk.Key.Return);
			Button.RegisterButton (Gdk.Key.Escape);
			Button.RegisterButton (Gdk.Key.w);
			Button.RegisterButton (Gdk.Key.a);
			Button.RegisterButton (Gdk.Key.s);
			Button.RegisterButton (Gdk.Key.d);
			Button.RegisterButton (Gdk.Key.period);
			Button.RegisterButton (Gdk.Key.comma);
			Button.RegisterButton (Gdk.Key.semicolon);
			Button.RegisterButton (Gdk.Key.apostrophe);

			//gameThread = new Thread (new ThreadStart (this.GameLoop));
			//gameThread.Start ();
			//gameThread.Priority = ThreadPriority.Highest;
			//System.GC.KeepAlive (gameThread);
		
		

			dArea.DoubleBuffered = false;
			gameTask = new Task (GameLoop);
			gameTask.Start ();
			System.GC.KeepAlive(gameTask);
		}

		public void OnExpose(object o, Gtk.ExposeEventArgs args){
			dAreaWindow = args.Event.Window;
			//RenderCairo ();
			args.RetVal = true;
		}

		public void RenderSG ()
		{
			//Console.WriteLine ("rendering");
			//dArea.DoubleBuffered = false;
			//if (dAreaWindow == null) {
			g = Gtk.DotNet.Graphics.FromDrawable (dAreaWindow);
			gc = new Gdk.GC (dAreaWindow);
			//}
			g.Clear (System.Drawing.Color.Black);
			int c = Renderer.allRenderers.Count;
			for (int i = 0; i < c; i++) {
				Renderer.allRenderers [i].RenderSG (g);
			}
			g.Dispose ();
			gc.Dispose ();
			//dAreaWindow.Dispose ();
			rendered = true;
		}


		public void RenderCairo ()
		{

			//try{
			cr = CairoHelper.Create (dAreaWindow);
			Surface sr = cr.GetTarget ();
			surf = sr.CreateSimilar (Cairo.Content.ColorAlpha, 800, 600);
			context = new Context (surf);


			context.SetSourceRGB (0, 0, 0);
			//context.Rectangle (0, 0, (double)screenResolution.x, (double)screenResolution.y);
			//	context.Fill ();
			context.Paint();
			int c = Renderer.allRenderers.Count;
			for (int i = 0; i < c; i++) {
				Renderer.allRenderers [i].RenderCairo (context);
			}
			cr.SetSource (surf);
			cr.Paint ();

			((IDisposable)cr.GetTarget()).Dispose ();
			cr.Dispose ();
			((IDisposable)sr).Dispose ();
			surf.Dispose ();
			context.Dispose ();
			//dAreaWindow.Dispose ();
			rendered = true;
			/*}catch(Exception ex){
				Console.WriteLine ("[RenderException] " + ex);
			}*/
		}

		public void GameOverCairo(){
			gameRunning = false;


			cr = CairoHelper.Create (dAreaWindow);
			Surface sr = cr.GetTarget ();
			surf = sr.CreateSimilar (Cairo.Content.ColorAlpha, 800, 600);
			context = new Context (surf);


			context.SetSourceRGB (0, 0, 0);
			context.Paint();

			context.MoveTo (new PointD (200, 300));
			context.SetSourceRGB (1,1,1);
			context.SetFontSize (40);
			context.TextPath ("Thank you for playing!");
			context.Fill ();


			cr.SetSource (surf);
			cr.Paint ();
			((IDisposable)cr.GetTarget()).Dispose ();
			cr.Dispose ();
			((IDisposable)sr).Dispose ();
			surf.Dispose ();
			context.Dispose ();
			gameTask.Dispose ();
		}


		void GameLoop(){
			WorldText text1a = new WorldText (new Vector2 (-4, 1.5f), "Hello. Welcome to this bounce wannabe.", 20);
			WorldText text1b = new WorldText (new Vector2 (-4, 1), "This is your starting point.", 20);
			WorldText text1c = new WorldText (new Vector2 (-4,0.5f), "Use A to move left, and D to move right.", 20);
			WorldText text1d = new WorldText (new Vector2 (-4, 0), "Rendering text is heavy so please move to the right.", 20);


			Goal goal = new Goal (new Vector2 (215, 0), 2);

			player = new Player (new Vector2(0, 0));
			player.gameObject.renderer.SetColor (System.Drawing.Color.Blue);
			player.gameObject.name = "Player";
			player.gameObject.rigidbody.mass = 5;
			//player.gameObject.rigidbody.angularSpeed = 2;
			//player.forceVelocity = true;
			Camera cam = new Camera (new Vector2(0,0));
			cam.follow = true;

			Water wat = new Water (new Vector2 (112, -7), 30, 8);
			wat.gameObject.renderer.SetColor (0,1,1,0.2f);
			wat.gameObject.name = "Water";
			wat.gameObject.enabled = false;
			wat.density = 50;


			Wall wall = new Wall (new Vector2(0, -6), new Vector2[]{
				new Vector2 (-25, -10),
				new Vector2(-25, 20),
				new Vector2(-10, 20),
				new Vector2(-10, 15),
				new Vector2(-15, 15),
				new Vector2(-15, 2),
				new Vector2 (20, 2),
				new Vector2 (20, 3),
				new Vector2 (22, 3),
				new Vector2 (22, 4),
				new Vector2(100, 4),
				new Vector2(100, -4),
				new Vector2(120, -4),
				new Vector2(120, -3),
				new Vector2(123, -3),
				new Vector2(123, 0),
				new Vector2(120, 0),
				new Vector2(120, 2),
				new Vector2(122, 4),
				new Vector2 (140, 4),
				new Vector2(140, -4),
				new Vector2(160, -4),
				new Vector2(160,4),
				new Vector2 (217, 4),
				new Vector2 (217, 20),
				new Vector2 (240, 20),
				new Vector2(240, -10)});
			wall.gameObject.renderer.SetColor(System.Drawing.Color.Gray);
			wall.gameObject.name = "Wall";
			wall.gameObject.rigidbody.mass = 10;
			wall.gameObject.rigidbody.frictionCoef = 2f;


			WorldText text2 = new WorldText (new Vector2 (16, 0.5f), "Press W when on a ground to jump.", 20);


			Checkpoint cp1 = new Checkpoint (new Vector2 (40, 0));

			WorldText text3a = new WorldText (new Vector2 (34, 4), "This yellow bar is a checkpoint.", 20);
			WorldText text3b = new WorldText (new Vector2 (34, 3.5f), "It will turn green if you activate it by touching it.", 20);
			WorldText text3c = new WorldText (new Vector2 (34, 3), "When you die, you'll be respawned to the last checkpoint.", 20);
			WorldText text3d = new WorldText (new Vector2 (34, 2.5f), "If you have none, then you'll be respawned at the starting point.", 20);


			SpikeSet ss = new SpikeSet (new Vector2 (62, -2), new Vector2 (-1, 0), new Vector2 (1, 0), 2, 0, 0.5f, 4);
			ss.SummonSpikes ();
			WorldText text4a = new WorldText (new Vector2 (60, 3), "These are spikes.", 20);
			WorldText text4b = new WorldText (new Vector2 (60, 2.5f), "Careful not to touch em!", 20);


			SpikeSet ss2 = new SpikeSet (new Vector2 (80, -2), new Vector2 (-5, 0), new Vector2 (5, 0), 2, 1.5f, 0.5f, 20);

			ss2.SummonSpikes ();
			WorldText text5a = new WorldText (new Vector2 (75, 2.5f), "Some spike set are long,", 20);
			WorldText text5b = new WorldText (new Vector2 (75, 2), "but may be gone after a while.", 20);
			WorldText text5c = new WorldText (new Vector2 (75, 1.5f), "Though they'll show up again, so be quick!", 20);

			Checkpoint cp2 = new Checkpoint (new Vector2 (90, 0));

			WorldText text6a = new WorldText (new Vector2 (101, -7), "Aw, man. What a fall.", 20);

			WorldText text6b = new WorldText (new Vector2 (101, -7.5f), "Wonder if we can go up again.", 20);


			InGameButton bt = new InGameButton (new Vector2 (123d, -8));
			bt.gameObject.transform.rotation = MyMath.FloatPIPer2;
			bt.AddBinding (wat.gameObject.SetActive);

			WorldText text6c = new WorldText (new Vector2 (119, -6.5f), "OwO What's this?", 20);


			WorldText text6d = new WorldText (new Vector2 (120, 3), "Liquid brings you up if your density is smaller than the liquid.", 20);


			WorldText text7a = new WorldText (new Vector2 (141, -7), "Not again.", 20);

			WorldText text7b = new WorldText (new Vector2 (150, -7), "These are steam", 20);
			WorldText text7c = new WorldText (new Vector2 (150, -7.5f), "They may bring you up, like liquid.", 20);
			WorldText text7d = new WorldText (new Vector2 (150, -8f), "However, they don't apply force uniformly.", 20);

			Steam st = new Steam (new Vector2 (158.5f, -10), new Vector2 (-1.5f, 0), new Vector2 (1.5f, 0));

			Checkpoint cp3 = new Checkpoint (new Vector2 (165, 0));

			Fire fir = new Fire(new Vector2 (180, -2), new Vector2 (-1, 0), new Vector2 (1, 0), 4, 2);
			WorldText text8a = new WorldText (new Vector2 (175, 2), "These red bubbles are supposed to be fire.", 20);
			WorldText text8b = new WorldText (new Vector2 (175, 1.5f), "Just like spikes, they may stop emitting after a while.", 20);

			WorldText text9a = new WorldText (new Vector2 (210, 3), "Well done. You've finished the tutorial.", 20);
			WorldText text9b = new WorldText (new Vector2 (210, 2.5f), "Here is the goal, get in.", 20);

			gameRunning = true;

			Stopwatch sw = new Stopwatch ();
			float onePer60 = 1.0f / 60;
			float time = 0;
			float Vsync = onePer60;
			float fixedTime = fixedDeltaTime;
			while (gameRunning) {
				//try {
				deltaTime = sw.Elapsed.Ticks / 10000000.0f;
				sw.Restart ();
				//Console.WriteLine ("FPS : " + 1 / deltaTime);

				time += deltaTime;
				fixedTime += deltaTime;

					updating = true;
				//behaviour updates
				int c = GameObject.allGameObjects.Count;
				for (int i = 0; i < c; i++) {
					GameObject.allGameObjects [i].Update ();
				}


				//behaviour late updates
				for (int i = 0; i < c; i++) {
					GameObject.allGameObjects [i].Update2 ();
				}


				//Stopwatch swp = Stopwatch.StartNew();
				//physics updates
				while (fixedTime >= fixedDeltaTime) {
					Physics.Update ();
					fixedTime -= fixedDeltaTime;
				}
				//swp.Stop ();
				//Console.WriteLine ("Physics time : " + swp.Elapsed.TotalSeconds);

				//behaviour late updates
				for (int i = 0; i < c; i++) {
					GameObject.allGameObjects [i].Update3 ();
				}

					updating = false;



				System.GC.Collect ();

				//if (Vsync >= onePer60) {
				//Stopwatch sw1 = Stopwatch.StartNew();
				RenderCairo();
				//sw1.Stop ();
				//Console.WriteLine ("Rendertime : " + sw1.Elapsed.TotalSeconds);
				//dArea.QueueDraw ();
				//}
				if (frameCount == ulong.MaxValue)
					frameCount = 0;
				frameCount++;
				//System.GC.WaitForPendingFinalizers ();
				//while (!rendered);
				/*}
				catch(Exception ex){
					Console.WriteLine ("[Exception] " + ex.ToString ());
				}*/
			}
		}


		public void KeyPressed(Gdk.EventKey e){
			Button.SetButtonDown (e.Key);
		}

		public void KeyReleased(Gdk.EventKey e){
			Button.SetButtonUp (e.Key);
		}



	}
}

