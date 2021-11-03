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
        map = new Map(new Vector2Int(20, 10));

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
        foreach(Cell cell in map.data)
        {
            float half = TerrainSettings.UnitSize / 2;
            Vector3 a = new Vector3(cell.Position.x * TerrainSettings.UnitSize + half, 0f, cell.Position.y * TerrainSettings.UnitSize - half);
            Vector3 b = new Vector3(cell.Position.x * TerrainSettings.UnitSize + half, 0f, cell.Position.y * TerrainSettings.UnitSize + half);
            Vector3 c = new Vector3(cell.Position.x * TerrainSettings.UnitSize - half, 0f, cell.Position.y * TerrainSettings.UnitSize + half);
            Vector3 d = new Vector3(cell.Position.x * TerrainSettings.UnitSize - half, 0f, cell.Position.y * TerrainSettings.UnitSize - half);
            mesher.AddQuad(a, b, c, d);
        }

        /* Generate the mesh and apply it */
        filter.mesh = mesher.GenerateMesh();
        mrenderer.material = material;
    }
}
