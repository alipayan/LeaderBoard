namespace LeaderBoard.Models;

public class MostSoldProduct : BaseScoreType
{
	//for in memory database
	//public int Id { get; set; }

	public const string RedisKey = "MostSold";

	public string CatalogId { get; set; } = null!;

}