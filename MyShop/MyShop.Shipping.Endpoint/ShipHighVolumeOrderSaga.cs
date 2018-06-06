namespace MyShop.Shipping.Endpoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MyShop.ItOps.Messages;
    using MyShop.Shipping.Messages;

    using NServiceBus;
    using NServiceBus.Logging;

    public class ShipHighVolumeOrderSaga : Saga<ShipHighVolumeOrderSagaData>,
                                           IAmStartedByMessages<ShipHighVolumeOrder>,
                                           IHandleMessages<ShipWithFanCourierResponse>
    {
        static readonly ILog Log = LogManager.GetLogger(typeof(ShipHighVolumeOrderSaga));

        public Task Handle(ShipHighVolumeOrder message, IMessageHandlerContext context)
        {
            Log.Info($"Received ShipHighVolumeOrder command for Order with Id {message.OrderId}.");

            Data.BatchStatuses = new Dictionary<Guid, bool>();
            Data.OrderId = message.OrderId;
            Data.CouldNotShip = false;

            const int NumberOfBatches = 4;
            for (var i = 0; i < NumberOfBatches; i++)
            {
                var correlationId = Guid.NewGuid();
                const bool IsSent = false;
                Data.BatchStatuses.Add(correlationId, IsSent);
                context.Send(new ShipWithFanCourierRequest { CorrelationId = correlationId });
            }

            return Task.CompletedTask;
        }

        public Task Handle(ShipWithFanCourierResponse message, IMessageHandlerContext context)
        {
            if (!message.PackageShipped)
            {
                if (!Data.CouldNotShip)
                {
                    Log.Info($"Couldn't ship Batch {message.CorrelationId} from Order {Data.OrderId}. Compensating...");
                    Compensate(context);

                    Data.CouldNotShip = true;
                }

                return Task.CompletedTask;
            }

            Log.Info($"Done shipping Batch {message.CorrelationId} from Order {Data.OrderId}.");
            Data.BatchStatuses[message.CorrelationId] = true;

            if (Data.BatchStatuses.Values.All(v => v))
            {
                Log.Info($"Done shipping Order with Id {Data.OrderId}.");
                context.Publish<IOrderShipped>(m => { m.OrderId = Data.OrderId; });
                MarkAsComplete();
            }

            return Task.CompletedTask;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipHighVolumeOrderSagaData> mapper)
        {
            mapper.ConfigureMapping<ShipHighVolumeOrder>(message => message.OrderId)
                .ToSaga(sagaData => sagaData.OrderId);
        }

        private void Compensate(IMessageHandlerContext context)
        {
            foreach (var batchId in Data.BatchStatuses.Keys)
            {
                context.Send(new CancelFanCourierShipping { CorrelationId = batchId });
            }
        }
    }

    public class ShipHighVolumeOrderSagaData : ContainSagaData
    {
        public Guid OrderId { get; set; }

        public IDictionary<Guid, bool> BatchStatuses { get; set; }

        public bool CouldNotShip { get; set; }
    }
}