
using UnityEngine;

public enum Biome
{
    WATER,
    COAST,
    GRASSLAND,
    SWAMP,
    FOREST,
    JUNGLE,
    SAVANNAH,
    DESERT,
    SNOW,
    TUNDRA,
    ROCKY
}

public static class BiomeExtensions
{
    public static Color GetColor(this Biome biome)
    {
        switch (biome)
        {
            case Biome.WATER:
                return new Color32(15, 6, 145, 255);
            case Biome.COAST:
                return new Color32(73, 92, 201, 255);
            case Biome.GRASSLAND:
                return new Color32(17, 122, 57, 255);
            case Biome.SWAMP:
                return new Color32(4, 64, 27, 255);
            case Biome.FOREST:
                return new Color32(49, 145, 54, 255);
            case Biome.JUNGLE:
                return new Color32(46, 92, 28, 255);
            case Biome.SAVANNAH:
                return new Color32(171, 166, 118, 255);
            case Biome.DESERT:
                return new Color32(213, 217, 102, 255);
            case Biome.SNOW:
                return new Color32(255, 255, 255, 255);
            case Biome.TUNDRA:
                return new Color32(179, 201, 201, 255);
            case Biome.ROCKY:
                return new Color32(128, 128, 128, 255);
            default:
                return Color.magenta;
        }
    }
}