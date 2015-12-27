using System;
using System.Collections.Generic;
using System.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App
{
    public static class GraphExtensions
    {
        public static DijkstraValues GetDijkstraValues(this IGraphNode node, IGraphNode target = null, bool ignoreDirection = false, GraphSearchMode searchMode = GraphSearchMode.ShortestPath)
        {
            if (searchMode != GraphSearchMode.ShortestPath)
            {
                throw new InvalidOperationException();
            }

            var allGraphNodes =
                node.Entity.Level.Entities.Where(entity => entity.Implements<IGraphNode>())
                    .Select(entity => entity.GetImplementation<IGraphNode>()).ToList();
            var distances = new Dictionary<IGraphNode, float>();
            var predecessors = new Dictionary<IGraphNode, IGraphNode>();
            foreach (var graphNode in allGraphNodes)
            {
                distances.Add(graphNode, float.MaxValue);
                predecessors.Add(graphNode, null);
            }
            distances[node] = 0;

            while (allGraphNodes.Count > 0)
            {
                var currentNode = allGraphNodes.OrderBy(_ => distances[_]).First();
                allGraphNodes.Remove(currentNode);
                foreach (var neighborTuple in GetNeighbors(currentNode, ignoreDirection))
                {
                    if (!allGraphNodes.Contains(neighborTuple.Item1))
                    {
                        continue;
                    }

                    var neighborDistance = distances[neighborTuple.Item1];
                    var alternativeDistance = neighborDistance + neighborTuple.Item2;
                    if (alternativeDistance < neighborDistance)
                    {
                        distances[neighborTuple.Item1] = alternativeDistance;
                        predecessors[neighborTuple.Item1] = currentNode;
                    }
                }
            }

            var path = new List<IGraphNode>();
            if (target != null)
            {
                path.Add(target);
                var current = target;
                while (predecessors[current] != null)
                {
                    current = predecessors[current];
                    path.Insert(0, current);
                }
            }

            return new DijkstraValues(distances, predecessors, path);
        }

        private static IEnumerable<Tuple<IGraphNode, float>> GetNeighbors(IGraphNode node, bool ignoreDirection)
        {
            foreach (var edge in node.AdjacentEdges.Select(_ => _.GetImplementation<IGraphEdge>()))
            {
                if (edge.Start == node.Entity)
                {
                    yield return Tuple.Create(edge.End.GetImplementation<IGraphNode>(), edge.Length);
                }
                else if (ignoreDirection)
                {
                    yield return Tuple.Create(edge.Start.GetImplementation<IGraphNode>(), edge.Length);
                }
            }
        }
    }

    public class DijkstraValues
    {
        public Dictionary<IGraphNode, float> Distances { get; }
        public Dictionary<IGraphNode, IGraphNode> Predecessors { get; }
        public IReadOnlyList<IGraphNode> Path { get; }

        public DijkstraValues(Dictionary<IGraphNode, float> distances, Dictionary<IGraphNode, IGraphNode> predecessors, IReadOnlyList<IGraphNode> path)
        {
            Distances = distances;
            Predecessors = predecessors;
            Path = path;
        }
    }

    public enum GraphSearchMode
    {
        ShortestPath = 0,
        LowestCost = 1
    }
}
