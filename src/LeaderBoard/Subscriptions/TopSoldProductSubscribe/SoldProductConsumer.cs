using LeaderBoard.Database;
using Microsoft.EntityFrameworkCore;

namespace LeaderBoard.Subscriptions.TopSoldProductSubscribe;

public class SoldProductConsumer(LeaderBoardDbContext context) : IConsumer<SoldProductEvent>
{
	private readonly LeaderBoardDbContext _context = context;

	public async Task Consume(ConsumeContext<SoldProductEvent> context)
	{
		var message = context.Message;
		var item = await _context.MostSoldProducts.FirstOrDefaultAsync(x => x.CatalogId == message.Slug, context.CancellationToken);

		if (item is not null)
			item.Score++;
		else
			await _context.MostSoldProducts.AddAsync(new Models.MostSoldProduct
			{
				CatalogId = message.Slug,
				Score = 1
			}, context.CancellationToken);

		await _context.SaveChangesAsync(context.CancellationToken);
	}
}
