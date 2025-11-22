using A9.Core;

public class GameState // Singleton
{
  private static GameState? instance = null;
  public int score { get; private set; }
  public int level { get; private set; }
  public bool win
  {
    get => level >= 5;
  }

  private GameState()
  {
    score = 0;
    level = 0;
  }
  public static GameState getInstance()
  {
    if (instance == null) instance = new GameState();
    return instance;
  }

  public void incrementScore() => score++;
  public void incrementLevel() => level++;
  public void reset()
  {
    score = 0;
    level = 0;
  }
}