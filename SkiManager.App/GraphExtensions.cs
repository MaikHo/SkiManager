using System;
using System.Collections.Generic;
using System.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App
{
    public static class GraphExtensions
    {
        public static DijkstraValues GetDijkstraValues(this IGraphNode node, bool ignoreDirection = false, GraphSearchMode searchMode = GraphSearchMode.ShortestPath)
        {
            if (searchMode != GraphSearchMode.ShortestPath)
            {
                throw new NotImplementedException();
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
                    var alternativeDistance = distances[currentNode] + neighborTuple.Item2;
                    if (alternativeDistance < neighborDistance)
                    {
                        distances[neighborTuple.Item1] = alternativeDistance;
                        predecessors[neighborTuple.Item1] = currentNode;
                    }
                }
            }

            return new DijkstraValues(distances, predecessors);
        }

        private static IEnumerable<Tuple<IGraphNode, float>> GetNeighbors(IGraphNode node, bool ignoreDirection)
        {
            foreach (var edge in node.AdjacentEdges.Select(_ => _.GetImplementation<IGraphEdge>()))
            {
                if (Equals(edge.Start.Entity, node.Entity))
                {
                    yield return Tuple.Create(edge.End, edge.Length);
                }
                else if (ignoreDirection || edge.IsBidirectional)
                {
                    yield return Tuple.Create(edge.Start, edge.Length);
                }
            }
        }
    }

    public class DijkstraValues
    {
        public Dictionary<IGraphNode, float> Distances { get; }
        public Dictionary<IGraphNode, IGraphNode> Predecessors { get; }

        public DijkstraValues(Dictionary<IGraphNode, float> distances, Dictionary<IGraphNode, IGraphNode> predecessors)
        {
            Distances = distances;
            Predecessors = predecessors;
        }

        public IReadOnlyList<IGraphNode> GetPathTowardsTarget(IGraphNode target)
        {
            var temp = target;
            var path = new List<IGraphNode> { target };
            while (Predecessors[temp] != null)
            {
                temp = Predecessors[temp];
                path.Insert(0, temp);
            }
            return path.AsReadOnly();
        }
    }

    public enum GraphSearchMode
    {
        ShortestPath = 0
    }
}
