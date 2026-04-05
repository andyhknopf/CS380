using System.Collections.Generic;

public class News
{
    public List<GridNode> reached = new List<GridNode>();
    private Queue<(GridNode node, int spreadAtTurn)> pending = new Queue<(GridNode, int)>();

    public void Plant(GridNode origin)
    {
        reached.Add(origin);
        pending.Enqueue((origin, origin.spreadCost));
    }

    public List<GridNode> Spread(int currentTurn, GridNode[,] grid, int width, int height)
    {
        List<GridNode> newlyReached = new List<GridNode>();

        while (pending.Count > 0 && pending.Peek().spreadAtTurn <= currentTurn)
        {
            var (node, _) = pending.Dequeue();

            foreach (var neighbor in GetNeighbors(node, grid, width, height))
            {
                if (reached.Contains(neighbor)) continue;
                if (neighbor.spreadCost == TerrainConstants.BLOCKED) continue;

                reached.Add(neighbor);
                pending.Enqueue((neighbor, currentTurn + neighbor.spreadCost));
                newlyReached.Add(neighbor);
            }
        }

        return newlyReached;
    }

    List<GridNode> GetNeighbors(GridNode node, GridNode[,] grid, int width, int height)
    {
        List<GridNode> neighbors = new List<GridNode>();
        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = node.x + dx[i];
            int ny = node.y + dy[i];
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                neighbors.Add(grid[nx, ny]);
        }

        return neighbors;
    }
}