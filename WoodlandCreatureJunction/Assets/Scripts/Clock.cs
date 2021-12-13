using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] RectTransform ClockFace;

    // Update is called once per frame
    void Update()
    {
        ClockFace.rotation = Quaternion.Euler(0.0f, 0.0f, 720.0f * Sun.CurrentTime);
    }
}
