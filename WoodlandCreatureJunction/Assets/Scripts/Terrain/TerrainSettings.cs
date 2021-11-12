using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSettings
{
    /// <summary>
    /// The amount of worldspace distance for a unit of height.
    /// </summary>
    public const float UnitHeight = 0.50f;

    /// <summary>
    /// The amount of worldspace distance for a unit of xz direction.
    /// </summary>
    public const float UnitSize = 1.0f;

    /// <summary>
    /// The percentage of a cell's face which is flat (not bridged).
    /// </summary>
    public const float CellMargin = 0.7f;

    /// <summary>
    /// The distance offset from the center of a face to the bridges.
    /// </summary>
    public const float CellFaceOffset = CellMargin * UnitSize / 2;

    /// <summary>
    /// How many flat terraces are present on a single slope.
    /// </summary>
    public const int NumTerraces = 4;

    /// <summary>
    /// How many interpolation points are present on a single slope.
    /// </summary>
    public const int TerraceInterp = NumTerraces * 2 + 1;

    /// <summary>
    /// How much does a single terrace interpolation move horizontally.
    /// </summary>
    public const float HorizontalTerraceStep = 1f / TerraceInterp;

    /// <summary>
    /// How much does a single terrace interpolation move vertically.
    /// </summary>
    public const float VerticalTerraceStep = 1f / (NumTerraces + 1);

    /// <summary>
    /// The chunk size for the generated mesh.
    /// </summary>
    public const int ChunkSize = 20;
}
