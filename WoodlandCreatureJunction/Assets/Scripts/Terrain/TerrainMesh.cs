using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    public Material material;
    public Map map;

    private MeshFilter filter;
    private MeshRenderer mrenderer;

    private void Start()
    {
        map = new Map(new Vector2Int(20, 20));

        filter = GetComponent<MeshFilter>();
        mrenderer = GetComponent<MeshRenderer>();

        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        /* First create a mesher for us to use */
        /* REMEMBER: XY in 2D is XZ in 3D! */
        Mesher mesher = new Mesher();

        /* Using Unit Sizes, generate a quad for each, centered at that quad's position */
        foreach (Cell cell in map.data)
        {
            /* First generate the face for this cell */
            mesher.AddQuad(
                new Vector3(cell.Position.x - TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y - TerrainSettings.CellFaceOffset),
                new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y - TerrainSettings.CellFaceOffset),
                new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset),
                new Vector3(cell.Position.x - TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset));

            /* Each Cell is also responsible for bridging to it's +x and +z neighbors */
            if(cell.Position.x < map.Size.x - 1)
            {
                var points = GenerateBridgeX(cell);
                mesher.AddQuad(points.a, points.b, points.c, points.d);
            }

            if(cell.Position.y < map.Size.y - 1)
            {
                var points = GenerateBridgeZ(cell);
                mesher.AddQuad(points.a, points.b, points.c, points.d);
            }

            if (cell.Position.y < map.Size.y - 1 && cell.Position.x < map.Size.x - 1)
            {
                var points = GenerateCornerBridge(cell);
                mesher.AddQuad(points.a, points.b, points.c, points.d);
            }
        }

        /* Generate the mesh and apply it */
        filter.mesh = mesher.GenerateMesh();
        mrenderer.material = material;
    }

    private (Vector3 a, Vector3 b, Vector3 c, Vector3 d) GenerateBridgeX(Cell cell)
    {
        Cell neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y);

        /* First two points are on the edge of the cell */
        Vector3 a = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);
        Vector3 b = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y - TerrainSettings.CellFaceOffset);

        /* Next two points are on the edge of the neighbor */
        Vector3 c = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        Vector3 d = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y + TerrainSettings.CellFaceOffset);

        return (a, b, c, d);
    }

    private (Vector3 a, Vector3 b, Vector3 c, Vector3 d) GenerateBridgeZ(Cell cell)
    {
        Cell neighbor = map.GetCell(cell.Position.x, cell.Position.y + 1);

        /* First two points are on the edge of the cell */
        Vector3 a = new Vector3(cell.Position.x - TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);
        Vector3 b = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);

        /* Next two points are on the edge of the neighbor */
        Vector3 c = new Vector3(neighbor.Position.x + TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        Vector3 d = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);

        return (a, b, c, d);
    }

    private (Vector3 a, Vector3 b, Vector3 c, Vector3 d) GenerateCornerBridge(Cell cell)
    {
        /* Starting from our corner, move CCW and make a quad from the 4 corners */
        Vector3 a = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);

        Cell neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y);
        Vector3 b = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y + TerrainSettings.CellFaceOffset);

        neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y + 1);
        Vector3 c = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);

        neighbor = map.GetCell(cell.Position.x, cell.Position.y + 1);
        Vector3 d = new Vector3(neighbor.Position.x + TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);

        return (a, b, c, d);
    }
}
