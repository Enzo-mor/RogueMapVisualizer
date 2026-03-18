//Classe qui stocke les données d'une "room", le type de la room ainsi que les murs présents
public class DataRoom
{
    public Room room;
    public Wall northWall;
    public Wall eastWall;
    public Wall westWall;
    public Wall southWall;


    public DataRoom(Room r)
    {
        room = r;
        northWall = new Wall(Direction.Up);
        eastWall = new Wall(Direction.Right);
        southWall = new Wall(Direction.Down);
        westWall =new Wall(Direction.Left);
    }

    public bool IsValid()
    {
        return room.rt != RoomType.Invalid;
    }

    public bool IsSpecial()
    {
        return (int)room.rt > 1;
    }

    public void GenerateOneWall(DataRoom neighbor, Direction d) 
    {
        // Si le voisin est inexistant ou invalide, on met un mur full
        if(neighbor == null || !neighbor.IsValid())
        {
            generateFullWall(d);
            return;
        }

        if(doYouHaveAWall(neighbor, d) && !IsSpecial())
        {
            copyNeighboor(neighbor, d);
            return;
        }

        switch(d)
        {
            case Direction.Up: northWall.generate(room.rt); break;
            case Direction.Right: eastWall.generate(room.rt); break;
            case Direction.Down: southWall.generate(room.rt); break;
            case Direction.Left: westWall.generate(room.rt); break;
        }

        // Synchronisation avec le voisin
        neighbor.copyNeighboor(this, RogueUtils.InverseDirection(d));
    }

    private void generateFullWall(Direction d)
    {
        switch (d) {
            case Direction.Up : northWall.setFull(); break;
            case Direction.Right : eastWall.setFull(); break;
            case Direction.Down : southWall.setFull(); break;
            case Direction.Left : westWall.setFull(); break;
        }
    }

    private bool doYouHaveAWall(DataRoom dr,Direction d)
    {
        switch (d) {
            case Direction.Up : return dr.southWall.isGenerated();
            case Direction.Right : return dr.westWall.isGenerated();
            case Direction.Down : return dr.northWall.isGenerated();
            case Direction.Left : return dr.eastWall.isGenerated();
            default : return false;
        }
    }

    private void copyNeighboor(DataRoom dr,Direction d)
    {
        if(dr.room.rt != RoomType.Invalid){
            switch (d) {
                case Direction.Up : northWall.synchronized(dr.southWall); break;
                case Direction.Right : eastWall.synchronized(dr.westWall); break;
                case Direction.Down : southWall.synchronized(dr.northWall); break;
                case Direction.Left : westWall.synchronized(dr.eastWall); break;
                default : break;
            }
        }
    }
}