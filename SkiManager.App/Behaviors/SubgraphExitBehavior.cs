using System;
using System.Reactive.Linq;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    [RequiresImplementation(typeof(IGraphNode))]
    public sealed class SubgraphExitBehavior : ReactiveBehavior, ISubgraphExit
    {
        private IDisposable _toUpperGraphSubscription;

        public IGraphNode UpperGraphNode { get; set; }

        protected override void Loaded()
        {
            _toUpperGraphSubscription = ChildEnter.Where(_ => _.OldParent != UpperGraphNode.Entity).Subscribe(TeleportEntityToUpperGraph);
        }

        protected override void Unloading()
        {
            _toUpperGraphSubscription.Dispose();
        }

        private void TeleportEntityToUpperGraph(ChildEnterEngineEventArgs args)
        {
            args.EnteringChild.SetParent(UpperGraphNode.Entity);
        }
    }
}
