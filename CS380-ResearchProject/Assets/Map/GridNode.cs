using System.Collections.Generic;
using UnityEngine;

public enum TerrainType { FIELD = 0, FOREST = 1, MOUNTAIN = 2 }

public static class TerrainConstants
{
    public const int BLOCKED = -1;
}

[System.Serializable]
public class GridNode
{
    public int x, y;
    public TerrainType terrain;
    public Vector3 worldPos;
    public GameObject visual;
    public GameObject bgVisual;

    public int spreadCost = 1;
    public List<Color> newsColors = new List<Color>();
    
    public int leftCount = 1; // Still working on this implementation, currently not being used
    // Purpose: fix the way delay(turn based) is working
}