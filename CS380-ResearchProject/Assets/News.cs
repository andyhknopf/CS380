using System.Collections.Generic;
using System.Drawing;
using UnityEngine; // color

[System.Serializable]
public class News
{
    [SerializeField] UnityEngine.Color id;
    public Subject subject;
    public NewsAction action;


    [HideInInspector]public List<GridNode> reached = new List<GridNode>();
    [HideInInspector] public string newsString;
    [SerializeField] public int speed = 1;

    private SortedList<int, Queue<GridNode>> pending = new SortedList<int, Queue<GridNode>>();

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
        this.id = color;
    }

    public News()
    {
        //newsString = subject.name + " " + action.text;
    }

    public News(News other)
    {
        id = other.id;
        reached = other.reached;
        pending = other.pending;
        subject = other.subject;
        action = other.action;
        newsString = other.newsString;
        speed = other.speed;
    }

    public UnityEngine.Color GetID()
    {
        return this.id;
    }

    public void Plant(GridNode origin)
    {
        // this.id = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

        reached.Add(origin);
        int initialDelay = speed * origin.spreadCost;
        Enqueue(origin, initialDelay);
    }

    public List<GridNode> Spread(int currentTurn, GridNode[,] grid, int width, int height)
    {
        List<GridNode> newlyReached = new List<GridNode>();

        while (pending.Count > 0 && pending.Keys[0] <= currentTurn)
        {
            var queue = pending.Values[0];
            var node = queue.Dequeue();
            if (queue.Count == 0)
                pending.RemoveAt(0);

            foreach (var neighbor in GetNeighbors(node, grid, width, height))
            {
                if (reached.Contains(neighbor)) continue;
                if (neighbor.spreadCost == TerrainConstants.BLOCKED) continue;

                int delay = speed * neighbor.spreadCost;
                int arrivalTurn = currentTurn + delay;

                reached.Add(neighbor);
                Enqueue(neighbor, arrivalTurn);
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
    private void Enqueue(GridNode node, int arrivalTurn)
    {
        if (!pending.ContainsKey(arrivalTurn))
            pending[arrivalTurn] = new Queue<GridNode>();
        pending[arrivalTurn].Enqueue(node);
    }
}