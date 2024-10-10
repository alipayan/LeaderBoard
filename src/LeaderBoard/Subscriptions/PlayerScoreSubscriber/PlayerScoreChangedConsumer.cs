using MassTransit;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public class PlayerScoreChangedConsumer : IConsumer<PlayerScoreChangedEvent>
{
	public Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
	{
		//save on set
		return Task.CompletedTask;
	}
}
