using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
    private Map map;


    public Pathfinder(Map map)
    {
        this.map = map;
    }

    public Path GetShortestPath(Cell source, Cell destination)
    {
        /* Step 1A: Allocate variables */
        Dictionary<Cell, PathfindCell> lookup = new Dictionary<Cell, PathfindCell>();   /* for easy conversions */
        List<PathfindCell> open = new List<PathfindCell>();                             /* keep sorted and treat as priority queue */
        List<PathfindCell> closed = new List<PathfindCell>();                           /* keep track of the nodes we finished searching */
        Path output = new Path();                                                       /* the output path we will return */

        /* Step 1B: Generate a lookup table for easy lookup between cells and pathfinder cells */
        foreach(var cell in map.data)
        {
            Vector2Int cellDelta = destination.Position - cell.Position;
            lookup.Add(cell, new PathfindCell() { cell = cell, heuristic = cellDelta.x * cellDelta.x + cellDelta.y * cellDelta.y, distance = int.MaxValue });
        }

        /* Step 2: Add the initial node to the open set */
        open.Add(lookup[source]);
        lookup[source].distance = 0;

        /* A* Algorithm - build up array of shortest distances */
        while(open.Count > 0)
        {
            /* Get the next cell */
            PathfindCell nextCell = open[0];
            open.Remove(nextCell);
            closed.Add(nextCell);
            int updatedLength = nextCell.distance + 1;

            /* If we grabbed the cell we were looking for, our work is done. */
            if (nextCell.cell == destination) break;

            /* If this cell's distance is INT_MAX, there is no path to our target */
            if (nextCell.distance == int.MaxValue) return null;

            /* Update Neighbors */
            foreach (Cell cell in map.GetNeighbors(nextCell.cell))
            {
                PathfindCell p = lookup[cell];
                if (closed.Contains(p)) continue;   //make sure to only update open nodes
                if (!open.Contains(p)) open.Add(p);  //add a new node to the open set
                if(p.distance > updatedLength)
                {
                    p.distance = updatedLength;
                    p.previous = nextCell;
                }
            }

            /* Restore the min heap property */
            open.Sort();
        }

        /* Generate a single path from source to destination */
        var next = lookup[destination];
        while(next.cell != source)
        {
            output.PushCellToEnd(next.cell);
            next = next.previous;
            
        }
        output.PushCellToEnd(next.cell); //add in the source

        /* Return the shortest path */
        return output;
    }


    private class PathfindCell : IComparable<PathfindCell>
    {
        public Cell cell;
        public int distance;
        public int heuristic;
        public PathfindCell previous;

        public int CompareTo(PathfindCell other)
        {
            /* A* Uses the F score (distance + heuristic) for sorting */
            return (distance + heuristic) - (other.distance + other.heuristic);
        }
    }

    /// <summary>
    /// A simple path implementation for backtracing found paths, then
    /// traversing them in a forwards direction.
    /// </summary>
    public class Path
    {
        public List<Cell> path;

        public Path()
        {
            path = new List<Cell>();
        }

        public void PushCellToEnd(Cell cell)
        {
            path.Insert(0, cell);
        }

        public Cell Traverse()
        {
            if(path[0] != null)
            {
                Cell next = path[0];
                path.RemoveAt(0);
                return next;
            }

            return null;
        }
    }
}
