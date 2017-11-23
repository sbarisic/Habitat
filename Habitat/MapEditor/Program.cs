using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

namespace HabitatMapEditor
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

            var texture = new Texture("floors.png");

            const int spriteSize = 32;
            var sourceCols = texture.Size.X / spriteSize;

            #region map setup
            var rnd = new Random();
            //var mapSize = new Vector2u((uint)rnd.Next(1, 1024), (uint)rnd.Next(1, 1024));
            var mapSize = new Vector2u(1024, 1024);
            Console.WriteLine("Create map of size {0}x{1} ({2} tiles)", mapSize.X, mapSize.Y, mapSize.X * mapSize.Y);
            var map = new uint[mapSize.X * mapSize.Y];

            for (var i = 0; i < map.Length; ++i)
            {
                map[i] = (uint)(rnd.Next(2, 944));
            }
            #endregion

            #region map population
            var renderStates = RenderStates.Default;
            renderStates.Texture = texture;
            var vertexArray = new VertexArray(PrimitiveType.Quads);

            for (var index = 0; index < map.Length; ++index)
            {
                var textureIndex = map[index];

                // sprite rect in source
                int sourceX = (int)(textureIndex % sourceCols) * spriteSize;
                int sourceY = (int)(textureIndex / sourceCols) * spriteSize;

                // position on map
                int mapX = (int)(index % mapSize.X) * spriteSize;
                int mapY = (int)(index / mapSize.X) * spriteSize;

                // Top left
                vertexArray.Append(new Vertex()
                {
                    Position = new Vector2f(mapX, mapY),
                    TexCoords = new Vector2f(sourceX, sourceY),
                    Color = Color.White
                });

                // Top right
                vertexArray.Append(new Vertex()
                {
                    Position = new Vector2f(mapX + spriteSize, mapY),
                    TexCoords = new Vector2f(sourceX + spriteSize, sourceY),
                    Color = Color.White
                });

                // Bottom right
                vertexArray.Append(new Vertex()
                {
                    Position = new Vector2f(mapX + spriteSize, mapY + spriteSize),
                    TexCoords = new Vector2f(sourceX + spriteSize, sourceY + spriteSize),
                    Color = Color.White
                });

                // Bottom left
                vertexArray.Append(new Vertex()
                {
                    Position = new Vector2f(mapX, mapY + spriteSize),
                    TexCoords = new Vector2f(sourceX, sourceY + spriteSize),
                    Color = Color.White
                });
            }
            #endregion

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();

                // draw tilemap
                window.Draw(vertexArray, renderStates);

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
