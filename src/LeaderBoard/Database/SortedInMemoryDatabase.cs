using LeaderBoard.Models;

namespace LeaderBoard.Database;

public class SortedInMemoryDatabase
{
	private Dictionary<string, SortedSet<Type>> _sets;

	private SortedSet<PlayerScore> playerScores = new SortedSet<PlayerScore>();
	private SortedSet<MostSoldProduct> mostSoldProducts = new SortedSet<MostSoldProduct>();

	public SortedSet<PlayerScore> PlayerScores => playerScores;
	public SortedSet<MostSoldProduct> MostSoldProducts => mostSoldProducts;



	public void AddItem<TModel>(TModel model) where TModel : BaseScoreType
	{
		if (model is PlayerScore player)
		{
			playerScores.Add(player);
		}
		else if (model is MostSoldProduct catalog)
		{
			mostSoldProducts.Add(catalog);
		}
	}

}
