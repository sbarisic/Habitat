using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace Habitat.MapEditor
{
    class Program
    {
        private const float zoomAmount = 1.1f;
        private static bool middleMouseDown;
        private static Vector2f middleMouseClickPosition;

        static void ZoomAtPosition(Vector2i pos, RenderWindow window, float zoom)
        {
            var beforeCoord = window.MapPixelToCoords(pos);

            var view = window.GetView();
            view.Zoom(zoom);
            window.SetView(view);

            var afterCoord = window.MapPixelToCoords(pos);
            var offsetCoords = beforeCoord - afterCoord;
            view.Move(offsetCoords);

            window.SetView(view);
        }

        static void PanView(RenderWindow window, Vector2i currentMousePos)
        {
            var mapped = window.MapPixelToCoords(currentMousePos);

            var view = window.GetView();
            view.Center += middleMouseClickPosition - mapped;
            window.SetView(view);

            middleMouseClickPosition = window.MapPixelToCoords(currentMousePos);
        }

        static void Main(string[] args)
        {
            var window = new RenderWindow(new VideoMode(1024, 768), "Poo", Styles.Default);

            #region event setup
            window.Closed += (s, e) => window.Close();
            window.MouseWheelScrolled += (object s, MouseWheelScrollEventArgs e) =>
            {
                if (e.Delta.Equals(1))
                    ZoomAtPosition(new Vector2i(e.X, e.Y), window, 1 / zoomAmount);
                else if (e.Delta.Equals(-1))
                    ZoomAtPosition(new Vector2i(e.X, e.Y), window, zoomAmount);
            };
            window.MouseButtonPressed += (object s, MouseButtonEventArgs e) =>
            {
                if (e.Button == Mouse.Button.Middle)
                {
                    middleMouseClickPosition = window.MapPixelToCoords(new Vector2i(e.X, e.Y));
                    middleMouseDown = true;
                }
            };
            window.MouseButtonReleased += (object s, MouseButtonEventArgs e) =>
            {
                if (e.Button == Mouse.Button.Middle)
                {
                    middleMouseDown = false;
                }
            };
            window.MouseMoved += (object s, MouseMoveEventArgs e) =>
            {
                if (middleMouseDown)
                    PanView(window, new Vector2i(e.X, e.Y));
            };
            #endregion

            #region map setup
            var rnd = new Random();

            var TileMap = new TileMap();
            TileMap.MapSize = new Vector2u((uint)rnd.Next(1, 1024), (uint)rnd.Next(1, 1024));
            Console.WriteLine("Created map of size: {0}x{1} ({2} tiles)", TileMap.MapSize.X, TileMap.MapSize.Y, TileMap.MapSize.X * TileMap.MapSize.Y);
            TileMap.Textures.Add(new TileMap.TextureMeta("space.png"));
            TileMap.Textures.Add(new TileMap.TextureMeta("floors.png"));

            var firstLevel = new TileMap.Level();
            for (uint i = 0; i < TileMap.MapSize.X * TileMap.MapSize.Y; ++i)
            {
                var spaceLayer = new TileMap.Level.Tile.Layer
                {
                    SpriteSheetIndex = 0,
                    SpriteIndex = (uint)rnd.Next(0, 5)
                };

                var tile = new TileMap.Level.Tile();
                tile.Layers.Add(spaceLayer);

                if (rnd.Next(0, 100) > 80)
                {
                    var newLayer = new TileMap.Level.Tile.Layer
                    {
                        SpriteSheetIndex = 1,
                        SpriteIndex = (uint)rnd.Next(2, 945)
                    };

                    tile.Layers.Add(newLayer);
                }

                firstLevel.Tiles.Add(tile);
            }

            TileMap.Levels.Add(firstLevel);
            TileMap.Rebuild(); // Rebuild after modification
            #endregion

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();

                // draw tilemap
                window.Draw(TileMap);

                /*
                // draw grid
                for (var row = 0; row < mapSize.Y; ++row)
                {
                    var rowLine = new RectangleShape(new Vector2f(mapSize.X * spriteSize, 1))
                    {
                        Position = new Vector2f(0, row * spriteSize - 1),
                        FillColor = Color.Green
                    };

                    window.Draw(rowLine);
                }
                
                for (var col = 0; col < mapSize.X; ++col)
                {
                    var colLine = new RectangleShape(new Vector2f(1, mapSize.Y * spriteSize))
                    {
                        Position = new Vector2f(col * spriteSize - 1, 0),
                        FillColor = Color.Green
                    };

                    window.Draw(colLine);
                }
                */

                window.Display();
            }
        }
    }
}
