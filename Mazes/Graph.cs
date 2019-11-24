using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mazes
{
    public class Graph<V, E>
    {
        public class GraphVertex
        {
            public V Value { get; set; }
            public List<GraphEdge> Edges { get; set; }
            public bool Visited;
            public GraphVertex(V value)
            {
                this.Value = value;
                this.Visited = false;
                Edges = new List<GraphEdge>();
            }
        }

        public class GraphEdge
        {
            public E Weight { get; set; }
            public GraphVertex Destination { get; set; }
            public GraphEdge(GraphVertex destination, E weight)
            {
                this.Destination = destination;
                this.Weight = weight;
            }
        }

        public IList<GraphVertex> vertices;

        public Graph()
        {
            vertices = new List<GraphVertex>();
        }

        public void AddVertex(V value)
        {
            vertices.Add(new GraphVertex(value));
        }

        public void AddEdge(GraphVertex start, GraphVertex end, E weight)
        {
            start.Edges.Add(new GraphEdge(end, weight));
            end.Edges.Add(new GraphEdge(start, weight));
        }
    }
}
