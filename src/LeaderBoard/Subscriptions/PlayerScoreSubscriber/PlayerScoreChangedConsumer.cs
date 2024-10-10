using LeaderBoard.Database;
using Microsoft.EntityFrameworkCore;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public class PlayerScoreChangedConsumer(LeaderBoardDbContext context,
	SortedInMemoryDatabase sortedDatabase) : IConsumer<PlayerScoreChangedEvent>
{
	private readonly LeaderBoardDbContext _context = context;
	private readonly SortedInMemoryDatabase _sortedDatabase = sortedDatabase;

	public async Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
	{
		var message = context.Message;
		var item = await _context.PlayerScores.FirstOrDefaultAsync(x => x.Username == message.PayerUserName, context.CancellationToken);

		if (item is not null)
			item.Score = message.Score;
		else
		{
			var newItem = new Models.PlayerScore
			{
				Score = message.Score,
				Username = message.PayerUserName
			};
			await _context.PlayerScores.AddAsync(newItem, context.CancellationToken);
			_sortedDatabase.AddItem(newItem);
		}

		await _context.SaveChangesAsync(context.CancellationToken);
	}
}
