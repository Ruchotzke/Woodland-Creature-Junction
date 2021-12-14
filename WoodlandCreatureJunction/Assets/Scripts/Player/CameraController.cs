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

    [Header("Control Parameters")]
    public float sensitivity = 1.0f;

    Player player;
    Transform pivot;

    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 targetPosition;
    Quaternion targetRotation;
    bool inConversation = false;
    float currTransitionTimer = 0.0f;
    public static bool transitioning = false;

    private void Awake()
    {
        pivot = transform.parent;
        player = GameObject.FindObjectOfType<Player>();
    }

    private void Update()
    {

        /* Handle Conversation Cutaways */
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
            //else
            //{
            //    /* We arent in a conversation and we aren't transitioning. Free Movement. */
            //    if (Cursor.lockState != CursorLockMode.Locked) Cursor.lockState = CursorLockMode.Locked;
            //    Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            //    pivot.localRotation = Quaternion.Euler(pivot.localRotation.x + mouseMovement.y * sensitivity * Time.deltaTime, pivot.localRotation.y, pivot.localRotation.z);
            //    player.transform.rotation = Quaternion.Euler(player.transform.rotation.x, player.transform.rotation.y + mouseMovement.x * sensitivity * Time.deltaTime, player.transform.rotation.z);
            //}
        }
    }

    public void StartConversation(Transform villager)
    {
        /* Initialize */
        inConversation = true;
        originalPosition = Camera.main.transform.position;
        originalRotation = Camera.main.transform.rotation;
        transitioning = true;
        Cursor.lockState = CursorLockMode.None;

        /* Calculate target */
        targetPosition = ((player.transform.position - villager.position) * 1.2f) + villager.position; //directly behind the player
        targetPosition += 4.0f * villager.transform.right;
        targetPosition += 2.0f * villager.transform.up;   //raise the camera up above the conversation a bit
        targetRotation = Quaternion.LookRotation((villager.transform.position - targetPosition).normalized); //look about halfway between them

    }

    public void EndConversation()
    {
        /* Reset */
        inConversation = false;
        transitioning = true;
    }
}
