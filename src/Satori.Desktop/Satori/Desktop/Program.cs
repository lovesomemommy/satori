using System;
using Satori.Client;

namespace Satori.Desktop;

public static class Program
{
	[STAThread]
	public static void Main(string[] args)
	{
		using SatoriGame satoriGame = new SatoriGame();
		satoriGame.Run();
	}
}
