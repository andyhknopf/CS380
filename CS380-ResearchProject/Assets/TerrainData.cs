using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Grid/TerrainData")]
public class TerrainData : ScriptableObject
{
    public TerrainType terrainType;
    public int spreadCost = 1;
}
