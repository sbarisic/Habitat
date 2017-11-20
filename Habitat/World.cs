using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using System.IO;

namespace Habitat {
	public enum MapSegmentType {
		main
	}

	public enum MapLayer {
		Region = -1,
		Floor = 1,
		Wall = 2,
	}

	struct MapSegment {
		public struct tileset_entry {
			public int firstgid;
			public string source;
		}

		public struct layer {
			public struct chunk {
				public int[] data;
				public int width;
				public int height;
				public int x;
				public int y;
			}

			public chunk[] chunks;
			public int startx;
			public int starty;
			public string type;
			public string name;

			public MapLayer LayerName {
				get {
					return (MapLayer)Enum.Parse(typeof(MapLayer), name);
				}
			}
		}

		public Dictionary<string, string> properties;
		public Dictionary<string, string> propertytypes;
		public tileset_entry[] tilesets;
		public layer[] layers;
		public int nextobjectid;

		public object GetProperty(string Name) {
			if (properties.ContainsKey(Name)) {
				if (propertytypes[Name] == "string")
					return properties[Name];

				throw new Exception("Unknown property type " + propertytypes[Name]);
			}

			throw new Exception("Property not found " + Name);
		}

		public MapSegmentType SegmentType {
			get {
				return (MapSegmentType)Enum.Parse(typeof(MapSegmentType), (string)GetProperty("segment_type"));
			}
		}

		public void PopulateTilemap(Tilemap TMap) {
			foreach (var L in layers) {
				if ((int)L.LayerName < 0)
					continue;

				TMap.AddLayer(L.LayerName, (int)L.LayerName);

				foreach (var C in L.chunks) {
					for (int i = 0; i < C.data.Length; i++) {
						int X = (i % C.width) + C.x;
						int Y = (i / C.height) + C.y;

						TMap.SetTile(X + 16, Y + 16, C.data[i], L.LayerName);
					}
				}
			}
		}
	}

	class World : Entity {
		public MapSegment MapSeg;
		public Tilemap Tilemap;
		public static int GridSize = 32;

		public World() {
			//string WorldJson = File.ReadAllText("habitat\\map_segments\\test.json");
			//MapSeg = JsonLoader.Deserialize<MapSegment>(WorldJson);

			Tilemap = new Tilemap("habitat\\textures\\tileset.png", 32 * GridSize, GridSize);
			Tilemap.Smooth = true;

			Tilemap.SetLayer(0);

			//Enum.GetValues(typeof(MapLayer));


			AddGraphic(Tilemap);

			//MapSeg.PopulateTilemap(Tilemap);

			//Dictionary<string, object> Map = (Dictionary<string, object>)JsonLoader.Deserialize(File.ReadAllText("habitat\\map_segments\\test.json"));
			//object[] Tilesets = Map.Value<object[]>("tilesets");

			//Tilemap.SetTile(0, 0, 0);
		}
	}
}
