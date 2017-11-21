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
using Habitat.Ents;

namespace Habitat {
	static class GCon {
		internal static Debugger Dbg;

		public static void WriteLine(string Fmt, params object[] Text) {
			WriteLine(string.Format(Fmt, Text));
		}

		public static void WriteLine(string Text) {
			Console.WriteLine(Text);
			Dbg?.Log(Text, false);
		}
	}

	class Program {
		static Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();

		static Assembly OnAssemblyResolve(object S, ResolveEventArgs Args) {
			AssemblyName AName = new AssemblyName(Args.Name);
			string StartupDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			string ExpectedPath = Path.Combine(StartupDir, "libs", AName.Name + ".dll");

			if (LoadedAssemblies.ContainsKey(AName.ToString())) {
				//GCon.WriteLine("Using {0}.dll", AName.Name);
				return LoadedAssemblies[AName.ToString()];
			}

			if (File.Exists(ExpectedPath)) {
				//GCon.WriteLine("Loading {0}.dll", AName.Name);
				Assembly Asm = Assembly.LoadFile(ExpectedPath);
				LoadedAssemblies.Add(AName.ToString(), Asm);
				return Asm;
			}

			//GCon.WriteLine("Not found {0}.dll", AName.Name);
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
			Game = new HabitatGame((int)(Desktop.Width * 0.8), (int)(Desktop.Height * 0.8));
			Game.Run();
		}
	}

	class HabitatGame {
		Game Game;
		Scene GameWorld;

		public HabitatGame(int W = 800, int H = 600) {
			Game = new Game(nameof(Habitat), W, H, 60, false);
			Game.Color = Color.Black;
			Game.WindowResize = false;
			Game.MouseVisible = true;

			GCon.Dbg = Game.Debugger;

			GameWorld = new Scene();
			MapChunk MapChunk = new MapChunk();
			GameWorld.Add(MapChunk);

			foreach (var WorldEnt in MapChunk.WorldEntities) {
				Image Rect = Image.CreateRectangle((int)WorldEnt.Width, (int)WorldEnt.Height, WorldEnt.GetPropertyOrDefault("Color", Color.White));
				Rect.SetOrigin(0, WorldEnt.Height);

				WorldEnt.AddGraphic(Rect);
				GameWorld.Add(WorldEnt);
			}

			Game.AddScene(GameWorld);
		}

		[OtterCommand(HelpText = "Find entities by EntityName")]
		static void GetEntities(string EntityName) {
			WorldEntity[] WorldEnts = Program.Game.GameWorld.GetEntities<WorldEntity>().ToArray();

			foreach (var WorldEnt in WorldEnts)
				if (EntityName == "*" || WorldEnt.EntityName == EntityName)
					GCon.WriteLine(WorldEnt.ToString());
		}

		[OtterCommand(HelpText = "Use entity by EntityID")]
		static void Use(string EntityID) {
			WorldEntity[] WEnts = Program.Game.GameWorld.GetEntities<WorldEntity>().Where((WE) => WE.EntityID == EntityID).ToArray();

			if (WEnts.Length != 1)
				GCon.WriteLine("Did not find exactly one entity, aborting");
			else
				WEnts[0].Use();
		}

		public void Run() {
			Game.Start();
		}
	}
}
