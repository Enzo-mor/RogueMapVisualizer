using System;
public static class RandomProvider
{
    private static Random rand = new Random();

    public static void Init(int seed)
    {
        if(seed != 0)
            rand = new Random(seed);
        else
            rand = new Random();
    }

    public static int Range(int min, int max)
    {
        return rand.Next(min, max);
    }

    public static float Value()
    {
        return (float)rand.NextDouble();
    }
}