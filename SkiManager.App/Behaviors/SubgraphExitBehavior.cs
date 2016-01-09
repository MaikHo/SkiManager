using System;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(IGraphNode))]
    public sealed class SubgraphExitBehavior : ReactiveBehavior, ISubgraphExit
    {
        public IGraphNode UpperGraphNode { get; set; }

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(ChildEnter.Where(_ => _.OldParent != UpperGraphNode.Entity).Subscribe(TeleportEntityToUpperGraph));
        }

        private void TeleportEntityToUpperGraph(ChildEnterEngineEventArgs args)
        {
            args.EnteringChild.SetParent(UpperGraphNode.Entity, Reasons.Subgraph.Leaving);
        }
    }
}
