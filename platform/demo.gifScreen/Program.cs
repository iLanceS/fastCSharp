using System;
using fastCSharp;
using fastCSharp.drawing.gif;

namespace demo.gifScreen
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (copyScreen copyScreen = new copyScreen("%" + date.Now.Ticks.toString() + ".gif", 1000 / 24)) 
			{
				Console.WriteLine("press quit to exit.");
				while(Console.ReadLine() != "quit");
			}
		}
	}
}
