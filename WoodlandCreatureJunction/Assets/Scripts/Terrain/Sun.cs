using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    //private GameObject light;
    [SerializeField]
    private float sunCycleTime = 30f;
    [SerializeField]
    private float radius = 5f;
    private float cTime;
    private GameObject light;
    
    // Start is called before the first frame update
    void Start()
    {
        //light = transform.GetChild(0).gameObject;
        cTime = 0f;
        light = GameObject.Find("Sunlight");
    }

    // Update is called once per frame
    void Update()
    {
        cTime += Time.deltaTime;
        cTime = cTime % 30f;
        float x_pos = radius * Mathf.Sin(Mathf.PI * (2*cTime)/sunCycleTime);
        float y_pos = radius * Mathf.Cos(Mathf.PI * (2*cTime) / sunCycleTime);
        Debug.Log(x_pos);
        transform.localPosition = new Vector3(x_pos, y_pos, transform.localPosition.z);
        if(y_pos < 0)
        {
            light.SetActive(false);
        }
        else
        {
            light.SetActive(true);
        }
        transform.LookAt(GetComponentInParent<Transform>());
        // transform.RotateAround(new Vector3(1f, 0f, 0f), sunCycleTime * Time.deltaTime);
    }

}
