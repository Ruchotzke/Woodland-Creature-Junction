using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSettings
{
    /// <summary>
    /// The amount of worldspace distance for a unit of height.
    /// </summary>
    public const float UnitHeight = 0.25f;

    /// <summary>
    /// The amount of worldspace distance for a unit of xz direction.
    /// </summary>
    public const float UnitSize = 1.0f;

    /// <summary>
    /// The percentage of a cell's face which is flat (not bridged).
    /// </summary>
    public const float CellMargin = 0.85f;

    /// <summary>
    /// The distance offset from the center of a face to the bridges.
    /// </summary>
    public const float CellFaceOffset = CellMargin * UnitSize / 2;
}
