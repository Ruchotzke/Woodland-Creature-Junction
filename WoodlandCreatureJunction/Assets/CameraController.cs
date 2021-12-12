using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple script used to control the camera's positioning and rotation.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Conversation Parameters")]
    public float TransitionTime = 2.0f;

    Player player;

    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 targetPosition;
    Quaternion targetRotation;
    bool inConversation = false;
    float currTransitionTimer = 0.0f;
    bool transitioning = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (inConversation)
        {
            /* Slerp and lerp towards the target camera position */
            if (transitioning)
            {
                currTransitionTimer += Time.deltaTime;
                if (currTransitionTimer >= TransitionTime)
                {
                    transitioning = false;
                    currTransitionTimer = TransitionTime;
                }
                Camera.main.transform.position = Vector3.Lerp(originalPosition, targetPosition, currTransitionTimer / TransitionTime);
                Camera.main.transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, currTransitionTimer / TransitionTime);
            }
        }
        else
        {
            /* Slerp and lerp towards the original camera position */
            if (transitioning)
            {
                currTransitionTimer -= Time.deltaTime;
                if (currTransitionTimer <= 0.0f)
                {
                    transitioning = false;
                    currTransitionTimer = 0.0f;
                }
                Camera.main.transform.position = Vector3.Lerp(originalPosition, targetPosition, currTransitionTimer / TransitionTime);
                Camera.main.transform.rotation = Quaternion.Slerp(originalRotation, targetRotation, currTransitionTimer / TransitionTime);
            }
        }
    }

    public void StartConversation(Transform villager)
    {
        /* Initialize */
        inConversation = true;
        originalPosition = Camera.main.transform.position;
        originalRotation = Camera.main.transform.rotation;
        transitioning = true;

        /* Calculate target */
        targetPosition = ((player.transform.position - villager.position) * 1.2f) + villager.position; //directly behind the player
        targetPosition += 4.0f * villager.transform.right;
        targetPosition += villager.transform.up;   //raise the camera up above the conversation a bit
        targetRotation = Quaternion.LookRotation((villager.transform.position - targetPosition).normalized); //look about halfway between them

    }

    public void EndConversation()
    {
        /* Reset */
        inConversation = false;
        transitioning = true;
    }
}
