using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private PlayerActor _pA;
    
    public void SetValues(PlayerActor playerActor)
    {
        this._pA = playerActor;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log(hit.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButton("Fire1") && _pA.stateIndex == PlayerActor.StateIndex.WALKING && other.tag == "Button")
        {
            ButtonTrigger bT = other.gameObject.GetComponent<ButtonTrigger>();
            if (bT != null)
                bT.OnPress();
        }
    }
}
