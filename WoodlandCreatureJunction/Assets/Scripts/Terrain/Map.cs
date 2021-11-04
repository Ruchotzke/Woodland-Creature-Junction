using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public readonly Vector2Int Size;
    public Cell[] data;

    public Map(Vector2Int size)
    {
        this.Size = size;
        data = new Cell[size.x * size.y];
        for(int y = 0, i = 0; y < size.y; y++)
        {
            for(int x = 0; x < size.x; x++, i++)
            {
                data[i] = new Cell(x, y, Mathf.RoundToInt(6 * Mathf.PerlinNoise(2 * (float)x / Size.x, 2 * (float)y / Size.y)), Color.green / 2.5f);
            }
        }

        //GenerateTerrain();
    }

    /// <summary>
    /// Generate an interesting terrain from the flat original terrain.
    /// </summary>
    private void GenerateTerrain()
    {
        /* Step 1: Select a few "chunks" to raise by one */
        /* Step 2: Raise terrain, and recursively continue to raise terrain using smaller chunks*/

        for(int chunk = 0; chunk < 5; chunk++)
        {
            int octaves = 3;
            List<Cell> curr = new List<Cell>(data);
            for (int i = 0; i < octaves; i++)
            {
                /* Make sure we have cells left to edit */
                if (curr.Count < 3) break;

                /* Generate a set of cells to rise up */
                List<Cell> region = GetChunk(curr[Random.Range(0, curr.Count)], 60 / (i + 1), curr);

                Color regionColor = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
                foreach (var cell in region)
                {
                    cell.height = Mathf.Min(cell.height + 1, 4);
                    cell.Color = regionColor;
                }
                curr = region;
            }
        }
    }

    /// <summary>
    /// Select a pseudorandom chunk of cells based on a position.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="size"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    private List<Cell> GetChunk(Cell origin, int size, List<Cell> input)
    {
        List<Cell> chunk = new List<Cell>();

        Queue<Cell> open = new Queue<Cell>();
        open.Enqueue(origin);

        while(open.Count > 0 && chunk.Count < size)
        {
            Cell next = open.Dequeue();
            chunk.Add(next);

            Cell top = input.Find(c => c.Position == next.Position + new Vector2Int(0, 1));
            Cell bottom = input.Find(c => c.Position == next.Position + new Vector2Int(0, -1));
            Cell left = input.Find(c => c.Position == next.Position + new Vector2Int(-1, 0));
            Cell right = input.Find(c => c.Position == next.Position + new Vector2Int(1, 0));

            if (top != null && !chunk.Contains(top) && !open.Contains(top)) open.Enqueue(top);
            if (bottom != null && !chunk.Contains(bottom) && !open.Contains(bottom)) open.Enqueue(bottom);
            if (left != null && !chunk.Contains(left) && !open.Contains(left)) open.Enqueue(left);
            if (right != null && !chunk.Contains(right) && !open.Contains(right)) open.Enqueue(right);
        }


        return chunk;
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Size.x || y >= Size.y) Debug.LogError("Access out of bounds.");

        return data[x + y * Size.x];
    }

    public void SetCell(int x, int y, Cell value)
    {
        if (x < 0 || y < 0 || x >= Size.x || y >= Size.y) Debug.LogError("Access out of bounds.");

        data[x + y * Size.x] = value;
    }
}

public class Cell
{
    public Vector2Int Position;
    public Color Color;
    public int height;

    public Cell(int x, int y, int height, Color color)
    {
        this.Position = new Vector2Int(x, y);
        this.Color = color;
        this.height = height;
    }
}
