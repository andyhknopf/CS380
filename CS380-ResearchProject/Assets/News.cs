using System.Collections.Generic;
using System.Drawing;
using UnityEngine; // color

[System.Serializable]
public class News
{
    UnityEngine.Color color;
    [HideInInspector]public List<GridNode> reached = new List<GridNode>();
    private Queue<(GridNode node, int spreadAtTurn)> pending = new Queue<(GridNode, int)>();
    public Subject subject;
    public NewsAction action;
    [HideInInspector] public string newsString;


    [SerializeField]
    public OpinionInfluencer influencer;
    public enum OpinionInfluencer : int
    {
      DISTANCE_FROM_SUBJECT = 0,
      LOYALTY_TO_KING = 1,
      LATTITUDE = 2,
    }


    public News(UnityEngine.Color color)
    {
      newsString = subject.name + " " + action.text;
      this.color = color;
    }

    public News()
    {
      newsString = subject.name + " " + action.text;
    }

  public News(News other)
  {
    color = other.color;
    reached = other.reached;
    pending = other.pending;
    subject = other.subject;
    action = other.action;
    newsString = other.newsString;
  }

    public UnityEngine.Color GetColor()
    {
        return this.color;
    }

    public void Plant(GridNode origin)
    {
        this.color = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

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