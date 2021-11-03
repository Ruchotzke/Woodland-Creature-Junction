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
                data[i] = new Cell(x, y, Mathf.RoundToInt(6 * Mathf.PerlinNoise((float)x / Size.x * 6.0f, (float)y / Size.y * 6.0f)), Color.white);
            }
        }
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
