using System;
using System.Numerics;
using Newtonsoft.Json;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;

namespace SkiManager.App.Behaviors
{
    public abstract class GraphEdgeBehavior : ReactiveBehavior, IGraphEdge
    {
        private Entity _start;

        [JsonProperty]
        public Entity Start
        {
            get { return _start; }
            set
            {
                if (value == _start)
                {
                    return;
                }
                if (!value.Implements<IGraphNode>())
                {
                    throw new InvalidOperationException("Start has to be an IGraphNode!");
                }
                _start?.GetImplementation<IGraphNode>().AdjacentEdges.Remove(Entity);
                _start = value;
                _start?.GetImplementation<IGraphNode>().AdjacentEdges.Add(Entity);
            }
        }

        private Entity _end;

        [JsonProperty]
        public Entity End
        {
            get { return _end; }
            set
            {
                if (value == _end)
                {
                    return;
                }
                if (!value.Implements<IGraphNode>())
                {
                    throw new InvalidOperationException("End has to be an IGraphNode!");
                }
                _end?.GetImplementation<IGraphNode>().AdjacentEdges.Remove(Entity);
                _end = value;
                _end?.GetImplementation<IGraphNode>().AdjacentEdges.Add(Entity);
            }
        }

        public float Length => Vector3.Distance(Start.GetBehavior<TransformBehavior>().Position, End.GetBehavior<TransformBehavior>().Position);

        [JsonProperty]
        public bool IsBidirectional { get; set; }

        [JsonProperty]
        public float BaseSpeedModifier { get; set; }
    }
}
