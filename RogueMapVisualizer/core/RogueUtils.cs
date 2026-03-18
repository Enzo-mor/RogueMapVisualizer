public static class RogueUtils
{
    public static int Manhattan(int x1,int x2,int y1,int y2)
    {
        return System.Math.Abs((x2-x1)) + System.Math.Abs((y2-y1));
    }

    public static Direction InverseDirection(Direction d)
    {
        switch (d) {
            case Direction.Up : return Direction.Down;
            case Direction.Right : return Direction.Left;
            case Direction.Down : return Direction.Up;
            case Direction.Left : return Direction.Right;
            default : return Direction.Down;
        }
    }
}

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public enum Biome
{
    Forest = 0
}
