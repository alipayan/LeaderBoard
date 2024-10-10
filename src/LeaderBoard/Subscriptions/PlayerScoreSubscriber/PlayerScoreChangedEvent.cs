namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public record PlayerScoreChangedEvent(string PayerUserName, int Score);