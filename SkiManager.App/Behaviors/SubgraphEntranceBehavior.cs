using System;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(IGraphNode))]
    public sealed class SubgraphEntranceBehavior : ReactiveBehavior, ISubgraphEntrance
    {
        private IDisposable _toSubgraphSubscription;

        public IGraphNode SubgraphNode { get; set; }

        protected override void Loaded()
        {
            _toSubgraphSubscription = ChildEnter.Where(_ => _.OldParent != SubgraphNode.Entity).Subscribe(TeleportEntityToSubgraph);
        }

        protected override void Unloading()
        {
            _toSubgraphSubscription.Dispose();
        }

        private void TeleportEntityToSubgraph(ChildEnterEngineEventArgs args)
        {
            args.EnteringChild.SetParent(SubgraphNode.Entity);
        }
    }
}
