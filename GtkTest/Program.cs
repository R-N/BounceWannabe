using System;
using Gtk;
using System.Drawing;

namespace GtkTest
{
	class MainClass
	{
		public static MainWindow screen = null;

		public static void Main (string[] args)
		{
			
			Application.Init ();
			screen = new MainWindow ();


			screen.Show ();
			Application.Run ();
		}
	}
}
