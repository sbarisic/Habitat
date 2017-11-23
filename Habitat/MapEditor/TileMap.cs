using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Habitat.MapEditor
{
    public class TileMap : Drawable
    {
        public class Level
        {
            public class Tile
            {
                public class Layer
                {
                    public uint SpriteSheetIndex
                    {
                        get;
                        internal set;
                    }

                    public uint SpriteIndex
                    {
                        get;
                        internal set;
                    }
                }

                public List<Layer> Layers
                {
                    get;
                    internal set;
                }

                public Tile()
                {
                    Layers = new List<Layer>();
                }
            }

            public List<Tile> Tiles
            {
                get;
                internal set;
            }

            public Level()
            {
                Tiles = new List<Tile>();
            }
        }

        public List<Level> Levels
        {
            get;
            internal set;
        }

        public const uint SpriteSize = 32;
        public static Vector2u MapSize
        {
            get;
            internal set;
        }

        public class TextureMeta
        {
            public uint Columns
            {
                get;
                internal set;
            }

            public uint Rows
            {
                get;
                internal set;
            }

            public Texture Texture
            {
                get;
                internal set;
            }

            public RenderStates RenderStates
            {
                get;
                internal set;
            }

            public TextureMeta(string filename)
            {
                Texture = new Texture(filename);
                Columns = Texture.Size.X / SpriteSize;
                Rows    = Texture.Size.Y / SpriteSize;

                RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, Texture, null);
            }

            public VertexArray GetSprite(int tileIndex, uint spriteIndex)
            {
                var sourcePos = new Vector2u(
                    (spriteIndex % Columns) * SpriteSize,
                    (spriteIndex / Columns) * SpriteSize
                );

                var mapPos = new Vector2i(
                    (int)((tileIndex % MapSize.X) * SpriteSize),
                    (int)((tileIndex / MapSize.X) * SpriteSize)
                );

                var vertexArray = new VertexArray(PrimitiveType.Points, 4);

                // Top Left
                vertexArray[0] = new Vertex()
                {
                    Position  = (Vector2f)mapPos,
                    Color     = Color.White,
                    TexCoords = (Vector2f)sourcePos
                };

                // Top Right
                vertexArray[1] = new Vertex()
                {
                    Position  = (Vector2f)(mapPos + new Vector2i((int)SpriteSize, 0)),
                    Color     = Color.White,
                    TexCoords = (Vector2f)(sourcePos + new Vector2u(SpriteSize, 0))
                };

                // Bottom Right
                vertexArray[2] = new Vertex()
                {
                    Position = (Vector2f)(mapPos + new Vector2i((int)SpriteSize, (int)SpriteSize)),
                    Color = Color.White,
                    TexCoords = (Vector2f)(sourcePos + new Vector2u(SpriteSize, SpriteSize))
                };
                
                // Bottom Left
                vertexArray[3] = new Vertex()
                {
                    Position = (Vector2f)(mapPos + new Vector2i(0, (int)SpriteSize)),
                    Color = Color.White,
                    TexCoords = (Vector2f)(sourcePos + new Vector2u(0, SpriteSize))
                };

                return vertexArray;
            }
        }

        public List<TextureMeta> Textures
        {
            get;
            internal set;
        }

        private Dictionary<uint, VertexArray> textureDictionary = new Dictionary<uint, VertexArray>();

        public TileMap()
        {
            Levels = new List<Level>();
            Textures = new List<TextureMeta>();
        }

        public void Rebuild()
        {
            textureDictionary.Clear();

            foreach(var level in Levels)
            {
                for (var tileIndex = 0; tileIndex < level.Tiles.Count; ++tileIndex)
                {
                    var tile = level.Tiles[tileIndex];
                    var topLayer = tile.Layers[tile.Layers.Count - 1];

                    var spriteVertexArray = Textures[(int)topLayer.SpriteSheetIndex].GetSprite(tileIndex, topLayer.SpriteIndex);

                    if (textureDictionary.ContainsKey(topLayer.SpriteSheetIndex) == false)
                        textureDictionary[topLayer.SpriteSheetIndex] = new VertexArray(PrimitiveType.Quads);

                    textureDictionary[topLayer.SpriteSheetIndex].Append(spriteVertexArray[0]);
                    textureDictionary[topLayer.SpriteSheetIndex].Append(spriteVertexArray[1]);
                    textureDictionary[topLayer.SpriteSheetIndex].Append(spriteVertexArray[2]);
                    textureDictionary[topLayer.SpriteSheetIndex].Append(spriteVertexArray[3]);
                }
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach(var entry in textureDictionary)
            {
                target.Draw(entry.Value, Textures[(int)entry.Key].RenderStates);
            }
        }
    }
}
