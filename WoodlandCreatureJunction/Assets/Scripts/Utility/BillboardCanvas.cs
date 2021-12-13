using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// A simple script used to keep a world space
/// canvas facing the player camera.
/// </summary>
public class BillboardCanvas : MonoBehaviour
{
    Camera playerCam;
    float elapsedTime = 0.0f;

    [Header("Components")]
    public TextMeshProUGUI Text;
    public GameObject CanvasContainer;
    public float MessageTimeout = 3.0f;

    private void Start()
    {
        playerCam = Camera.main;
        CanvasContainer.SetActive(false);
    }

    private void Update()
    {
        /* Rotate to always face the player */
        transform.rotation = Quaternion.LookRotation((transform.position - playerCam.transform.position).normalized);

        /* If we are displaying text, possibly stop */
        if (CanvasContainer.activeSelf)
        {
            elapsedTime += Time.deltaTime;
            if(elapsedTime > MessageTimeout)
            {
                elapsedTime = 0.0f;
                CanvasContainer.SetActive(false);
            }
        }
    }

    public void DisplayMessage(string text)
    {
        /* Set up the message */
        Text.text = text;

        /* Display */
        CanvasContainer.SetActive(true);
    }
}
