

using LeaderBoard.Attributes;
using LeaderBoard.Models;

namespace LeaderBoard;

public class ScoreService(IConnectionMultiplexer connectionMultiplexer)
{
	private readonly IDatabase _database = connectionMultiplexer.GetDatabase();


	public async Task<bool> AddAsync(string topic, string memberId, int score)
	{
		return await _database.SortedSetAddAsync(topic, memberId, score);
	}

	public async Task<double> IncreamentAsync(string topic, string memberId)
	{
		return await _database.SortedSetIncrementAsync(topic, memberId, 1);
	}

	public async Task<IEnumerable<T>> GetTopAsync<T>(string topic, int k) where T : BaseScoreType
	{
		var items = await _database.SortedSetRangeByRankWithScoresAsync(topic, 0, k - 1, Order.Descending);

		var result = new List<T>();

		foreach (var item in result)
		{
			result.Add(item);
		}
		return result;
	}
}

public static class SrotedSetEntryExtension
{
	public static T ToModel<T>(this SortedSetEntry entry) where T : BaseScoreType
	{
		var model = Activator.CreateInstance<T>();
		model.Score = Convert.ToInt32(entry.Score);

		var properties = typeof(T).GetProperties();

		foreach (var property in properties.Where(x => x.CanWrite))
		{
			var attribute = Attribute.GetCustomAttribute(property, typeof(ElementAttribute));
			if (attribute is not null)
			{
				property.SetValue(model, entry.Element.ToString());
			}
		}

		//var projections = items.Select(x => new MostSoldProduct
		//{
		//	CatalogId = x.Element.ToString(),
		//	Score = Convert.ToInt32(x.Score)
		//}).ToList();

		//return (IEnumerable<T>)projections; 
		return entry.ToModel<T>();
	}
}
