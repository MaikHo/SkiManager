using System;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class CashierBehavior : ReactiveBehavior, ICashier
    {
        private IDisposable _childEnterSubscription;

        public Func<float> TicketPriceResolver { get; set; }

        public IGraphNode NextNode { get; set; }

        protected override void Loaded()
        {
            _childEnterSubscription = ChildEnter.Subscribe(SellTicketAndSetEntityToNextNode);
        }

        protected override void Unloading()
        {
            _childEnterSubscription.Dispose();
        }

        private void SellTicketAndSetEntityToNextNode(ChildEnterEngineEventArgs args)
        {
            var customer = args.EnteringChild.GetBehavior<CustomerBehavior>();
            if (customer == null)
            {
                // not a customer -> ignore
                return;
            }

            if (!customer.Inventory.TryTakeItem(Items.Money, TicketPriceResolver()))
            {
                // not enough money -> set forward
                customer.Entity.SetParent(NextNode.Entity);
                return;
            }

            // enough money taken -> give ticket and set forward
            customer.Inventory.AddItem(Items.SkiTicket, 1);
            customer.Entity.SetParent(NextNode.Entity);
        }
    }
}
