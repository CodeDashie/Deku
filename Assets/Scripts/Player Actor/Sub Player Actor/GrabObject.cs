using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : MonoBehaviour
{
    private PlayerActor _pA;
    private Collider _curObject;
    private float _regrabTime = 0.1f;
    private float _regrabTimer;

    private bool _canDrop = false;
    private bool _isKinematic = false;
    
    public void SetValues(PlayerActor playerActor)
    {
        _pA = playerActor;
    }
    
    void Update()
    {
        if (_regrabTimer >= 0.0f)
            _regrabTimer -= Time.deltaTime;
        
        if (_pA.isHoldingObject)
        {
            // drop
            /*if (Input.GetButtonDown("Fire1") && _canDrop)
            {
                _regrabTimer = _regrabTime;
                _pA.state = PlayerActor.State.WALKING;

                float difference;
                if (_curObject.transform.localScale.x > _curObject.transform.localScale.z)
                    difference = _curObject.transform.localScale.x / 2.0f;
                else
                    difference = _curObject.transform.localScale.z / 2.0f;

                difference += _pA.controller.radius;
                float angle = transform.eulerAngles.y;

                Vector3 newPos = new Vector3(difference * Mathf.Sin(angle), 0.0f, difference * Mathf.Cos(angle));
                _curObject.transform.position += newPos;

                // reset variables
                _curObject.transform.parent = null;
                _curObject.attachedRigidbody.isKinematic = _isKinematic;
                _curObject = null;
                _pA.isHoldingObject = false;  
            }*/
            // throw
            //else if (Input.GetButtonDown("Fire2"))
            if (Input.GetButtonDown("Fire1") && _canDrop)
            {
                Rigidbody rB = _curObject.attachedRigidbody;



                _curObject.transform.parent = null;
                rB.isKinematic = _isKinematic;

                rB.AddRelativeForce(new Vector3 (0, 1, 1) * _pA.throwPower);

                _curObject = null;
                _pA.isHoldingObject = false;
            }
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetButtonDown("Fire1"))
            if (_regrabTimer <= 0.0f && other.tag == "Object" && _pA.stateIndex != PlayerActor.StateIndex.ON_LEDGE && !_pA.isHoldingObject)
            {
                _canDrop = false;
                _isKinematic = other.attachedRigidbody.isKinematic;
                if (!_isKinematic)
                    other.attachedRigidbody.isKinematic = true;
                
                _pA.isHoldingObject = true;
                _curObject = other;

                _curObject.transform.position = new Vector3(transform.position.x, transform.position.y + (_pA.controller.height / 2.0f) + (_curObject.transform.localScale.y / 2.0f), transform.position.z);
                _curObject.transform.rotation = transform.rotation;
                _curObject.transform.parent = transform;
            }

        if (Input.GetButtonUp("Fire1"))
            _canDrop = true;
    }
}
