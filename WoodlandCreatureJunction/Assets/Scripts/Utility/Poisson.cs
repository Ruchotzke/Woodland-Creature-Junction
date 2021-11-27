using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisson
{
    const int NUM_DISC_GENERATIONS = 30;

    Rect region;
    float radius;
    float cellSize;
    Vector2?[,] grid;
    Vector2Int gridSize;

    public Poisson(Rect region, float radius)
    {
        this.region = region;
        this.radius = radius;

        cellSize = radius / Mathf.Sqrt(2);
        gridSize = new Vector2Int(Mathf.CeilToInt(region.width / cellSize), Mathf.CeilToInt(region.height / cellSize));
        grid = new Vector2?[gridSize.x, gridSize.y];
    }

    public List<Vector2> SamplePoints()
    {
        return SamplePoints(new Vector2(Random.value * region.width + region.x, Random.value * region.height + region.y));
    }

    public List<Vector2> SamplePoints(Vector2 seed)
    {
        /* Initialization */
        List<Vector2> samples = new List<Vector2>();
        grid = new Vector2?[gridSize.x, gridSize.y];
        List<Vector2> openPoints = new List<Vector2>();

        /* Add the seed point and start the algorithm */
        Debug.Log("Adding seed");
        openPoints.Add(seed);
        samples.Add(seed);
        Vector2Int seedInt = ToGrid(seed);
        grid[seedInt.x, seedInt.y] = seed;

        /* Continue to run until we can't add any more points to the queue */
        while(openPoints.Count > 0)
        {
            /* Grab the next element */
            Debug.Log("Popping next point.");
            int nextIndex = Random.Range(0, openPoints.Count);
            Vector2 nextPoint = openPoints[nextIndex];
            openPoints.RemoveAt(nextIndex);

            /* Generate several points in a disc */
            for (int i = 0; i < NUM_DISC_GENERATIONS; i++)
            {
                //Debug.Log("Attempting secondary");
                /* Generate a new point */
                Vector2 discPoint = GeneratePoint(nextPoint, radius);

                /* Check 1: Is this point in-bounds */
                if (!region.Contains(discPoint)) continue;

                /* Check 2: is this point not near anything */
                if (IsInvalid(discPoint)) continue;

                /* Checks succeeded - this point is valid. */
                Debug.Log("Adding secondary");
                openPoints.Add(discPoint);
                samples.Add(discPoint);
                Vector2Int pointCoords = ToGrid(discPoint);
                grid[pointCoords.x, pointCoords.y] = discPoint;
            }
        }

        /* Return the final result */
        return samples;
    }

    private Vector2Int ToGrid(Vector2 point)
    {
        return new Vector2Int((int)(point.x / cellSize), (int)(point.y / cellSize));
    }

    private Vector2 GeneratePoint(Vector2 source, float minDist)
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * (Random.value + 1) * minDist;
    }

    private bool IsInvalid(Vector2 point)
    {
        Vector2Int gridPoint = ToGrid(point);
        for (int y = -2; y < 3; y++)
        {
            for (int x = -2; x < 3; x++)
            {
                Vector2Int newPos = gridPoint + new Vector2Int(x, y);
                if (newPos.x < 0 || newPos.y < 0 || newPos.x >= gridSize.x || newPos.y >= gridSize.y) continue;
                Vector2? other = grid[newPos.x, newPos.y];
                if(other != null)
                {
                    if (Vector2.Distance(other.Value, point) < radius)
                        return true;
                }
            }
        }

        return false;
    }
}
