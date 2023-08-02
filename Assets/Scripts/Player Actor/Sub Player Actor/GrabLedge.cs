using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLedge : MonoBehaviour
{
    /*const float HALF_PI = 1.7079632679490f;
    const float PI = 3.14159265358979f;
    const float TAU = 6.28318530717958f;
    const float Rad2Deg = 180.0f / PI;
    const float Deg2Rad = PI / 180.0f;

    private PlayerActor _pA;
    private CapsuleCollider _collider;
    private Collider _curLedge;
    private float _actionTime = 0.1f;
    private float _actionTimer;
    private float _regrabTime = 0.1f;
    private float _regrabTimer;

    private bool _isClimbing = false;
    private bool _isStandingUp = false;

    private float _climbTime;

    private Vector3 climbPos = new Vector3(0, 0, 0);

    public void SetValues(PlayerActor playerActor)
    {
        _pA = playerActor;
        _collider = _pA.GetComponent<CapsuleCollider>();
    }

    void ResetAllTriggers()
    {
        foreach (AnimatorControllerParameter parameter in _pA.anim.parameters)
            _pA.anim.ResetTrigger(parameter.name);
    }

    void FixedUpdate()
    {
        /*Vector3 pAP = _pA.transform.position;
        float test = (Mathf.Atan2(pAP.z + 13.0f, pAP.x) * (-1.0f)) + (PI / 2);
        if (test < 0.0f)
            test += (PI * 2);
        Debug.Log(test);*/
        /*
        if (_pA.anim.GetCurrentAnimatorStateInfo(0).IsName("Climb") && _isClimbing)
        {
            const float DISTANCE = 3.0f;
            float rotation = transform.eulerAngles.y * Deg2Rad;
            if (_climbTime < 0.45f)
            {
                _pA.controller.Move(new Vector3(0.0f, 4.5f, 0.0f) * Time.deltaTime);
            }
            else if (_climbTime < 0.7f)
            {
                //Vector3 v = new Vector3(DISTANCE * Mathf.Sin(rotation), 0.0f, DISTANCE * Mathf.Cos(rotation));
                _pA.controller.Move(new Vector3(DISTANCE * Mathf.Sin(rotation), 0.0f, DISTANCE * Mathf.Cos(rotation)) * Time.deltaTime);
            }
            else
            //if (_pA.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                _isStandingUp = true;
                _isClimbing = false;
                _pA.anim.SetTrigger("isStandingUp");

                Debug.Log(_climbTime);
                _climbTime = 0.0f;
                //Vector3 v = new Vector3(0.0f, 2.0f, 0.0f);
                //Vector3 gv = _pA.gameObject.transform.position + v;
                //_pA.controller.Move(v);

                //const float DISTANCE = 1.0f;
                //float rotation = transform.eulerAngles.y * Deg2Rad;
                //v.x = ;
                //v.y = ;
                //v = new Vector3(DISTANCE * Mathf.Sin(rotation), 0.0f, DISTANCE * Mathf.Cos(rotation));

                //_pA.controller.radius = _pA.DEFAULT_CHARACTER_CONTROLLER_RADIUS;
                //_pA.controller.Move(v);
            }

            _climbTime += Time.deltaTime;
        }
        //if (_pA.anim.GetCurrentAnimatorStateInfo(0).IsName("StandUp") && _isStandingUp)
        if (_pA.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && _isStandingUp)
        {
            //if (_pA.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                _isStandingUp = false;
                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");

                if (x == 0.0f && y == 0.0f)
                    ;// _pA.anim.SetTrigger("isIdling");

                _pA.state = PlayerActor.State.WALKING;
            }
        }

        if (_regrabTimer >= 0.0f)
            _regrabTimer -= Time.deltaTime;
    }

    void Update()
    {
        if (_actionTimer <= 0)
        {
            if (_pA.state == PlayerActor.State.ON_LEDGE && !_isClimbing && !_isStandingUp)
            {

                float y = Input.GetAxisRaw("Vertical");
                
                // jump
                if (Input.GetButton("Jump"))
                {
                    _regrabTimer = _regrabTime;
                    ResetAllTriggers();
                    _pA.anim.SetTrigger("isJumping");
                    _pA.isJumping = true;
                    _pA.fallVelocity = _pA.jumpVelocity;
                    _pA.state = PlayerActor.State.WALKING;
                }
                // crawl up
                else if (y > 0.0f)
                {
                    ResetAllTriggers();
                    _pA.anim.SetTrigger("isClimbing");
                    _isClimbing = true;
                }
                // drop
                else if (y < 0.0f)
                {
                    _regrabTimer = _regrabTime;
                    _pA.state = PlayerActor.State.WALKING;

                    ResetAllTriggers();
                    _pA.anim.SetTrigger("isFalling");

                    //_pA.anim.SetTrigger("isIdling");

                    //_pA.controller.Move(new Vector3(0.0f, 0.0f, _pA.DEFAULT_CHARACTER_CONTROLLER_RADIUS - _curLedge.transform.position.z - _pA.transform.position.z - 20.0f));

                    //_pA.controller.radius = _pA.DEFAULT_CHARACTER_CONTROLLER_RADIUS;
                }
            }
        }
        else
            _actionTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_regrabTimer <= 0.0f && other.tag == "Ledge" && _pA.state != PlayerActor.State.ON_LEDGE && !_pA.isHoldingObject)
        {
            // setup current state and reset variables

                _actionTimer = _actionTime;
                _pA.isWalking = false;
                _pA.state = PlayerActor.State.ON_LEDGE;
                _curLedge = other;

                ResetAllTriggers();
                _pA.anim.SetTrigger("isLedgeGrab");

                _pA.fallVelocity = 0.0f;
            

            GameObject o = other.transform.GetChild(0).gameObject;
            
            // move air position

                _pA.controller.Move(new Vector3(0.0f, o.transform.position.y - _pA.transform.position.y - 0.95f, 0.0f));

            // move ground position

            // hypotenuse
            // (o = object)
            // (h = hypotonuse)
            // (pA = player actor)

                float oAngle = o.transform.eulerAngles.y * Deg2Rad;
                float hAngle = oAngle + HALF_PI;
                float pAScale = o.transform.localScale.x;// / 2.0f;
            
                Vector3 v = new Vector3(pAScale * Mathf.Sin(hAngle), 0.0f, pAScale * Mathf.Cos(hAngle));
            
                Vector3 p = o.transform.position + v;
                Vector3 pAP = _pA.transform.position;

                float hypotenuse = DistanceBetweenTwoPoints(new Vector2(p.x, p.z), new Vector2(pAP.x, pAP.z));

            // angle

                float x = p.x - pAP.x;
                float y = p.z - pAP.z;
                float angle = Mathf.Atan2(y, x);
                angle = ClampAngle(((angle * (-1.0f)) + (PI / 2)) - oAngle);
            
            // adjacent

                float adjacent = hypotenuse * Mathf.Cos(angle);
                //adjacent -= _pA.controller.radius;


            //
            
            Vector3 vA = new Vector3(adjacent * Mathf.Sin(oAngle), 0.0f, adjacent * Mathf.Cos(oAngle));
            _pA.controller.Move(vA);
            
            _pA.transform.rotation = Quaternion.Euler(new Vector3(0.0f, o.transform.eulerAngles.y, 0.0f));
        }

    }
    
    private static float ClampAngle(float a)
    {
        if (a < 0.0f)
            a += TAU;
        if (a > TAU)
            a -= TAU;

        return a;
    }

    private static float DistanceBetweenTwoPoints(Vector2 vA, Vector2 vB)
    {
        float x = vB.x - vA.x;
        float y = vB.y - vA.y;

        return Mathf.Sqrt(x * x + y * y);
    }
    */
}
