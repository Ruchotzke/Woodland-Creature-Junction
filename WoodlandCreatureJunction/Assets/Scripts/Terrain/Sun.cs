using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    /// <summary>
    /// A float between 0 and 1, where 0 is morning, 0.5 is sunset, and 1/0 is morning
    /// </summary>
    public static float CurrentTime = 0.0f;

    //private GameObject light;
    [SerializeField]
    private float sunCycleTime = 30f;
    [SerializeField]
    private float radius = 5f;
    private float cTime;
    private new GameObject light;
    
    // Start is called before the first frame update
    void Start()
    {
        //light = transform.GetChild(0).gameObject;
        cTime = 0f;
        light = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        cTime += Time.deltaTime;
        cTime = cTime % sunCycleTime;
        CurrentTime = cTime / sunCycleTime;
        float x_pos = radius * Mathf.Sin(Mathf.PI * (2*cTime)/sunCycleTime);
        float y_pos = radius * Mathf.Cos(Mathf.PI * (2*cTime) / sunCycleTime);
        transform.localPosition = new Vector3(x_pos, y_pos, transform.localPosition.z);
        if (y_pos < 0)
        {
            light.SetActive(false);
        }
        else
        {
            light.SetActive(true);
        }
        light.transform.LookAt(transform.parent);
    }

}
