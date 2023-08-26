using MassTransit;
using Order.Api.Models;
using Shared.StockEvents;
using System.Threading.Tasks;

namespace Order.Api.Consumers
{
    public class StockNotreservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly DataContext _dataContext;

        public Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            throw new System.NotImplementedException();
        }
    }
}
