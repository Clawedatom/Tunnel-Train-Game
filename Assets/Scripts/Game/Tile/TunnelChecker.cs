using System.Collections.Generic;
using UnityEngine;

public static class TunnelChecker 
{
    static TileManager tileManager;


    public static List<Vector2Int> SearchTunnel(TileManager manager)
    {
        tileManager = manager;
        Vector2Int startIndex = tileManager.WorldToGrid(tileManager.GetStartPos());

        List<Vector2Int> path = TunnelBFS(startIndex);

        return path;
    }
    private static List<Vector2Int> TunnelBFS(Vector2Int start) // returns tunnel from start zone to end zone
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();  //order of explore
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();  //tracks visited cells
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>(); // actual path - key = currentTile, value  = prev tile



        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] dirs =
        {
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.up,
        Vector2Int.down
    };

        while (queue.Count > 0) // nothing left to searcn
        {
            Vector2Int current = queue.Dequeue();

            if (tileManager.IsGridIndexInFinish(current.x, current.y))
            {
                return ReconstructPath(cameFrom, start, current);
            }

            foreach (var dir in dirs)
            {
                Vector2Int neighbour = current + dir;

                if (!tileManager.IsInBounds(neighbour.x, neighbour.y))
                    continue;

                // Must be dug tunnel space
                if (!tileManager.IsDug(neighbour))
                    continue;



                if (visited.Contains(neighbour))
                    continue;

                //atp the cell is a valid
                visited.Add(neighbour);
                cameFrom[neighbour] = current;
                queue.Enqueue(neighbour);
            }
        }

        return null;
    }
    
    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end) // create full path
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();
        return path;

    }
}
