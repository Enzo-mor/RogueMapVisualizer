using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RogueMapVisualizer
{
    public partial class MainWindow : Window
    {
        int tileSize = 50;
        MapBiome biome;
        Room[,] rooms;
        int stepInd = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerateMap(object sender, RoutedEventArgs e)
        {
            stepInd = 0;

            if (!int.TryParse(minWidthBox.Text, out int minWidth)) minWidth = 5;
            if (!int.TryParse(minHeightBox.Text, out int minHeight)) minHeight = 5;
            if (!int.TryParse(maxWidthBox.Text, out int maxWidth)) maxWidth = 8;
            if (!int.TryParse(maxHeightBox.Text, out int maxHeight)) maxHeight = 8;
            if (!int.TryParse(minMinusBox.Text, out int minMinus)) minMinus = 15;
            if (!int.TryParse(maxMinusBox.Text, out int maxMinus)) maxMinus = 20;
            if (!int.TryParse(seedBox.Text, out int seed)) seed = 0;

            biome = new MapBiome(maxWidth, minWidth, maxHeight, minHeight, minMinus, maxMinus, seed);
            bool[,] biomeMap = biome.generateMap();

            MapRoom roomGen = new MapRoom(biomeMap);
            rooms = roomGen.generateMapRoom();

            Render(rooms, biome.steps[stepInd]);
            UpdateStepText();
        }

        private void NextStep(object sender, RoutedEventArgs e)
        {
            stepInd = Math.Min(biome.steps.Count-1, stepInd+1);
            Render(rooms, biome.steps[stepInd]);
            UpdateStepText();
        }

        private void PreviousStep(object sender, RoutedEventArgs e)
        {
            stepInd = Math.Max(0, stepInd - 1);
            Render(rooms, biome.steps[stepInd]);
            UpdateStepText();
        }

        private void DrawBiome(bool[,] map)
        {
            int w = map.GetLength(0);
            int h = map.GetLength(1);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Fill = map[x, y] ? Brushes.Black : Brushes.White
                    };

                    Canvas.SetLeft(rect, x * tileSize);
                    Canvas.SetTop(rect, y * tileSize);

                    MapCanvas.Children.Add(rect);
                }
            }
        }

        private void DrawClusters()
        {
            for (int i = 0; i < biome.clusters.Count; i++)
            {
                for (int j = 0; j < biome.clusters[i].Count; j++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = tileSize,
                        Height = tileSize,
                        Fill = Brushes.Aqua
                    };

                    Canvas.SetLeft(rect, biome.clusters[i][j].x * tileSize);
                    Canvas.SetTop(rect, biome.clusters[i][j].y * tileSize);

                    MapCanvas.Children.Add(rect);
                }
            }
        }

        private void DisplayOptionsChanged(object sender, RoutedEventArgs e)
        {
            if (biome == null || rooms == null) return;

            Render(rooms, biome.steps[stepInd]);
        }

        private void Render(Room[,] rooms, bool[,] biome)
        {
            MapCanvas.Children.Clear();

            DrawBiome(biome);

            if (showClustersCheck.IsChecked == true)
                DrawClusters();

            DrawGrid(rooms.GetLength(0), rooms.GetLength(1));

            if (showRoomsCheck.IsChecked == true)
                DrawRooms(rooms);

            /*if (showWallsCheck.IsChecked == true)
                DrawWalls(rooms);*/
        }

        private void DrawGrid(int w, int h)
        {
            for (int x = 0; x <= w; x++)
            {
                DrawLine(x, 0, x, h, Brushes.DarkGray);
            }

            for (int y = 0; y <= h; y++)
            {
                DrawLine(0, y, w, y, Brushes.DarkGray);
            }
        }

        private void DrawRooms(Room[,] rooms)
        {
            int w = rooms.GetLength(0);
            int h = rooms.GetLength(1);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Room r = rooms[x, y];

                    Ellipse circle = new Ellipse
                    {
                        Width = tileSize * 0.3,
                        Height = tileSize * 0.3,
                        Fill = GetRoomColor(r)
                    };

                    Canvas.SetLeft(circle, x * tileSize + tileSize * 0.3);
                    Canvas.SetTop(circle, y * tileSize + tileSize * 0.3);

                    MapCanvas.Children.Add(circle);
                }
            }
        }

        private void UpdateStepText()
        {
            if (biome == null || biome.steps == null || biome.steps.Count == 0)
            {
                stepTextBlock.Text = "Step : 0 / 0";
                return;
            }

            stepTextBlock.Text = $"Step : {stepInd + 1} / {biome.steps.Count}";
        }

        /*private void DrawWalls(Room[,] rooms)
        {
            int w = rooms.GetLength(0);
            int h = rooms.GetLength(1);

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    Room r = rooms[x, y];

                    DrawWallPoint(x, y, 0.5, 0.1, r.up);    // North
                    DrawWallPoint(x, y, 0.9, 0.5, r.right); // East
                    DrawWallPoint(x, y, 0.5, 0.9, r.down);  // South
                    DrawWallPoint(x, y, 0.1, 0.5, r.left);  // West
                }
            }
        }*/

        /*private void DrawWallPoint(int x, int y, double px, double py, bool wall)
        {
            Ellipse dot = new Ellipse
            {
                Width = tileSize * 0.15,
                Height = tileSize * 0.15,
                Fill = wall ? Brushes.Red : Brushes.Green
            };

            Canvas.SetLeft(dot, x * tileSize + tileSize * px - dot.Width / 2);
            Canvas.SetTop(dot, y * tileSize + tileSize * py - dot.Height / 2);

            MapCanvas.Children.Add(dot);
        }*/

        private Brush GetRoomColor(Room r)
        {
            return r.rt switch
            {
                RoomType.Invalid => Brushes.Black,
                RoomType.Basic => Brushes.White,
                RoomType.Start => Brushes.Green,
                RoomType.Boss => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        private void DrawLine(int x1, int y1, int x2, int y2, Brush color)
        {
            Line line = new Line
            {
                X1 = x1 * tileSize,
                Y1 = y1 * tileSize,
                X2 = x2 * tileSize,
                Y2 = y2 * tileSize,
                Stroke = color,
                StrokeThickness = 2
            };
            MapCanvas.Children.Add(line);
        }
    }
}