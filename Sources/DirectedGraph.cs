using System;
using System.Collections.Generic;
using System.Linq;

namespace WG2
{

    public class DirectedGraph<T>
    {
        private readonly Dictionary<T, List<T>> AdjacencyList;

        private Random Random = new Random();

        public DirectedGraph()
        {
            AdjacencyList = new Dictionary<T, List<T>>();
        }

        public void AddVertex(T vertex)
        {
            if(!AdjacencyList.ContainsKey(vertex))
            {
                AdjacencyList[vertex] = new List<T>();
            }
        }

        public void AddEdge(T from, T to)
        {
            if(!AdjacencyList.ContainsKey(from))
            {
                AddVertex(from);
            }

            if(!AdjacencyList.ContainsKey(to))
            {
                AddVertex(to);
            }

            AdjacencyList[from].Add(to);
        }

        public IList<T> GetAdjacentVertices(T vertex)
        {
            if(AdjacencyList.ContainsKey(vertex))
            {
                return AdjacencyList[vertex];
            }

            throw new KeyNotFoundException($"Вершина '{vertex}' не найдена в графе.");
        }

        public bool ContainsVertex(T vertex)
        {
            return AdjacencyList.ContainsKey(vertex);
        }

        public int Count()
        {
            return AdjacencyList.Count;
        }

        public T GetRandom(int min, int max)
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
