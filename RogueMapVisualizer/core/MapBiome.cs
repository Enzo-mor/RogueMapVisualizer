public class MapBiome
{
    int maxWidth;
    int minWidth;
    int maxHeight;
    int minHeight;
    int minMinusCase;
    int maxMinusCase;

    bool[,] map;

    int width;
    int height;

    public List<List<(int x, int y)>> clusters;

    //Tableau pour Kruskal
    int[] parent;

    int seed;

    public List<bool[,]> steps = new List<bool[,]>();

    public MapBiome(int maxWidth,int minWidth,int maxHeight,int minHeight,int minMinusCase,int maxMinusCase,int seed)
    {
        this.maxWidth = maxWidth;
        this.minWidth = minWidth;
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.minMinusCase = minMinusCase;
        this.maxMinusCase = maxMinusCase;
        this.seed = seed;
    }

    public bool[,] generateMap()
    {
        steps.Clear();
        RandomProvider.Init(seed);

        width = RandomProvider.Range(minWidth, maxWidth+1);
        height = RandomProvider.Range(minHeight, maxHeight+1);

        map = new bool[width,height];

        int minusCase = RandomProvider.Range(minMinusCase, maxMinusCase);
        int tries = 0;

        for(int i = 0; i < minusCase && tries < 50; i++) {
            if(!ErasedACase(map)){
                i--;
                tries++;
            }
        }

        if(tries >= 50)Console.WriteLine("Trop d'essaies a la génération du biomes");
        steps.Add(CopyMap(map));

        clusters = FindsClusters();
        LinkCluster();
        steps.Add(CopyMap(map));
        EnsureBorderConnection();
        steps.Add(CopyMap(map));

        return map;
    }

    //Supprime une case en bordure
    private bool ErasedACase(bool[,] map)
    {

        int x = 0;
        int y = 0;
        int dx = 0;
        int dy = 0;

        // 1. Choix du côté
        int side = RandomProvider.Range(0, 4);

        switch (side)
        {
            case 0: // Haut
                x = RandomProvider.Range(0, width);
                y = height - 1;
                dx = 0;
                dy = -1;
                break;

            case 1: // Bas
                x = RandomProvider.Range(0, width);
                y = 0;
                dx = 0;
                dy = 1;
                break;

            case 2: // Gauche
                x = 0;
                y = RandomProvider.Range(0, height);
                dx = 1;
                dy = 0;
                break;

            case 3: // Droite
                x = width - 1;
                y = RandomProvider.Range(0, height);
                dx = -1;
                dy = 0;
                break;
        }

        // 2. Avancer tant que c'est déjà creusé
        while (IsInside(x,y))
        {
            if (!map[x, y])
            {
                map[x, y] = true;
                return true;
            }

            x += dx;
            y += dy;
        }
        return false;
    }

    private bool VerifyBorder()
    {
        // Colonnes gauche et droite
        for (int y = 0; y < height; y++)
        {
            if (!map[0, y]) return true;
            if (!map[width - 1, y]) return true;
        }

        // Lignes haut et bas
        for (int x = 0; x < width; x++)
        {
            if (!map[x, 0]) return true;
            if (!map[x, height - 1]) return true;
        }

        return false;
    }


    //Trouver une case false
    private (int x,int y) FindFirstFalse(bool[,] map)
    {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                if(!map[i,j])return (i,j);
            }
        }
        return (-1,-1);
    }

    bool[,] CopyMap(bool[,] original)
    {
        bool[,] copy = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                copy[x, y] = original[x, y];
            }
        }

        return copy;
    }

    //Trouver les clusters
    private List<List<(int x, int y)>> FindsClusters()
    {
        List<List<(int x, int y)>> clusters = new List<List<(int, int)>>();

        List<(int x, int y)> currentCluster;

        bool[,] copy = CopyMap(map);

        while (!IsAllTrue(copy))
        {
            currentCluster = FindCluster(copy);
            clusters.Add(currentCluster);
        }

        return clusters;
    }

    private List<(int, int)> FindCluster(bool[,] myMap)
    {
        List<(int x, int y)> cluster = new List<(int, int)>();

        (int x,int y) = FindFirstFalse(myMap);

        if (x != -1)
            FindClusterRecc(myMap, cluster, x, y);

        return cluster;
    }

    private void FindClusterRecc(bool[,] myMap,List<(int x, int y)> cluster,int x,int y)
    {
        if(IsInside(x,y) && !myMap[x, y])
        {
            myMap[x,y] = true;
            cluster.Add((x,y));
            FindClusterRecc(myMap,cluster,x+1,y);
            FindClusterRecc(myMap,cluster,x-1,y);
            FindClusterRecc(myMap,cluster,x,y+1);
            FindClusterRecc(myMap,cluster,x,y-1);
        }
        else
        {
            return;
        }
    }

    private bool IsInside(int x, int y)
    {
        return x >= 0 &&
            y >= 0 &&
            x < width &&
            y < height;
    }

    public bool IsAllTrue(bool[,] myMap)
    {
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                if(!myMap[i,j])return false;
            }
        }
        return true;
    }

    //MST kruskal
    private void LinkCluster()
    {
        List<(int clusterA, int clusterB, int cost, (int, int) pointA, (int, int) pointB)> edges = new List<(int, int, int, (int,int), (int,int))>();
        
        //Build edges list
        for(int i = 0; i < clusters.Count; i++) {
            for(int j = i+1; j < clusters.Count; j++) {
                var (p1, p2) = findClosestPoint(clusters[i], clusters[j]);
                int distance = RogueUtils.Manhattan(p1.x, p2.x, p1.y, p2.y);
                edges.Add((i, j, distance, p1, p2));
            }
        }

        edges.Sort((a, b) => a.cost.CompareTo(b.cost));

        parent = new int[clusters.Count];
        for(int i = 0; i < parent.Length; i++) parent[i] = i;

        List<((int,int),(int,int))> connections = new List<((int,int),(int,int))>();

        foreach(var edge in edges) {
            int rootA = Find(edge.clusterA);
            int rootB = Find(edge.clusterB);

            if(rootA != rootB) {
                Union(rootA, rootB);
                connections.Add((edge.pointA, edge.pointB));
            }
        }

        foreach(var conn in connections) {
            DigPath(conn.Item1, conn.Item2);
        }
    }

    private int Find(int x) {
        if(parent[x] != x) parent[x] = Find(parent[x]);
        return parent[x];
    }

    private void Union(int x, int y) {
        parent[Find(x)] = Find(y);
    }

    private void DigPath((int x, int y) a, (int x, int y) b)
    {
        int x = a.x;
        int y = a.y;

        while (x != b.x)
        {
            map[x, y] = false;
            x += (b.x > x) ? 1 : -1;
        }

        while (y != b.y)
        {
            map[x, y] = false;
            y += (b.y > y) ? 1 : -1;
        }

        map[x, y] = false;
    }


    private ((int x, int y),(int x, int y)) findClosestPoint(List<(int,int)> c1, List<(int,int)> c2)
    {
        int min = int.MaxValue;
        int manhattan;
        (int, int) bestPointOfC1 = (-1,-1);
        (int, int) bestPointOfC2 = (-1,-1);


        foreach ((int x,int y)p1 in c1){
            foreach ((int x,int y)p2 in c2){
                manhattan = RogueUtils.Manhattan(p1.x,p2.x,p1.y,p2.y);
                if (manhattan < min)
                {
                    min = manhattan;
                    bestPointOfC1 = p1;
                    bestPointOfC2 = p2;
                }
            }
        }

        return (bestPointOfC1,bestPointOfC2);
    }

    private (int x, int y) FindClosestBorderPoint((int x, int y) from)
    {
        int bestDist = int.MaxValue;
        (int x, int y) best = (-1, -1);

        // Colonnes
        for (int y = 0; y < height; y++)
        {
            Check(0, y);
            Check(width - 1, y);
        }

        // Lignes
        for (int x = 0; x < width; x++)
        {
            Check(x, 0);
            Check(x, height - 1);
        }

        return best;

        void Check(int x, int y)
        {
            int d = RogueUtils.Manhattan(from.x, x, from.y, y);
            if (d < bestDist)
            {
                bestDist = d;
                best = (x, y);
            }
        }
    }

    private void EnsureBorderConnection()
    {
        if (VerifyBorder())
            return;

        var start = FindFirstFalse(map);
        if (start.x == -1)
        {
            Console.WriteLine("Map Pleine");
            return;
        }

        var border = FindClosestBorderPoint(start);
        DigPath(start, border);
    }

}
