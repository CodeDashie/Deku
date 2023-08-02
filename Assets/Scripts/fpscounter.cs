using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpscounter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    float timer = 0;
    int frameRate;

    // Update is called once per frame
    void Update()
    {
        if (timer < 0.5f)
        {
            timer += Time.deltaTime;
            frameRate += 1;
        }
        else
        {
            Debug.Log("FPS: " + frameRate.ToString());
            timer = 0;
            frameRate = 0;
        }
    }
}