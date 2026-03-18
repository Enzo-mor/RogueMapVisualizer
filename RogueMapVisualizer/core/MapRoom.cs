public class MapRoom
{
    bool[,] myMap;
    Room[,] roomMap;
    int width;
    int height;

    int xstart,ystart;

    public MapRoom(bool[,] biomeMap)
    {
        myMap = biomeMap;
        width = myMap.GetLength(0);
        height = myMap.GetLength(1);
        roomMap = new Room[width,height];
    }

    public Room[,] generateMapRoom()
    {
        buildRoomMap();

        placeStartRoom();
        placeBossRoom();

        return roomMap;
    }

    void buildRoomMap()
    {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                roomMap[i,j] = new Room();
                if (myMap[i, j])
                {
                    roomMap[i,j].rt = RoomType.Invalid;
                }
                else
                {
                    roomMap[i,j].rt = RoomType.Basic;
                    buildCardRoom(ref roomMap[i,j],i,j);
                }
            }
        }
    }

    void buildCardRoom(ref Room r, int x, int y)
    {
        if(y > 0 && !myMap[x,y-1]) r.up = true;
        if(x < width-1 && !myMap[x+1,y]) r.right = true;
        if(x > 0 && !myMap[x-1,y]) r.left = true;
        if(y < height-1 && !myMap[x,y+1]) r.down = true;
    }

    void placeStartRoom()
    {
        List<(int x, int y)> starts = new List<(int x, int y)>();
        // Colonnes gauche et droite
        for (int y = 0; y < height; y++)
        {
            if (!myMap[0, y]) starts.Add((0,y));
            if (!myMap[width - 1, y]) starts.Add((width - 1,y));
        }

        // Lignes haut et bas
        for (int x = 0; x < width; x++)
        {
            if (!myMap[x, 0]) starts.Add((x,0));
            if (!myMap[x, height - 1]) starts.Add((x,height - 1));
        }

        if (starts.Count > 0)
        {
            var startRoom = starts[RandomProvider.Range(0, starts.Count)];
            xstart = startRoom.x;
            ystart = startRoom.y;
            roomMap[xstart,ystart].rt = RoomType.Start;
        }

    }

    void placeBossRoom()
    {
        int bestx = -1;
        int besty = -1;
        int bestManhattan = int.MinValue;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (roomMap[x,y].rt == RoomType.Basic)
                {
                    var m = RogueUtils.Manhattan(x,xstart,y,ystart);
                    if(m > bestManhattan)
                    {
                        bestx = x;
                        besty = y;
                        bestManhattan = m;
                    }
                }
            }
        }

        if(bestx != -1 && besty != -1)
        {
            roomMap[bestx,besty].rt = RoomType.Boss;
        }
    }

}

public struct Room
{
    public RoomType rt;

    //Point cardinaux a true quand la direction est libre
    public bool up;
    public bool right;
    public bool left;
    public bool down;
}

public enum RoomType
{
    Invalid = 0,
    Basic = 1,
    Start = 2,
    Boss = 3
}
