using UnityEngine;

public class Noise
{
    public static float GenerateNoise(Vector2 pos, Vector2 frequency, Vector2 offset)
    {
        return Mathf.PerlinNoise(pos.x * frequency.x + offset.x, pos.y * frequency.y + offset.y);
    }

    public static float GenerateNoise(Vector2 pos)
    {
        return Mathf.PerlinNoise(pos.x, pos.y);
    }

    public static float GenerateNoise(Vector2 pos, Vector2 frequency)
    {
        return Mathf.PerlinNoise(pos.x * frequency.x, pos.y * frequency.y);
    }
}