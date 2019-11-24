using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazes
{
    public class Maze : Graph<int, float>
    {
        public int rows;
        public int cols;

        enum Direction
        {
            North, East, West, South
        }

        public Maze(int rows, int cols) : base()
        {
            this.rows = rows;
            this.cols = cols;

            InitializeVertices();
        }

        private void InitializeVertices()
        {
            vertices = new GraphVertex[rows * cols];

            for (int i = 0; i < rows * cols; i++)
            {
                vertices[i] = new Graph<int, float>.GraphVertex(i);
            }
        }

        private GraphVertex GetVertexInDirection(GraphVertex origin, Direction direction)
        {
            int originRow = origin.Value / cols;
            int originCol = origin.Value % cols;
            int newRow = originRow;
            int newCol = originCol;
            switch(direction)
            {
                case Direction.North:
                    newRow -= 1;
                    break;
                case Direction.South:
                    newRow += 1;
                    break;
                case Direction.East:
                    newCol += 1;
                    break;
                case Direction.West:
                    newCol -= 1;
                    break;
            }

            if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
            {
                return null;
            }
            else
            {
                return vertices[newRow * cols + newCol];
            }
        }

        public List<GraphVertex> GetAllUnvisitedAdjacentVerts(GraphVertex vertex)
        {
            List<GraphVertex> ret = new List<GraphVertex>();
            ret.Add(GetVertexInDirection(vertex, Direction.North));
            ret.Add(GetVertexInDirection(vertex, Direction.South));
            ret.Add(GetVertexInDirection(vertex, Direction.East));
            ret.Add(GetVertexInDirection(vertex, Direction.West));
            ret = ret.FindAll(x => x != null && x.Visited == false);

            List<GraphVertex> ret2 = new List<GraphVertex>();
            foreach (GraphVertex v in ret)
            {
                if (v != null && v.Visited == false)
                {
                    ret2.Add(v);
                }
            }

            return ret2;
        }

        public List<GraphVertex> GetAllUnvisitedConnectedVerts(GraphVertex vertex)
        {
            return vertex.Edges.Select(x => x.Destination).ToList().FindAll(x => x.Visited == false);
        }

        public void RecursiveBacktracker()
        {
            InitializeVertices();

            Random random = new Random();

            Stack<GraphVertex> vertStack = new Stack<GraphVertex>();
            vertStack.Push(vertices[0]);
            vertices[0].Visited = true;
            while (vertStack.Count() > 0)
            {
                // Do a "drunken walk"
                bool loopDrunkenWalk = true;
                do
                {
                    GraphVertex vertex = vertStack.Peek();
                    List<GraphVertex> possibleVerts = GetAllUnvisitedAdjacentVerts(vertex);
                    if (possibleVerts.Count > 0)
                    {
                        GraphVertex randomChoice = possibleVerts[random.Next(0, possibleVerts.Count)];
                        randomChoice.Visited = true;
                        AddEdge(vertex, randomChoice, 1);
                        vertStack.Push(randomChoice);
                    }
                    else
                    {
                        loopDrunkenWalk = false;
                    }
                } while (loopDrunkenWalk);

                // Recurse backwards
                bool loopBacktracking = true;
                do
                {
                    GraphVertex vertex = vertStack.Peek();
                    List<GraphVertex> possibleVerts = GetAllUnvisitedAdjacentVerts(vertex);
                    if (possibleVerts.Count > 0)
                    {
                        loopBacktracking = false;
                    }
                    else
                    {
                        vertStack.Pop();
                    }
                } while (loopBacktracking && vertStack.Count > 0);
            }

            foreach(GraphVertex vertex in vertices)
            {
                vertex.Visited = false;
            }
        }
    }
}
