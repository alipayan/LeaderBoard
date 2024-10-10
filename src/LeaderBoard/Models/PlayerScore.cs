using LeaderBoard.Attributes;

namespace LeaderBoard.Models;

public class PlayerScore : BaseScoreType
{
	//for in memory database
	//public int Id { get; set; }

	public const string RedisKey = "PlayerScore";

	[Element]
	public string Username { get; set; } = null!;

}