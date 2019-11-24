using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mazes
{
    /// <summary>
    /// Interaction logic for MazeControl.xaml
    /// </summary>
    public partial class MazeControl : UserControl
    {
        Maze maze;
        MazeCell[] cells;
        int wallThickness = 2;
        public MazeControl()
        {
            InitializeComponent();

            maze = new Maze(50, 50);
            maze.RecursiveBacktracker();
            PlaceControls();
            BreakWalls();
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                ColorFlood();
            }).Start();
        }

        private void ColorFlood()
        {
            List<Maze.GraphVertex> frontiers = new List<Maze.GraphVertex>();
            
            // Add the bottom left vert as a frontier
            frontiers.Add(maze.vertices[(maze.rows - 1) * maze.cols]);

            double hue = 0;
            Brush brush;

            while (frontiers.Count > 0)
            {
                hue += 1;

                List<Maze.GraphVertex> newFrontiers = new List<Maze.GraphVertex>();
                foreach (Maze.GraphVertex frontier in frontiers)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        brush = new SolidColorBrush(HSVColor.GetHSVColor(hue, 1, 1));
                        cells[frontier.Value].Rect.Fill = brush;
                    });
                    
                    frontier.Visited = true;
                    newFrontiers.AddRange(maze.GetAllUnvisitedConnectedVerts(frontier));
                }
                frontiers = newFrontiers;
                Thread.Sleep(3);
            }
        }

        private void PlaceControls()
        {
            MazeGrid.Children.Clear();
            cells = new MazeCell[maze.rows * maze.cols];
            
            for (int i = 0; i < maze.rows; i++)
            {
                MazeGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < maze.cols; i++)
            {
                MazeGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for(int i = 0; i < maze.rows * maze.cols; i++)
            {
                int row = i / maze.cols;
                int col = i % maze.cols;
                Maze.GraphVertex vertex = maze.vertices[i];
                MazeCell newCell = new MazeCell();
                newCell.SetValue(Grid.RowProperty, row);
                newCell.SetValue(Grid.ColumnProperty, col);
                MazeGrid.Children.Add(newCell);
                cells[i] = newCell;
            }
        }

        private void BreakWalls()
        {
            for (int i = 0; i < maze.rows * maze.cols; i++)
            {
                Thickness newThickness = new Thickness(wallThickness / 2);

                // Make walls twice as thick on the edges so that all walls look the same thickness
                if (i % maze.cols == 0)
                {
                    newThickness.Left = wallThickness;
                }
                if (i % maze.cols == maze.cols - 1)
                {
                    newThickness.Right = wallThickness;
                }
                if (i / maze.cols == 0)
                {
                    newThickness.Top = wallThickness;
                }
                if (i / maze.cols == maze.rows - 1)
                {
                    newThickness.Bottom = wallThickness;
                }

                // Bust down a wall for each neihboring edge a vertex has
                foreach (Maze.GraphEdge edge in maze.vertices[i].Edges)
                {
                    if (i + 1 == edge.Destination.Value)
                    {
                        newThickness.Right = 0;
                    }
                    else if (i - 1 == edge.Destination.Value)
                    {
                        newThickness.Left = 0;
                    }
                    else if (i + maze.cols == edge.Destination.Value)
                    {
                        newThickness.Bottom = 0;
                    }
                    else if (i - maze.cols == edge.Destination.Value)
                    {
                        newThickness.Top = 0;
                    }
                }

                // Apply the new thickness
                cells[i].Border.BorderThickness = newThickness;
            }

            // Create start and end point by breaking walls at bottom left and top right corner
            int bottomLeftIndex = (maze.rows - 1) * maze.cols;
            Thickness thickness = cells[bottomLeftIndex].Border.BorderThickness;
            thickness.Bottom = 0;
            cells[bottomLeftIndex].Border.BorderThickness = thickness;

            int topRightIndex = maze.cols - 1;
            thickness = cells[topRightIndex].Border.BorderThickness;
            thickness.Top = 0;
            cells[topRightIndex].Border.BorderThickness = thickness;
        }
    }
}
