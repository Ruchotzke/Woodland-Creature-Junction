using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoiseDebug : MonoBehaviour
{
    RawImage image;
    Texture2D tex;

    

    private void Start()
    {
        image = GetComponent<RawImage>();
        tex = new Texture2D(300, 300);
        image.texture = tex;

        UpdateTexture();
    }

    void UpdateTexture()
    {
        for (int y = 0; y < 300; y++)
        {
            for (int x = 0; x < 300; x++)
            {
                float val = Mathf.PerlinNoise(x / 300.0f, y / 300.0f);
                tex.SetPixel(x, y, new Color(val, val, val, 1));
            }
        }
        tex.Apply();
    }

    private void OnValidate()
    {
        if(tex != null)
        {
            UpdateTexture();
        }
        
    }
}
