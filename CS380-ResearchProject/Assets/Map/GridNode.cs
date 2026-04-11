using UnityEngine;

public enum TerrainType { FIELD = 0, FOREST = 1, MOUNTAIN = 2 }

public static class TerrainConstants
{
    public const int BLOCKED = -1;
}

public class GridNode
{
    public int x, y;
    public TerrainType terrain;
    public Vector3 worldPos;
    public GameObject visual;
    public int spreadCost = 1;
    public int leftCount = 1;
}