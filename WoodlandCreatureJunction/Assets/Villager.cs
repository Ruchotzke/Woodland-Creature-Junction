using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public AnimationController animationController;
    public Pathfinder pathfinder;
    public TerrainMesh terrainMesh;
    public Transform home;

    public float MoveSpeed = 1.0f;

    VillagerAI AIState = VillagerAI.STANDING;
    bool Moving = false;
    float elapsedStateTime = 0.0f;

    Cell target;
    Pathfinder.Path currPath = null;
    Vector3[] currPos = new Vector3[2]; //0 = source, 1 = target

    private void Start()
    {
        /* Generate important components */
        animationController = GetComponent<AnimationController>();
        terrainMesh = FindObjectOfType<TerrainMesh>();
        pathfinder = new Pathfinder(terrainMesh.map);

        /* Testing: Choose a random place and walk there */
        var nearby = Nearby(20);
        Vector2Int pos = nearby[Random.Range(0, nearby.Count)];
        target = terrainMesh.map.GetCell(pos.x, pos.y);
        StartPathfinding(target);
    }

    private void Update()
    {
        /* On any frame we move, we need to update our height to match the terrain */
        if (AIState == VillagerAI.WANDERING)
        {
            /* Move forward on the lerp route, checking if we made it to our destination */
            Vector3 direction = (currPos[1] - currPos[0]).normalized;
            if(Vector3.Distance(transform.position, currPos[1]) > MoveSpeed * Time.deltaTime)
            {
                /* We can move towards, but not arrive */
                transform.position += direction * MoveSpeed * Time.deltaTime;
            }
            else
            {
                /* We would overshoot if we moved. Stop at the target and update state */
                transform.position = currPos[1];

                /* Update state */
                if(currPath.path.Count > 0)
                {
                    /* There is still another step. Move again */
                    currPos[0] = currPos[1];
                    currPos[1] = terrainMesh.CellToWorld(currPath.Traverse());
                }
                else
                {
                    /* We reached the end of our path and arrived. End the pathfinding */
                    Moving = false;
                    AIState = VillagerAI.STANDING;
                }
            }

            /* We should also face our target */
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(direction.x, 0.0f, direction.z).normalized), 150f * Time.deltaTime);
        }

        /* Update the animation state */
        switch (AIState)
        {
            case VillagerAI.STANDING:
                animationController.CurrentAnimation = AnimationController.Animation.IDLE;
                break;
            case VillagerAI.WANDERING:
                animationController.CurrentAnimation = AnimationController.Animation.MOVING;
                break;
            case VillagerAI.RETURNING_HOME:
                animationController.CurrentAnimation = AnimationController.Animation.MOVING;
                break;
            default:
                break;
        }
        
    }

    void StartPathfinding(Cell endGoal)
    {
        /* Get the shortest path */
        target = endGoal;
        currPath = pathfinder.GetShortestPath(terrainMesh.WorldToCell(transform.position), target);

        /* Set up the first pathfinding step */
        currPos[0] = terrainMesh.CellToWorld(currPath.Traverse());
        currPos[1] = terrainMesh.CellToWorld(currPath.Traverse());

        Debug.Log(string.Join(", ", currPos));

        /* Update the state */
        Moving = true;
        AIState = VillagerAI.WANDERING;
    }

    public List<Vector2Int> Nearby(int range)
    {
        List<Vector2Int> nearby = new List<Vector2Int>();
        Cell curr = terrainMesh.WorldToCell(transform.position);
        for(int y = -range / 2; y <= range / 2; y++)
        {
            for (int x = -range / 2; x <= range / 2; x++)
            {
                if (x == 0 && y == 0) continue;
                Vector2Int target = curr.Position + new Vector2Int(x, y);
                if (target.x >= 0 && target.y >= 0 && target.x < terrainMesh.map.Size.x && target.y < terrainMesh.map.Size.y)
                {
                    nearby.Add(target);
                }
            }
        }

        return nearby;
    }

    public enum VillagerAI
    {
        STANDING,
        WANDERING,
        RETURNING_HOME
    }
}
