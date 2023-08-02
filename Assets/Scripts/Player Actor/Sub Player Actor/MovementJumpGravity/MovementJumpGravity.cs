using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class MovementJumpGravity : State
{
    float mag = 0.0f;
    bool isCrouch = false;
    bool isStrafing = true;
    bool isIdle = true;
    bool isWalk = false;
    bool isRun = false;

    int idleFrame = 0;
    int walkFrame = 0;

    int jumpLockFrame = 0;
    float jumpLockMag = 0.0f;
    Vector2 jumpLockSpeedV;

    int MoveXAniParaId;
    int MoveZAniParaId;

    public override void SetValues(PlayerActor playerActor)
    {
        this._pA = playerActor;
        _pA.anim.speed = 3.5f;
        MoveXAniParaId = Animator.StringToHash("MoveX");
        MoveZAniParaId = Animator.StringToHash("MoveZ");
        
        _pA.HandRig.weight = 0.0f;
        _pA.AngleRig.weight = 0.0f;
        _pA.Shield.SetActive(false);
        isCrouch = false;
    }

    public override void Activate()
    {
        Debug.Log("Activate Move");
        _pA.stateIndex = PlayerActor.StateIndex.WALKING;
        isActive = true;
        setAnimParam(0.0f);
        _pA.anim.speed = 3.5f;
        if (_pA.controller.isGrounded)
            Transition(false, "Strafe");
        else
            Transition(true, "Air");
    }

    public override void Deactivate()
    {
        isActive = false;

        _pA.HandRig.weight = 0.0f;
        _pA.AngleRig.weight = 0.0f;
        _pA.Shield.SetActive(false);
        _pA.ShieldBack.gameObject.SetActive(true);
        //isCrouch = false;
        //_pA.isWalking = false;
        //setAnimationSpeed(0.0f);
        _pA.anim.speed = 0.0f;
    }

    void FixedUpdate()
    {
        if (isActive)
            movement();
    }

    private void Update()
    {
        if (isActive)
            setJumpGravity();
    }

    void movement()
    {
        if (Gamepad.current == null)
            ;// return;
        if (isCrouch)
        {
            setShieldAngle();
            return;
        }

        // desired velocity
        Vector2 v = magAndVector();

        // gravity and jump velocity
        if (!_pA.controller.isGrounded)
            _pA.fallVelocity += _pA.gravity;

        // transition frame delay
        frameDelay(v);

        // facing, move and animation if ordered to
        if (!(v.x == 0.0f && v.y == 0.0f))
            fixFacingAngleToCamera(ref v);

        // animation speed
        if (isRun) setAnimParam(_pA.runAnimParam);
        else if (isWalk)
        {
            float param;
            if (walkFrame == -1)
                param = _pA.walkAnimParam;
            else
            // base + (param difference * frame percent/100)
            {
                float frameDiv = (float)walkFrame / _pA.walkFrames;
                float frameD = (frameDiv) * (-1.0f) + 1.0f;
                param = _pA.walkAnimParam + ((_pA.runAnimParam - _pA.walkAnimParam) * frameD);
                //Debug.Log(param);
            }

            setAnimParam(param);
        }
        else
        {
            //Debug.Log("testtt " + walkFrame);
            float param;
            if (idleFrame == -1)
                param = _pA.idleAnimParam;
            else
            {
                // base + (param difference * frame percent/100)


                float frameDiv = (float)idleFrame / _pA.idleFrames;
                float frameD = (frameDiv) * (-1.0f) + 1.0f;
                param = _pA.idleAnimParam + ((_pA.walkAnimParam - _pA.idleAnimParam) * frameD);
                //param = 0.0f + (_pA.walkAnimParam - _pA.idleAnimParam * ((((float) idleFrame / _pA.idleFrames) * (-1f)) + 1f));
            }

            setAnimParam(param);
        }

        // move to new position
        _pA.controller.Move(new Vector3(v.x, _pA.fallVelocity, v.y) * Time.deltaTime);
    }
    
    Vector2 magAndVector()
    {
        Vector2 v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // animation speed based on magnitute of input axis

        if (isStrafing)
        {
            mag = new Vector2(v.x, v.y).magnitude;
            if (mag > 1.0f) mag = 1.0f;
        }
        else
        {
            // Clamp lock airspeed
            float adjust = (1.0f - new Vector2(v.x, v.y).magnitude);
            if (adjust > 1.0f) adjust = 1.0f;
            adjust *= _pA.airSpeedClamp;
            mag = jumpLockMag - adjust;
            if (mag > 1.0f) mag = 1.0f;
            if (mag < 0.0f) mag = 0.0f;

            float angle;
            if (v.x != 0.0f || v.y != 0.0f)
                angle = Mathf.Atan2(v.y, v.x);
            else
                angle = _pA.transform.eulerAngles.y;
            v.x = 1 * Mathf.Cos(angle);
            v.y = 1 * Mathf.Sin(angle);
        }

        float tempX = v.x;
        v.x = v.x * Mathf.Sqrt(1 - 0.5f * (v.y * v.y));
        v.y = v.y * Mathf.Sqrt(1 - 0.5f * (tempX * tempX));

        return v;
    }

    void frameDelay(Vector2 v)
    {
        if (isStrafing)
        {
            /*if (mag < _pA.idleMag)
            {
                isIdle = false;
                isWalk = false;
                isRun = false;
                idleFrame = -1;
                walkFrame = -1;
            }
            else*/ if (mag < _pA.walkMag)
            {
                //Debug.Log(v.x + ", " + v.y);
                isIdle = true;
                isWalk = false;
                isRun = false;
                idleFrame = -1;
                walkFrame = -1;
            }
            else if (mag < _pA.runMag || isIdle)
            {
                //Debug.Log("Idle " + idleFrame);
                isWalk = false;
                if (idleFrame == -1)
                {
                    isIdle = true;
                    idleFrame = _pA.idleFrames;
                }
                else if (idleFrame == 0)
                {
                    isIdle = false;
                    isWalk = true;
                }
                else
                {
                    isIdle = true;
                    idleFrame--;
                }

                isRun = false;
                walkFrame = -1;
            }
            else if (isWalk)
            {
                if (walkFrame == -1)
                    walkFrame = _pA.walkFrames;
                else if (walkFrame == 0)
                {
                    isWalk = false;
                    isRun = true;
                }
                else
                    walkFrame--;
                idleFrame = -1;
            }
        }
        else if (false)
        {
            isIdle = false;
            isWalk = false;
            isRun = false;
            idleFrame = -1;
            walkFrame = -1;
            if (mag < _pA.walkMag)
                isIdle = true;
            else if (mag < _pA.runMag)
            {
                isWalk = true;
                idleFrame = 0;
            }
            else if (isWalk)
            {
                isRun = true;
                walkFrame = 0;
            }
        }
        if (jumpLockFrame > 0)
            jumpLockFrame--;
    }

    void fixFacingAngleToCamera(ref Vector2 v)
    {
        float speed = 0.0f;
        float newFacing;

        newFacing = (Maths.Rad2Deg * Mathf.Atan2(v.x, v.y)) +
                  _pA.camera.transform.eulerAngles.y;
        if (isStrafing)
        {
            if      (isRun)  speed = _pA.runSpeed;
            else if (isWalk) speed = _pA.walkSpeed;
            else             speed = 0.0f;
        }
        else
            speed = _pA.airSpeed * mag;

        // facing from desired direction with camera angle in mind

        // set new facing
        float currFacingAngle;

        currFacingAngle = setFacing(newFacing, v);
        
        // new walk position from newFacing
        v.x = speed * Mathf.Sin(currFacingAngle);
        v.y = speed * Mathf.Cos(currFacingAngle);
    }

    float setFacing(float newFacing, Vector2 v)
    {
        float curFacing = _pA.transform.eulerAngles.y;

        if (mag >= 0.1f)
        {
            if (newFacing > 360.0f) newFacing -= 360.0f;
            else if (newFacing < 0.0f) newFacing += 360.0f;

            // is there a difference between current frame facing and next frame ordered facing?
            if (curFacing != newFacing)
            {
                float velocity;
                if (isStrafing)
                {
                    if (isRun)
                        velocity = _pA.turnSpeed * Time.deltaTime;
                    else if (false)//(mag < _pA.idleMag)
                        return curFacing * Maths.Deg2Rad;
                    else
                        velocity = 9000.0f * Time.deltaTime;
                }
                else
                    velocity = _pA.turnSpeedAir * Time.deltaTime;

                // check if velocity is less than the difference
                if ((curFacing <= newFacing && curFacing + velocity >= newFacing) ||
                    (curFacing >= newFacing && curFacing - velocity <= newFacing))
                    curFacing = newFacing;

                else if (newFacing < 180)
                {
                    if (curFacing > newFacing && curFacing < newFacing + 180)
                        curFacing -= velocity;
                    else
                        curFacing += velocity;
                }
                else
                {
                    if ((curFacing > newFacing && curFacing < newFacing + 180) || (curFacing + 360 > newFacing && curFacing < newFacing - 180))
                        curFacing -= velocity;
                    else
                        curFacing += velocity;
                }

                if (curFacing > 360.0f) curFacing -= 360.0f;
                else if (curFacing < 0.0f) curFacing += 360.0f;
                //if ((int)_pA.transform.eulerAngles.y != (int)curFacing)
                _pA.transform.rotation = Quaternion.Euler(_pA.transform.eulerAngles.x, curFacing, _pA.transform.eulerAngles.z);
                //if (!isRun)
                    //Debug.Log(curFacing + ", " + mag + ", " + (Maths.Rad2Deg * Mathf.Atan2(v.y, v.x)) + ", " + v.x + " + " + v.y);
            }
        }

        return curFacing * Maths.Deg2Rad;
    }

    void setShieldAngle()
    {
        Vector3 v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 v2 = _pA.ShieldTargetCrouch.transform.localPosition;
        Vector3 pos = _pA.ShieldTargetCrouch.transform.position;
        pos = new Vector3(v2.x + (v.x * _pA.shieldMaxAngle), v2.y + (v.y * _pA.shieldMaxAngle), v2.z);
        _pA.ShieldTarget.transform.localPosition = pos;
    }

    void setJumpGravity()
    {
        if (!isCrouch)
        {// state
            if (isStrafing)
            {
                if (!_pA.controller.isGrounded)
                {
                    // distance to ground
                    RaycastHit hit;
                    CharacterController charContr = _pA.controller;
                    Vector3 p1 = (transform.position + new Vector3(0, 1, 0)) + charContr.center + Vector3.up * -charContr.height * 0.4F;
                    Vector3 p2 = p1 + Vector3.up * charContr.height;

                    // Cast character controller shape 10 meters forward to see if it is about to hit anything.
                    if (Physics.CapsuleCast(p1, p2, charContr.radius, new Vector3(0, -1, 0), out hit, 10))
                        if (hit.distance < 3.0f)
                            return;

                    Transition(true, "Air");
                    return;
                }

                // jump
                if (Input.GetButton("Jump"))
                    if (isStrafing && !_pA.isHoldingObject)
                        jump();
            }

            // landed
            else if (_pA.controller.isGrounded && jumpLockFrame <= 0)
            {
                _pA.fallVelocity = _pA.groundedGravity;
                Transition(false, "Strafe");
            }
            else if (Input.GetButtonUp("Crouch"))
                _pA.anim.CrossFade("Air", 0.0f, 0, 0.0f, 0.0f);
        }
        if (isStrafing)
        {
            // shield stuff
            if (Input.GetButtonDown("Shield") || Input.GetButtonDown("Crouch"))
            {
                if (Input.GetButtonDown("Crouch"))
                {
                    _pA.anim.CrossFade("Crouch", 0.0f, 0, 0.0f, 0.0f);
                    isCrouch = true;
                    _pA.ShieldTarget.transform.position = _pA.ShieldTargetCrouch.transform.position;
                }
                else if (Input.GetButtonDown("Shield"))
                    _pA.ShieldTarget.transform.position = _pA.ShieldTargetStand.transform.position;
                if (!(Input.GetButtonDown("Shield") && Input.GetButtonDown("Crouch")))
                {
                    _pA.ShieldBack.gameObject.SetActive(false);
                    _pA.Shield.SetActive(true);
                    _pA.HandRig.weight = 1.0f;
                    _pA.AngleRig.weight = 1.0f;
                }
            }
            else if (Input.GetButtonUp("Shield") || Input.GetButtonUp("Crouch"))
            {
                if ((Input.GetButtonUp("Shield") && !Input.GetButton("Crouch")) || (Input.GetButtonUp("Crouch") && !Input.GetButton("Shield")))
                {
                    //Debug.Log("ShieldUp");
                    _pA.HandRig.weight = 0.0f;
                    _pA.AngleRig.weight = 0.0f;
                    _pA.Shield.SetActive(false);
                    _pA.ShieldBack.gameObject.SetActive(true);
                }
                if (Input.GetButtonUp("Crouch"))
                {
                    //Debug.Log("ShieldUp2");
                    _pA.anim.CrossFade("Strafe", 0.0f, 0, 0.0f, 0.0f);
                    isCrouch = false;
                    _pA.ShieldTarget.transform.position = _pA.ShieldTargetStand.transform.position;
                }
                else
                    _pA.ShieldTarget.transform.position = _pA.ShieldTargetCrouch.transform.position;
            }
        }
    }

    public void jump()
    {
        Transition(true, "Air");
        _pA.fallVelocity = _pA.jumpVelocity;
        jumpLockFrame = _pA.jumpLockFrames;
        jumpLockMag = mag;
    }

    void Transition(bool b, string s)
    {
        _pA.anim.CrossFade(s, 0.35f, 0, 0.0f, 0.0f);
        isStrafing = !b;
    }

    void setAnimParam(float mag)
    {
        _pA.anim.SetFloat(MoveZAniParaId, mag);
    }

}
