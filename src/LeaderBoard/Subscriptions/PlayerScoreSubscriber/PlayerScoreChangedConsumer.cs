using LeaderBoard.Database;
using Microsoft.EntityFrameworkCore;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public class PlayerScoreChangedConsumer(LeaderBoardDbContext context) : IConsumer<PlayerScoreChangedEvent>
{
	private readonly LeaderBoardDbContext _context = context;

	public async Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
	{
		//save on set
		var message = context.Message;
		var item = await _context.PlayerScores.FirstOrDefaultAsync(x => x.Username == message.PayerUserName, context.CancellationToken);

		if (item is not null)
			item.Score = message.Score;
		else
			await _context.PlayerScores.AddAsync(new Models.PlayerScore
			{
				Score = message.Score,
				Username = message.PayerUserName
			}, context.CancellationToken);

		await _context.SaveChangesAsync(context.CancellationToken);
	}
}
