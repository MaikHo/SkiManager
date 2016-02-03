using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SkiManager.App.Interfaces;
using SkiManager.Engine;

namespace SkiManager.App.Behaviors
{
    public sealed class CashierBehavior : ReactiveBehavior, ICashier
    {
        [JsonProperty]
        public float TicketPrice { get; set; }

        [JsonProperty]
        public int MinimumProcessingSeconds { get; set; } = 0;

        [JsonProperty]
        public int MaximumProcessingSeconds { get; set; } = 0;

        [JsonProperty]
        public bool UseWaitingQueueOfParent { get; set; } = false;

        [JsonProperty]
        public bool IsProcessing { get; private set; }

        [JsonProperty]
        public IGraphNode NextNode { get; set; }

        [JsonProperty]
        public IReadOnlyList<Item> SoldItems { get; private set; } = new List<Item> { Items.SkiTicket }.AsReadOnly();

        public Cost GetCostForItem(Item item)
        {
            if (item != Items.SkiTicket)
            {
                throw new InvalidOperationException($"This item seller does not sell item {item.Name}.");
            }
            return new Cost(Items.Money, TicketPrice);
        }

        protected override void Loaded(BehaviorLoadedEventArgs args)
        {
            if (!UseWaitingQueueOfParent)
            {
                args.TrackSubscription(ChildEnter.Subscribe(ProcessEnteringChild));
            }
            else
            {
                args.TrackSubscription(Update.Where(_ => !IsProcessing).Subscribe(_ => TakeCustomerFromQueueAndProcess()));
            }
        }

        private async void ProcessEnteringChild(ChildEnterEngineEventArgs args)
        {
            await SellTicketAndSetEntityToNextNode(args.EnteringChild);
        }

        private async void TakeCustomerFromQueueAndProcess()
        {
            if (IsProcessing)
            {
                return;
            }

            var customer = Entity.Parent.GetImplementationInChildren<IWaitingQueue>().GetDisabledEntitiesFromQueue().FirstOrDefault();
            if (customer == null)
            {
                return;
            }

            IsProcessing = true;
            customer.SetParent(Entity, Reasons.Processing.Started);
            await SellTicketAndSetEntityToNextNode(customer);
        }

        private async Task SellTicketAndSetEntityToNextNode(Entity customerEntity)
        {
            IsProcessing = true; // TODO properly synchronize this to prevent threading issues
            var customer = customerEntity.GetBehavior<CustomerBehavior>();
            if (customer == null)
            {
                // not a customer -> ignore
                customerEntity.SetParent(NextNode.Entity, Reasons.NotAllowed);
                ResetIsProcessingAndCheckForNextCustomer();
                return;
            }

            if (!customer.Inventory.TryTakeItem(Items.Money, TicketPrice))
            {
                // not enough money -> set forward
                customer.Entity.SetParent(NextNode.Entity, Reasons.DoesNotHaveRequiredItem);
                ResetIsProcessingAndCheckForNextCustomer();
                return;
            }

            // enough money taken -> wait processing time, give ticket and set forward
            if (MaximumProcessingSeconds > 0)
            {
                var waitTime = TimeSpan.FromSeconds(new Random().Next(MinimumProcessingSeconds, MaximumProcessingSeconds));
                await Timing.Delay(waitTime);
            }
            customer.Inventory.AddItem(Items.SkiTicket, 1);
            customer.Entity.IsEnabled = true;
            customer.Entity.SetParent(NextNode.Entity, Reasons.Processing.Finished);
            ResetIsProcessingAndCheckForNextCustomer();
        }

        private void ResetIsProcessingAndCheckForNextCustomer()
        {
            IsProcessing = false;
            Task.Factory.StartNew(TakeCustomerFromQueueAndProcess);
        }
    }
}
;