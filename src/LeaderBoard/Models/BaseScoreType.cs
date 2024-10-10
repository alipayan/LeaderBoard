namespace LeaderBoard.Models;

public class BaseScoreType
{
	public int Score { get; set; }

}

public class BaseScoreTypeComparer : IComparer<BaseScoreType>
{
	public int Compare(BaseScoreType? thisOne, BaseScoreType? thatOne)
	{
		ArgumentNullException.ThrowIfNull(thisOne);
		ArgumentNullException.ThrowIfNull(thatOne);

		return thisOne!.Score.CompareTo(thatOne!.Score);
	}
}
