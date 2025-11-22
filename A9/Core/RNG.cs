using A9.Core;
public static class RNG
{
  private static Random random = new Random();
  public static double getRandomDouble(double min, double max)
  {
    return (max - min) * random.NextDouble() + min;
  }
}