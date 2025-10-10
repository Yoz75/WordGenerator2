using System;
using System.Collections.Generic;
using System.Linq;

namespace WG2
{

    public class DirectedGraph<TVertexWeight, TEgdeWeight> where TVertexWeight : notnull
    {
        private readonly Dictionary<TVertexWeight, List<(TVertexWeight, TEgdeWeight)>> AdjacencyList;

        private Random Random = new Random();

        public DirectedGraph()
        {
            AdjacencyList = [];
        }

        public DirectedGraph(int capacity)
        {
            AdjacencyList = [];
        }

        public void AddVertex(TVertexWeight vertex)
        {
            if(!AdjacencyList.ContainsKey(vertex))
            {
                AdjacencyList[vertex] = [];
            }
        }

        public void AddEdge(TVertexWeight from, TVertexWeight to, TEgdeWeight egde)
        {
            if(!AdjacencyList.ContainsKey(from))
            {
                AddVertex(from);
            }

            if(!AdjacencyList.ContainsKey(to))
            {
                AddVertex(to);
            }

            AdjacencyList[from].Add((to, egde));
        }

        public IList<(TVertexWeight, TEgdeWeight)> GetAdjacentVertices(TVertexWeight vertex)
        {
            if(AdjacencyList.ContainsKey(vertex))
            {
                return AdjacencyList[vertex];
            }

            throw new KeyNotFoundException($"Вершина '{vertex}' не найдена в графе.");
        }

        public bool ContainsVertex(TVertexWeight vertex)
        {
            return AdjacencyList.ContainsKey(vertex);
        }

        public int Count()
        {
            return AdjacencyList.Count;
        }

        public TVertexWeight GetRandom(int min, int max)
        {
            if(max < min)
            {
                //xor swap
                max ^= min;
                min ^= max;
                max ^= min;
            }

            return AdjacencyList.Keys.ElementAt(Random.Next(min, max));
        }
    }
}
