namespace LeaderBoard.Subscriptions.PlayerScoreSubscriber;

public record PlayerScoreChangedEvent(string PlayerUserName, int Score);