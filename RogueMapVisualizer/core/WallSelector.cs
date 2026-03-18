public static class WallSelector
{
    // Probabilités pondérées pour chaque type de mur selon le type de room et la direction
    private static Dictionary<(RoomType, Direction), List<(WallType, float)>> weights = new()
    {
        // Basic rooms
        { (RoomType.Basic, Direction.Up), new List<(WallType,float)> { (WallType.empty,1f) } },
        { (RoomType.Basic, Direction.Down), new List<(WallType,float)> { (WallType.empty,1f) } },
        { (RoomType.Basic, Direction.Left), new List<(WallType,float)> { (WallType.empty,1f) } },
        { (RoomType.Basic, Direction.Right), new List<(WallType,float)> { (WallType.empty,1f) } },

        // Start rooms - toujours empty
        { (RoomType.Start, Direction.Up), new List<(WallType,float)>{ (WallType.empty,1f) } },
        { (RoomType.Start, Direction.Down), new List<(WallType,float)>{ (WallType.empty,1f) } },
        { (RoomType.Start, Direction.Left), new List<(WallType,float)>{ (WallType.empty,1f) } },
        { (RoomType.Start, Direction.Right), new List<(WallType,float)>{ (WallType.empty,1f) } },

        // Boss rooms - toujours one passage
        { (RoomType.Boss, Direction.Up), new List<(WallType,float)>{ (WallType.empty,1f) } },
        { (RoomType.Boss, Direction.Down), new List<(WallType,float)>{ (WallType.empty,1f) } },
        { (RoomType.Boss, Direction.Left), new List<(WallType,float)>{ (WallType.empty,1f) } },
        { (RoomType.Boss, Direction.Right), new List<(WallType,float)>{ (WallType.empty,1f) } },
    };

    public static WallType Select(RoomType rt, Direction dir)
    {
        var list = weights[(rt, dir)];
        float r = RandomProvider.Value();
        float cumulative = 0f;
        foreach(var (wt, w) in list)
        {
            cumulative += w;
            if(r <= cumulative) return wt;
        }
        return list[list.Count-1].Item1; // fallback
    }
}
