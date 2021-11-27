using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public readonly Vector2Int Size;
    public Cell[] data;

    const float HEIGHT = 25f;
    const int OCTAVES = 3;
    const float PERSISTENCE = 0.8f;
    const float EXPONENT = 2f;

    public Map(Vector2Int size)
    {
        this.Size = size;

        /* Generate a map */
        data = new Cell[size.x * size.y];
        for(int y = 0, i = 0; y < size.y; y++)
        {
            for(int x = 0; x < size.x; x++, i++)
            {
                var sample = SampleTerrain(x, y);
                data[i] = new Cell(x, y, Mathf.RoundToInt(sample.height), sample.biome.GetColor());
            }
        }

        /* Find any flat spots for houses */
        Debug.Log("Flat 3x3 spots: " + FindFlatSpots().Count);
        Poisson poisson = new Poisson(new Rect(0f, 0f, 100f, 100f), 1);
        List<Vector2> points = poisson.SamplePoints();
        Debug.Log("Poisson complete: " + points.Count + " generated.");
    }

    private (float height, Biome biome) SampleTerrain(int x, int y)
    {
        /* Do noise sampling for base sample */
        /* Generate both a height map and a moisture map */
        float heightSample = 0.0f;
        float moistureSample = 0.0f;
        float amplitude = 1.0f;
        float frequency = 6.0f;
        float divisor = 0.0f;
        for(int i = 0; i < OCTAVES; i++)
        {
            /* Calculate this octaves noise. Add an offset to prevent correlation */
            heightSample += amplitude * Noise.GenerateNoise(new Vector2((float)x / Size.x, (float)y / Size.y), Vector2.one * frequency, 0.13f * i * Vector2.one);
            moistureSample += amplitude * Noise.GenerateNoise(new Vector2((float)x / Size.x, (float)y / Size.y), Vector2.one * frequency * 0.5f, 0.97f * i * Vector2.one);
            divisor += amplitude;
            amplitude *= PERSISTENCE;
            frequency *= 2;
        }
        heightSample /= divisor;
        heightSample = Mathf.Pow(heightSample * 1.2f, EXPONENT);

        float height = heightSample * HEIGHT;

        /* Calculate the biome based on height */
        Biome biome = FindBiome(heightSample, moistureSample);


        return (height, biome);
    }

    Biome FindBiome(float height, float moisture)
    {
        /* Lower elevations are all water */
        if (height < 0.1f) return Biome.WATER;
        else if (height < 0.15f) return Biome.COAST;
        else if (height < 0.3f)
        {
            /* Coastal / Lowlands */
            if (moisture < 0.3f) return Biome.GRASSLAND;
            else return Biome.SWAMP;
        }
        else if(height < 0.6f)
        {
            /* Normal Height */
            if (moisture < 0.3f) return Biome.DESERT;
            if (moisture < 0.6f) return Biome.GRASSLAND;
            else return Biome.FOREST;
        }
        else if(height < 0.8f)
        {
            /* Hills */
            if (moisture < 0.3f) return Biome.ROCKY;
            if (moisture < 0.5f) return Biome.SAVANNAH;
            if (moisture < 0.8f) return Biome.TUNDRA;
            else return Biome.SNOW;

        }
        else
        {
            /* Mountaintops */
            if (moisture < 0.3f) return Biome.ROCKY;
            else return Biome.SNOW;
        }
    }

    private List<Vector2Int> FindFlatSpots()
    {
        List<Vector2Int> spots = new List<Vector2Int>();

        for (int y = 1; y < Size.y - 1; y++)
        {
            for (int x = 1; x < Size.x - 1; x++)
            {
                int height = GetCell(x, y).height;
                bool isValid = true;
                for (int ky = -1; ky < 2 && isValid; ky++)
                {
                    for (int kx = -1; kx < 2 && isValid; kx++)
                    {
                        if (GetCell(x + kx, y + ky).height != height) isValid = false;
                    }
                }

                if (isValid) spots.Add(new Vector2Int(x, y));
                //if (isValid) GetCell(x, y).Color = Color.magenta;

            }
        }

        return spots;
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
