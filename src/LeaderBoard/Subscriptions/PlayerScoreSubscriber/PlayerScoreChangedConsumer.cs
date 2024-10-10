using LeaderBoard.Models;

namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public class PlayerScoreChangedConsumer(LeaderBoardDbContext context,
	SortedInMemoryDatabase sortedDatabase,
	ScoreService scoreService) : IConsumer<PlayerScoreChangedEvent>
{
	private readonly LeaderBoardDbContext _context = context;
	private readonly SortedInMemoryDatabase _sortedDatabase = sortedDatabase;
	private readonly ScoreService _scoreService = scoreService;

	public async Task Consume(ConsumeContext<PlayerScoreChangedEvent> context)
	{
		var message = context.Message;

		#region in memory

		//var item = await _context.PlayerScores.FirstOrDefaultAsync(x => x.Username == message.PayerUserName, context.CancellationToken);

		//if (item is not null)
		//	item.Score = message.Score;
		//else
		//{
		//	var newItem = new Models.PlayerScore
		//	{
		//		Score = message.Score,
		//		Username = message.PayerUserName
		//	};

		//	//add player score to in memory database
		//	await _context.PlayerScores.AddAsync(newItem, context.CancellationToken);

		//	//add player score to sortedset data collection
		//	_sortedDatabase.AddItem(newItem);
		//}

		//await _context.SaveChangesAsync(context.CancellationToken);

		#endregion

		await _scoreService.AddAsync(PlayerScore.RedisKey, message.PlayerUserName, message.Score);
	}
}
