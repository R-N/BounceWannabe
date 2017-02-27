using System;
using Gdk;
using System.Collections.Generic;

namespace GtkTest
{
	public class Button
	{
		bool pressed = false;
		ulong downframe = 0;
		ulong upframe = 0;
		static Dictionary<Gdk.Key, Button> buttons = new Dictionary<Gdk.Key, Button>();
		static Dictionary<string, Button> buttonsByString = new Dictionary<string, Button>();
		 Button ()
		{
			
		}

		public void Press(){
			if (!pressed) {
				pressed = true;
				if (MainWindow.updating) {
					if (MainWindow.frameCount == ulong.MaxValue)
						downframe = 0;
					else
						downframe = MainWindow.frameCount + 1;

				} else {
					downframe = MainWindow.frameCount;
				}
			}
		}

		public void Release(){
			if (pressed) {
				pressed = false; 
				if (MainWindow.updating) {
					if (MainWindow.frameCount == ulong.MaxValue)
						upframe = 0;
					else
						upframe = MainWindow.frameCount + 1;

				} else {
					upframe = MainWindow.frameCount;
				}
			}
		}

		public bool ButtonDown{
			get{
				return pressed && (MainWindow.frameCount - downframe == 0);
			}
		}
		public bool Pressed{
			get{
				return pressed;
			}
		}
		public bool ButtonUp{
			get{
				return !pressed && (MainWindow.frameCount - upframe == 0);
			}
		}
		public bool ButtonClick{
			get{
				return !pressed && (MainWindow.frameCount - upframe == 0 && upframe - downframe < 5);
			}
		}

		public static Button RegisterButton(Gdk.Key key){
			if (buttons.ContainsKey (key))
				return null;
			Button bt = new Button ();
			buttons.Add (key, bt);
			buttonsByString.Add (key.ToString (), bt);
			return bt;
		}

		public static void SetButtonDown(Gdk.Key key){
			if (buttons.ContainsKey(key))
			buttons [key].Press ();
		}
		public static void SetButtonDown(string keyString){
			if (buttonsByString.ContainsKey(keyString))
			buttonsByString [keyString].Press ();
		}
		public static void SetButtonUp(Gdk.Key key){
			if (buttons.ContainsKey(key))
			buttons [key].Release ();
		}
		public static void SetButtonUp(string keyString){
			if (buttonsByString.ContainsKey(keyString))
			buttonsByString [keyString].Release ();
		}

		public static bool GetButtonDown(Gdk.Key key){
			if (buttons.ContainsKey(key))
				return buttons [key].ButtonDown;
			return false;
		}
		public static bool GetButtonDown(string keyString){
			if (buttonsByString.ContainsKey(keyString))
				return buttonsByString [keyString].ButtonDown;
			return false;
		}
		public static bool GetButton(Gdk.Key key){
			if (buttons.ContainsKey(key))
				return buttons [key].Pressed;
			return false;
		}
		public static bool GetButton(string keyString){
			if (buttonsByString.ContainsKey(keyString))
				return buttonsByString [keyString].Pressed;
			return false;
		}
		public static bool GetButtonUp(Gdk.Key key){
			if (buttons.ContainsKey(key))
				return buttons [key].ButtonUp;
			return false;
		}
		public static bool GetButtonUp(string keyString){
			if (buttonsByString.ContainsKey(keyString))
				return buttonsByString [keyString].ButtonUp;
			return false;
		}
	}
}

