using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformCollide : MonoBehaviour
{
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "FallingPlatform")
            hit.transform.SendMessage("Collided", SendMessageOptions.RequireReceiver);
    }
}