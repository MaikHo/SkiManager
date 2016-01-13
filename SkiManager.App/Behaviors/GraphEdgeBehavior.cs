using System.Numerics;
using Newtonsoft.Json;
using SkiManager.App.Interfaces;
using SkiManager.Engine;
using SkiManager.Engine.Behaviors;
using SkiManager.Engine.Interfaces;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(ILineTransform))]
    public abstract class GraphEdgeBehavior : ReactiveBehavior, IGraphEdge
    {
        private IGraphNode _start;

        [JsonProperty]
        public IGraphNode Start
        {
            get { return _start; }
            set
            {
                if (value == _start)
                {
                    return;
                }

                _start?.Entity?.GetImplementation<IGraphNode>().AdjacentEdges.Remove(Entity);
                _start = value;
                _start?.Entity?.GetImplementation<IGraphNode>().AdjacentEdges.Add(Entity);

                Entity.GetImplementation<ILineTransform>().Point1 = _start?.Entity?.GetImplementation<ITransform>()?.Position ?? Vector3.Zero;
            }
        }

        private IGraphNode _end;

        [JsonProperty]
        public IGraphNode End
        {
            get { return _end; }
            set
            {
                if (value == _end)
                {
                    return;
                }

                _end?.Entity?.GetImplementation<IGraphNode>().AdjacentEdges.Remove(Entity);
                _end = value;
                _end?.Entity?.GetImplementation<IGraphNode>().AdjacentEdges.Add(Entity);

                Entity.GetImplementation<ILineTransform>().Point2 = _end?.Entity?.GetImplementation<ITransform>()?.Position ?? Vector3.Zero;
            }
        }

        public float Length => Vector3.Distance(Start?.Entity?.GetBehavior<TransformBehavior>().Position ?? Vector3.Zero, End?.Entity?.GetBehavior<TransformBehavior>().Position ?? Vector3.Zero);

        [JsonProperty]
        public bool IsBidirectional { get; set; }

        [JsonProperty]
        public float BaseSpeedModifier { get; set; }
    }
}
