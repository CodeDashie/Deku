using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLocator : MonoBehaviour
{
    const float PI = 3.14159265358979f;
    const float Deg2Rad = PI / 180.0f;

    void OnDrawGizmos()
    {
        
        GameObject o = transform.parent.gameObject;

        float r = (o.transform.eulerAngles.y - 90.0f) * Deg2Rad;
        float s = transform.localScale.x / 2.0f;
        //float s = 1.0f;

        Vector3 v = new Vector3(s * Mathf.Sin(r), 0.0f, s * Mathf.Cos(r));

        Vector3 p = transform.position;

        Gizmos.DrawLine(p - v, p + v);
        //Gizmos.DrawSphere(transform.position, 0.5F);
    }
}
