using MassTransit;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscribe;

public class SoldProductConsumer : IConsumer<SoldProductEvent>
{
	public Task Consume(ConsumeContext<SoldProductEvent> context)
	{
		return Task.CompletedTask;
	}
}
