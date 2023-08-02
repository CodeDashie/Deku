using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public class Object
    {
        public Collider _object;
        
        public bool _isIdleColliding = true;
        public bool _isCollisionExited = false;
        public Vector3 _lastPosition;

        public Object(Collider o)
        {
            _object = o;
        }
    }

    // colliding objects list
    private List<Object> _colObject;

    private PlayerActor _pA;

    private Collider _curObject;
    private Side _curSide;
    private Vector2 _objectOffset;
    private Vector2 _characterOffset;
    private float _regrabTime = 0.1f;
    private float _regrabTimer;
    private bool _canDrop = false;
    private bool _isKinematic = false;

    enum Side
    {
        EAST,
        NORTH,
        WEST,
        SOUTH
    };

    private const float NORTH_EAST = 45.0f;
    private const float SOUTH_EAST = 315.0f;

    private const float NORTH_WEST = 135.0f;
    private const float SOUTH_WEST = 225.0f;
    
    public void SetValues(PlayerActor playerActor)
    {
        _pA = playerActor;
        _colObject = new List<Object>();
    }

    void ResetAllTriggers()
    {
        foreach (AnimatorControllerParameter parameter in _pA.anim.parameters)
            _pA.anim.ResetTrigger(parameter.name);
    }

    // this script pushes all rigidbodies that the character touches
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        OnCharacterCollideObject(hit);
    }

    void OnCharacterCollideObject(ControllerColliderHit hit)
    {
        if (_pA.stateIndex == PlayerActor.StateIndex.WALKING && hit.transform.tag == "ObjectPushable")
        {
            foreach (Object o in _colObject)
                if (hit.collider == o._object)
                {
                    o._isIdleColliding = true;
                    o._isCollisionExited = false;
                    return;
                }
            
            _colObject.Add(new Object(hit.collider));
            
        }

    }

    private void FixedUpdate()
    {
        PushPullInput();
        GrabInput();
        SetGravity();
    }

    private float AngleBetween(Vector2 center, Vector2 other)
    {
        return (180 / Mathf.PI) * (Mathf.PI - Mathf.Atan2(other.y - center.y, other.x - center.x));

        //return 0.0f;
    }

    private void SetSide()
    {
        Vector2 center = new Vector2(_curObject.transform.position.x, _curObject.transform.position.z);
        Vector2 other = new Vector2(transform.position.x, transform.position.z);

        float angle = AngleBetween(center, other);

        float facing = 0.0f;

        //Debug.Log(AngleBetween(center, other));

        if (angle >= NORTH_EAST && angle <= NORTH_WEST)
        {
            _curSide = Side.NORTH;
            facing = 180.0f;
        }

        else if (angle >= SOUTH_WEST && angle <= SOUTH_EAST)
        {
            _curSide = Side.SOUTH;
            facing = 0.0f;
        }


        else if (angle >= NORTH_WEST && angle <= SOUTH_WEST)
        {
            _curSide = Side.WEST;
            facing = 270.0f;
        }

        else if (angle <= NORTH_EAST || angle >= SOUTH_EAST)
        {
            _curSide = Side.EAST;
            facing = 90.0f;
        }

        _pA.transform.rotation = Quaternion.Euler(_pA.transform.eulerAngles.x, facing, _pA.transform.eulerAngles.z);
    }

    void GrabInput()
    {
        if (_pA.stateIndex == PlayerActor.StateIndex.WALKING)
        {
            // setup for object list check removal
            List<Object> colObjectRemove = new List<Object>();

            // for each object that is currently being collided with
            foreach (Object o in _colObject)
            {
                // is idle but moved position in relation to last frame the character is no
                // longer idle and colliding with the object and the object is removed
                // from the list of objects that the character is colliding with
                if (o._isCollisionExited && o._lastPosition != _pA.transform.position)
                {
                    // setup for object check list removal and go to next in foreach
                    colObjectRemove.Add(o);
                    continue;
                }
                // if colliding, if no longer colliding but has not moved and is not on exit frame
                if (!o._isIdleColliding && !o._isCollisionExited)
                {
                    // on stop move position

                    // this should be the same position as the next frame, else it will check if it's colliding
                    // with the OnControllerColliderHit method else it's not considered colliding
                    o._lastPosition = _pA.transform.position;
                    // setup for next frame to check if it is the same position as this one
                    o._isCollisionExited = true;
                }

                // if colliding and press button for push and pull state
                if (Input.GetButtonDown("Fire1"))
                {
                    // is now in pushing object state
                    _pA.stateIndex = PlayerActor.StateIndex.PUSHING_OBJECT;
                    // reset animation and set animation to grab
                    ResetAllTriggers();
                    _pA.anim.SetTrigger("isGrabIdle");
                    // setup currently grabbed object
                    _curObject = o._object;
                    SetSide();
                    // make sure the offset is the same as the initial when moving the object
                    _objectOffset = new Vector2(_pA.transform.position.x - _curObject.transform.position.x, _pA.transform.position.z - _curObject.transform.position.z);
                    _characterOffset = new Vector2(_curObject.transform.position.x - _pA.transform.position.x, _curObject.transform.position.z - _pA.transform.position.z);
                    // clear colliding object list
                    _colObject.Clear();
                    // exit method
                    return;
                }

                // if idle will be set back to true if the character is pushing on an object
                // and false if it's idle or moving and no longer colliding with an object
                o._isIdleColliding = false;
            }

            // remove objects from the list that are no longer being collided with
            foreach (Object o in colObjectRemove)
                _colObject.Remove(o);
            colObjectRemove.Clear();
        }
    }

    Vector3 _lastPos;

    void PushPullInput()
    {
        if (_pA.stateIndex == PlayerActor.StateIndex.PUSHING_OBJECT)
        {
            // if colliding and press button for push and pull state
            float yDif = _pA.transform.position.y - _curObject.transform.position.y;
            // get button || is object below feet || is object above head
            if (Input.GetButtonDown("Fire1") || (yDif > _curObject.transform.localScale.y || -yDif > _pA.controller.height))
            {
                // is now in walking state
                _pA.stateIndex = PlayerActor.StateIndex.WALKING;
                ResetAllTriggers();
                _pA.anim.SetTrigger("isIdling");
                _curObject.attachedRigidbody.velocity = new Vector3();
                _pA.controller.Move(new Vector3());
                // exit method
                return;
            }

            // desired velocity
            Vector2 v = new Vector2
            (
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            );
            
            Rigidbody body = _curObject.attachedRigidbody;

            // calculate push direction from move direction,
            // we only push objects to the sides never up and down
            Vector3 pushDir = new Vector3();// = new Vector3(v.y, 0, 0);// v.y);
            if (_curSide == Side.EAST)
                pushDir = new Vector3(v.y, 0, 0);// v.y);
            else if (_curSide == Side.WEST)
                pushDir = new Vector3(-v.y, 0, 0);// v.y);

            else if (_curSide == Side.NORTH)
                pushDir = new Vector3(0, 0, -v.y);// v.y);
            else if (_curSide == Side.SOUTH)
                pushDir = new Vector3(0, 0, v.y);// v.y);

            if (v.y > 0.0f)
            {
                ResetAllTriggers();
                _pA.anim.SetTrigger("isGrabPush");
                body.velocity = pushDir * 5.0f;

                Vector2 objectOffset = new Vector2(_pA.transform.position.x - _curObject.transform.position.x, _pA.transform.position.z - _curObject.transform.position.z);

                _pA.controller.Move(new Vector3(_objectOffset.x - objectOffset.x, 0.0f, _objectOffset.y - objectOffset.y));

                if (_lastPos != body.position)
                    _pA.controller.Move(new Vector3(v.y * 5.0f, 0.0f, 0.0f) * Time.deltaTime);
                _lastPos = body.position;
            }
            else if (v.y < 0.0f)
            {

                ResetAllTriggers();
                _pA.anim.SetTrigger("isGrabPull");
                Vector2 objectOffset = new Vector2(_pA.transform.position.x - _curObject.transform.position.x, _pA.transform.position.z - _curObject.transform.position.z);

                _pA.controller.Move(new Vector3(_objectOffset.x - objectOffset.x, 0.0f, _objectOffset.y - objectOffset.y));

                if (_lastPos != body.position)
                    _pA.controller.Move(new Vector3(v.y * 5.0f, 0.0f, 0.0f) * Time.deltaTime);
                _lastPos = body.position;
                body.velocity = pushDir * 5.0f;

            }
            else
            {
                ResetAllTriggers();
                _pA.anim.SetTrigger("isGrabIdle");
                body.velocity = new Vector3();
            }
            
        }
    }
    
    void SetGravity()
    {
        if (_pA.stateIndex == PlayerActor.StateIndex.PUSHING_OBJECT)
        {
            // gravity
            if (!_pA.controller.isGrounded)
                _pA.fallVelocity += _pA.gravity;
            // landed
            else
                _pA.fallVelocity = _pA.groundedGravity;

            // move to new position
            _pA.controller.Move(new Vector3(0, _pA.fallVelocity, 0) * Time.deltaTime);
        }
    }
}
