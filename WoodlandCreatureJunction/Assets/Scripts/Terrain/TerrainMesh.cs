using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour
{
    public Material material;
    public Map map;
    public Vector2Int size;
    public int numHomes = 10;

    [Header("Prefabs")]
    public GameObject pf_House;
    public Villager pf_Villager;

    private Vector2Int NumChunks;

    MeshFilter[] filters;
    

    private void Start()
    {
        map = new Map(size);
        NumChunks = new Vector2Int(Mathf.CeilToInt((float)size.x / TerrainSettings.ChunkSize), Mathf.CeilToInt((float)size.y / TerrainSettings.ChunkSize));

        filters = new MeshFilter[NumChunks.x * NumChunks.y];

        GenerateChunks();

        /* DEBUG INFO: Flat spots */
        var spots = map.FindFlatSpots(new Vector2Int(5, 5));

        /* Sort the spots by height - we want to prefer higher places */
        spots.Sort((Vector2Int a, Vector2Int b) => b.y - a.y);
        for(int i = 0; i < Mathf.Min(numHomes, spots.Count); i++)
        {
            /* Generate a home at this location */
            Vector2Int spot = spots[i];
            spots.Remove(spot);
            Cell chosen = map.GetCell(spot.x, spot.y);

            /* Generate home */
            var Home = Instantiate(pf_House);
            Home.transform.position = CellToWorld(chosen);
            Home.transform.Rotate(0f, 90f * Random.Range(0, 4), 0f);

            /* Generate a villager for this house */
            var villager = Instantiate(pf_Villager);
            villager.name = "VILLAGER " + i;
            villager.home = Home.transform;
            villager.transform.position = Home.transform.position;
        }
    }

    public Vector3 CellToWorld(Cell cell)
    {
        return new Vector3(cell.Position.x * TerrainSettings.UnitSize, cell.height * TerrainSettings.UnitHeight, cell.Position.y * TerrainSettings.UnitSize);
    }

    public Cell WorldToCell(Vector3 position)
    {
        Vector2Int cellpos = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        return map.GetCell(cellpos.x, cellpos.y);
    }

    void GenerateChunks()
    {
        /* Generate a portion of the mesh through each chunk */
        for (int y = 0; y < NumChunks.y; y++)
        {
            for (int x = 0; x < NumChunks.x; x++)
            {
                /* Generate and store the chunk object */
                GameObject chunk = new GameObject("Chunk " + x + ", " + y);
                chunk.transform.parent = transform;
                chunk.AddComponent<MeshRenderer>().material = material;
                chunk.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                filters[x + y * NumChunks.x] = chunk.AddComponent<MeshFilter>();

                /* Generate a mesh for this chunk */
                List<Cell> chunkCells = new List<Cell>();
                for (int cy = 0; cy < TerrainSettings.ChunkSize; cy++)
                {
                    for (int cx = 0; cx < TerrainSettings.ChunkSize; cx++)
                    {
                        chunkCells.Add(map.GetCell(cx + x * TerrainSettings.ChunkSize, cy + y * TerrainSettings.ChunkSize));
                    }
                }
                GenerateTerrain(filters[x + y * NumChunks.x], chunkCells);
                chunk.AddComponent<MeshCollider>();
            }
        }
    }

    void GenerateTerrain(MeshFilter chunkFilter, List<Cell> chunkCells)
    {
        /* First create a mesher for us to use */
        /* REMEMBER: XY in 2D is XZ in 3D! */
        Mesher mesher = new Mesher();

        /* Using Unit Sizes, generate a quad for each, centered at that quad's position */
        foreach (Cell cell in chunkCells)
        {
            /* First generate the face for this cell */
            mesher.AddQuad(
                new Vector3(cell.Position.x - TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y - TerrainSettings.CellFaceOffset),
                new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y - TerrainSettings.CellFaceOffset),
                new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset),
                new Vector3(cell.Position.x - TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset),
                cell.Color, cell.Color, cell.Color, cell.Color);

            /* Each Cell is also responsible for bridging to it's +x and +z neighbors */
            if(cell.Position.x < map.Size.x - 1)
            {
                GenerateBridgeX(cell, mesher);
            }

            if(cell.Position.y < map.Size.y - 1)
            {
                GenerateBridgeZ(cell, mesher);
            }

            if (cell.Position.y < map.Size.y - 1 && cell.Position.x < map.Size.x - 1)
            {
                GenerateCornerBridge(cell, mesher);
            }
        }

        /* Generate the mesh and apply it */
        chunkFilter.mesh = mesher.GenerateMesh();
    }

    private void GenerateBridgeX(Cell cell, Mesher mesher)
    {
        Cell neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y);

        /* First two points are on the edge of the cell */
        Vector3 a = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);
        Vector3 b = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y - TerrainSettings.CellFaceOffset);

        /* Next two points are on the edge of the neighbor */
        Vector3 c = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        Vector3 d = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y + TerrainSettings.CellFaceOffset);

        //are we going to terrace? */
        if(Mathf.Abs(cell.height - neighbor.height) == 1)
        {
            Terrace(mesher, cell, a, b, neighbor, d, c);
        }
        else
        {
            mesher.AddQuad(a, b, c, d, cell.Color, cell.Color, neighbor.Color, neighbor.Color);
        }
    }

    private void GenerateBridgeZ(Cell cell, Mesher mesher)
    {
        Cell neighbor = map.GetCell(cell.Position.x, cell.Position.y + 1);

        /* First two points are on the edge of the cell */
        Vector3 a = new Vector3(cell.Position.x - TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);
        Vector3 b = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);

        /* Next two points are on the edge of the neighbor */
        Vector3 c = new Vector3(neighbor.Position.x + TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        Vector3 d = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);

        //are we going to terrace? */
        if (Mathf.Abs(cell.height - neighbor.height) == 1)
        {
            Terrace(mesher, cell, a, b, neighbor, d, c);
        }
        else
        {
            mesher.AddQuad(a, b, c, d, cell.Color, cell.Color, neighbor.Color, neighbor.Color);
        }
    }

    private void GenerateCornerBridge(Cell cell, Mesher mesher)
    {
        /* Starting from our corner, move CCW and make a quad from the 4 corners */
        Vector3 a = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);

        Cell neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y);
        Vector3 b = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y + TerrainSettings.CellFaceOffset);
        Color cb = neighbor.Color;

        neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y + 1);
        Vector3 c = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        Color cc = neighbor.Color;

        neighbor = map.GetCell(cell.Position.x, cell.Position.y + 1);
        Vector3 d = new Vector3(neighbor.Position.x + TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        Color cd = neighbor.Color;

        mesher.AddQuad(a, b, c, d, cell.Color, cb, cc, cd);

        ///* Starting from our corner, move CCW and make a 4 triangles from the 4 corners */
        //Vector3 a = new Vector3(cell.Position.x + TerrainSettings.CellFaceOffset, cell.height * TerrainSettings.UnitHeight, cell.Position.y + TerrainSettings.CellFaceOffset);

        //Cell neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y);
        //Vector3 b = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y + TerrainSettings.CellFaceOffset);
        //Color cb = neighbor.Color;

        //neighbor = map.GetCell(cell.Position.x + 1, cell.Position.y + 1);
        //Vector3 c = new Vector3(neighbor.Position.x - TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        //Color cc = neighbor.Color;

        //neighbor = map.GetCell(cell.Position.x, cell.Position.y + 1);
        //Vector3 d = new Vector3(neighbor.Position.x + TerrainSettings.CellFaceOffset, neighbor.height * TerrainSettings.UnitHeight, neighbor.Position.y - TerrainSettings.CellFaceOffset);
        //Color cd = neighbor.Color;

        //Vector3 midpoint = (a + b + c + d) / 4.0f;
        //Color midcolor = (cell.Color + cb + cc + cd) / 4.0f;

        ///* Generate 4 triangles to correctly bridge this gap */
        //mesher.AddTriangle(a, b, midpoint, cell.Color, cb, midcolor);
        //mesher.AddTriangle(b, c, midpoint, cb, cc, midcolor);
        //mesher.AddTriangle(c, d, midpoint, cc, cd, midcolor);
        //mesher.AddTriangle(d, a, midpoint, cd, cell.Color, midcolor);
    }

    public void Terrace(Mesher mesher, Cell start, Vector3 sl, Vector3 sr, Cell end, Vector3 el, Vector3 er)
    {
        /* First do the first slope */
        Vector3 nl = TerraceLerp(sl, el, 1);
        Vector3 nr = TerraceLerp(sr, er, 1);
        Color intermediate = TerraceLerp(start.Color, end.Color, 1);
        mesher.AddQuad(sl, sr, nr, nl, start.Color, start.Color, intermediate, intermediate);

        /* Do the intermediate connection */
        for(int i = 2; i < TerrainSettings.TerraceInterp; i++)
        {
            Vector3 left = nl;
            Vector3 right = nr;
            Color old = intermediate;

            nl = TerraceLerp(sl, el, i);
            nr = TerraceLerp(sr, er, i);
            intermediate = TerraceLerp(start.Color, end.Color, i);

            mesher.AddQuad(left, right, nr, nl, old, old, intermediate, intermediate);
        }

        /* Finally, connect to the endpooint */
        mesher.AddQuad(nl, nr, er, el, intermediate, intermediate, end.Color, end.Color);


    }

    /// <summary>
    /// A terrace interpolation function. Inspiration from
    /// https://catlikecoding.com/unity/tutorials/hex-map/part-3/
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
    {
        /* What do we need to multiply here */
        float h = step * TerrainSettings.HorizontalTerraceStep;
        float v = ((step + 1) / 2) * TerrainSettings.VerticalTerraceStep;

        /* Calculate the interpolated point */
        a.x += (b.x - a.x) * h;
        a.y += (b.y - a.y) * v;
        a.z += (b.z - a.z) * h;

        /* Return the new point */
        return a;
    }

    /// <summary>
    /// A terrace color lerp. Based on 
    /// https://catlikecoding.com/unity/tutorials/hex-map/part-3/
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * TerrainSettings.HorizontalTerraceStep;
        return Color.Lerp(a, b, h);
    }
}
