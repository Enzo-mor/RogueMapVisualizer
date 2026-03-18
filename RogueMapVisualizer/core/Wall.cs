public class Wall
{
    public WallType wallType{get;private set;}
    public Direction d{get;private set;}

    public Wall( Direction d)
    {
        wallType = WallType.notGenerate;
        this.d = d;
    }

    public void generate(RoomType rt)
    {
        if (!isGenerated())
        {
            wallType =  WallSelector.Select(rt,d);
        }
    }

    public void setFull()
    {
        wallType = WallType.full;
    }

    //l'apelleur prend le type du parametre
    public void synchronized(Wall w)
    {
        wallType = w.wallType;
    }

    public bool isGenerated()
    {
        return wallType != WallType.notGenerate;
    }

}

public enum WallType
{
    notGenerate = -1,
    full = 0,
    empty = 1,
    onePassage = 2,
    twoPassage = 3,
    threePassage = 4
}
