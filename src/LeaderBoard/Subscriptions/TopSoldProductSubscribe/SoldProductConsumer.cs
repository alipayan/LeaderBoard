using LeaderBoard.Models;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscribe;

public class SoldProductConsumer(LeaderBoardDbContext context,
	SortedInMemoryDatabase sortedDatabase,
	ScoreService scoreService) : IConsumer<SoldProductEvent>
{
	private readonly LeaderBoardDbContext _context = context;
	private readonly SortedInMemoryDatabase _sortedDatabase = sortedDatabase;
	private readonly ScoreService _scoreService = scoreService;

	public async Task Consume(ConsumeContext<SoldProductEvent> context)
	{
		var message = context.Message;

		#region in memory

		var item = await _context.MostSoldProducts.FirstOrDefaultAsync(x => x.CatalogId == message.Slug, context.CancellationToken);

		if (item is not null)
			item.Score++;
		else
		{
			var newItem = new Models.MostSoldProduct
			{
				CatalogId = message.Slug,
				Score = 1
			};
			await _context.MostSoldProducts.AddAsync(newItem, context.CancellationToken);
			_sortedDatabase.AddItem(newItem);
		}
		await _context.SaveChangesAsync(context.CancellationToken);

		#endregion

		await _scoreService.IncreamentAsync(MostSoldProduct.RedisKey, message.Slug);
	}
}
