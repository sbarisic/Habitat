using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otter;
using System.IO;
using TiledSharp;
using Habitat.Ents;

namespace Habitat {
	public enum MapLayers : int {
		Walls = 1,
		Floors = 2,
		Zones = 3,
		Entities = 4,
	}

	class MapChunk : Entity {
		public Tilemap Tilemap;
		public int TileSize = 32;

		public List<WorldEntity> WorldEntities;

		static MapLayers ParseLayer(string Name) {
			return (MapLayers)Enum.Parse(typeof(MapLayers), Name, true);
		}

		WorldEntity CreateWorldEntity(TmxMap Map, TmxObject Obj) {
			if (Obj.ObjectType == TmxObjectType.Tile) {
				foreach (var Tileset in Map.Tilesets) {
					if (Obj.Tile.Gid >= Tileset.FirstGid && Obj.Tile.Gid < (Tileset.FirstGid + Tileset.TileCount)) {
						TmxTilesetTile T = Tileset.Tiles.Where((Tl) => Tl.Id == Obj.Tile.Gid - Tileset.FirstGid).First();

						WorldEntity WEnt = new WorldEntity();
						WEnt.SetPosition((float)Obj.X, (float)Obj.Y);
						WEnt.Width = (float)Obj.Width;
						WEnt.Height = (float)Obj.Height;

						foreach (var P in T.Properties)
							if (P.Key == "EntityName")
								WEnt.EntityName = P.Value;
							else if (P.Key == "EntityID") {
							} else
								WEnt.SetProperty(P.Key, P.Value);


						foreach (var P in Obj.Properties) {
							if (P.Key == "EntityID")
								WEnt.EntityID = P.Value;
							else
								WEnt.SetProperty(P.Key, P.Value);
						}

						if (WEnt.EntityName == null)
							throw new Exception(string.Format("Invalid ent name for tile ID {0} in tileset {1}", Obj.Tile.Gid, Tileset.Name));
						return WEnt;
					}
				}
			}

			throw new Exception("Invalid world entity");
		}

		public MapChunk() {
			//string WorldJson = File.ReadAllText("habitat\\map_segments\\test.json");
			//MapSeg = JsonLoader.Deserialize<MapSegment>(WorldJson);


			TmxMap Map = new TmxMap("habitat\\map_segments\\test.tmx");
			int FirstID = Map.Tilesets[0].FirstGid;

			if (Map.TileWidth != Map.TileHeight)
				throw new Exception("Unsupported non-rectangular tiles");
			TileSize = Map.TileWidth;

			Tilemap = new Tilemap(Map.Tilesets[0].Image.Source, Map.Width * TileSize, Map.Height * TileSize, TileSize, TileSize);
			Tilemap.Smooth = true;
			foreach (var L in Enum.GetValues(typeof(MapLayers)))
				Tilemap.AddLayer((Enum)L, (int)L);
			AddGraphic(Tilemap);

			foreach (var L in Map.Layers) {
				foreach (var T in L.Tiles) {
					int ID = T.Gid - FirstID;
					if (ID < 0)
						continue;

					TileInfo TInf = Tilemap.SetTile(T.X, T.Y, ID, ParseLayer(L.Name));
					TInf.FlipX = T.HorizontalFlip;
					TInf.FlipY = T.VerticalFlip;
					TInf.FlipD = T.DiagonalFlip;
				}
			}

			WorldEntities = new List<WorldEntity>();
			foreach (var OG in Map.ObjectGroups)
				if (ParseLayer(OG.Name) == MapLayers.Entities) {
					foreach (var O in OG.Objects)
						WorldEntities.Add(CreateWorldEntity(Map, O));
				}

			/*foreach (var O in Map.ObjectGroups) {
				MapLayers L = ParseLayer(O.Name);

				if (L == MapLayers.Zones) {
					foreach (var Obj in O.Objects) {
						if (Obj.ObjectType == TmxObjectType.Polygon) {
							Vert[] Verts = Obj.Points.Select((P) => new Vert((float)Obj.X + (float)P.X, (float)Obj.Y + (float)P.Y)).ToArray();

							Vertices V = new Vertices(Verts);
							V.PrimitiveType = VertexPrimitiveType.LinesStrip;
							V.Color = Color.White;
							AddGraphic(V);
						}
					}
				}
			}*/
		}
	}
}