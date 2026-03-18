public class MapWall
{
    private DataRoom[,] mapWall;
    private Room[,] roomMap;

    int width;
    int height;

    public MapWall(Room[,] roomMap)
    {
        width = roomMap.GetLength(0);
        height = roomMap.GetLength(1);

        this.roomMap = roomMap;
        mapWall = new DataRoom[width,height];
        initMapWall();
    }

    public DataRoom[,] generateMapWall()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                var dr = mapWall[i,j];
                if(!dr.IsValid()) continue;

                // Up
                dr.GenerateOneWall(IsInside(i, j-1) ? mapWall[i, j-1] : null, Direction.Up);
                // Right
                dr.GenerateOneWall(IsInside(i+1, j) ? mapWall[i+1, j] : null, Direction.Right);
                // Down
                dr.GenerateOneWall(IsInside(i, j+1) ? mapWall[i, j+1] : null, Direction.Down);
                // Left
                dr.GenerateOneWall(IsInside(i-1, j) ? mapWall[i-1, j] : null, Direction.Left);
            }
        }
        return mapWall;
    }

    private void initMapWall()
    {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                mapWall[i,j] = new DataRoom(roomMap[i,j]);
            }
        }
    }

    private bool IsInside(int x, int y)
    {
        return x >= 0 &&
            y >= 0 &&
            x < width &&
            y < height;
    }
}
