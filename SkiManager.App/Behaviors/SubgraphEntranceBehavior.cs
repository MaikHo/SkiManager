using System;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(IGraphNode))]
    public sealed class SubgraphEntranceBehavior : ReactiveBehavior, ISubgraphEntrance
    {
        public IGraphNode SubgraphNode { get; set; }

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            args.TrackSubscription(ChildEnter.Where(_ => SubgraphNode?.Entity != null && _.OldParent != SubgraphNode.Entity).Subscribe(TeleportEntityToSubgraph));
        }

        private void TeleportEntityToSubgraph(ChildEnterEngineEventArgs args)
        {
            args.EnteringChild.SetParent(SubgraphNode.Entity);
        }
    }
}
