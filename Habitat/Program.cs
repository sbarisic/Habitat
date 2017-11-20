using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using SFML.Window;
using Otter;

namespace Habitat {
	static class GCon {
		public static void WriteLine(string Fmt, params object[] Text) {
			WriteLine(string.Format(Fmt, Text));
		}

		public static void WriteLine(string Text) {
			Console.WriteLine(Text);
		}
	}

	class Program {
		static Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();

		static Assembly OnAssemblyResolve(object S, ResolveEventArgs Args) {
			AssemblyName AName = new AssemblyName(Args.Name);
			string StartupDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string ExpectedPath = Path.Combine(StartupDir, "libs", AName.Name + ".dll");

			if (LoadedAssemblies.ContainsKey(AName.ToString())) {
				GCon.WriteLine("Using {0}.dll", AName.Name);
				return LoadedAssemblies[AName.ToString()];
			}

			if (File.Exists(ExpectedPath)) {
				GCon.WriteLine("Loading {0}.dll", AName.Name);
				Assembly Asm = Assembly.LoadFile(ExpectedPath);
				LoadedAssemblies.Add(AName.ToString(), Asm);
				return Asm;
			}

			GCon.WriteLine("Not found {0}.dll", AName.Name);
			return null;
		}

		public static HabitatGame Game;

		static void Main(string[] Args) {
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
			Run();
		}

		static void Run() {
			//MapSegment M = JsonLoader.Deserialize<MapSegment>(File.ReadAllText("habitat\\map_segments\\test.json"));
			
			VideoMode Desktop = VideoMode.DesktopMode;
			Game = new HabitatGame((int)(Desktop.Width * 0.9), (int)(Desktop.Height * 0.9));
			Game.Run();
		}
	}

	class HabitatGame {
		Game Game;

		public HabitatGame(int W = 800, int H = 600) {
			Game = new Game(nameof(Habitat), W, H, 60, false);
			Game.Color = Color.Black;
			Game.WindowResize = false;
			Game.MouseVisible = true;

			Scene MainScene = new Scene();
			MainScene.Add(new World());
			Game.AddScene(MainScene);
		}

		public void Run() {
			Game.Start();
		}
	}
}
