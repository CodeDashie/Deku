using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class LadderState : State

{
    private CapsuleCollider _collider;
    private Collider _curLedge;
    private float _curLedgeMinY;
    private float _curLedgeMaxY;
    private float _actionTime = 0.1f;
    private float _actionTimer;
    private const float _regrabTime = 1.5f;
    private float _regrabTimer = 0.0f;
    private float _height;
    private float _ledgeClimbYTime = 0.4f;
    private float _ledgeClimbXZTime = 0.8f;

    private bool _isClimbing = false;
    private bool _isStandingUp = false;
    private bool _isClimbingUp = true;
    private bool _isClimingOnLedge = false;

    private float _climbTime;

    private Vector3 climbPos = new Vector3(0, 0, 0);


    private SariaInputActions _sariaControls;
    private InputAction _climbAction;
    private InputAction _jumpAction;
    private InputAction _dropAction;

    private void Awake()
    {
        _sariaControls = new SariaInputActions();
    }

    private void OnEnable()
    {
        _climbAction = _sariaControls.Player.Move;
        _climbAction.Enable();
        _jumpAction = _sariaControls.Player.Jump;
        _jumpAction.Enable();
        _dropAction = _sariaControls.Player.Crouch;
        _dropAction.Enable();
    }

    public override void SetValues(PlayerActor playerActor)
    {
        _pA = playerActor;
        _collider = _pA.GetComponent<CapsuleCollider>();
        //_regrabTimer = _regrabTime;
        CharacterController cc = _pA.gameObject.GetComponent<CharacterController>();
        _height = cc.height;
    }

    public override void Activate()
    {
        isActive = true;
     
        _isClimbing = false;
        _isStandingUp = false;
        _isClimbingUp = true;
        _isClimingOnLedge = false;

        _pA.anim.SetFloat("x", 0.0f);
        _pA.anim.CrossFade("Ladder", 0.0f, 0, 0.0f, 0.0f);
        _pA.fallVelocity = 0.0f;
        _regrabTimer = _regrabTime;
    }

    public override void Deactivate()
    {
        Debug.Log("Deactivate Ladder");
        isActive = false;
        //_regrabTimer = _regrabTime;
    }

    // smoothing between frames
    void FixedUpdate()
    {
        if (_regrabTimer > 0.0f)
            _regrabTimer -= Time.deltaTime;
        if (isActive)
            OnPhysics();
    }

    // accurate input
    private void Update()
    {
        if (isActive)
            OnInput();
    }

    void OnPhysics()
    {
        if (_isClimingOnLedge)
        {
            const float DISTANCE = 3.0f;
            float rotation = _curLedge.transform.eulerAngles.y * Maths.Deg2Rad;
            if (_climbTime < _ledgeClimbYTime)
                _pA.controller.Move(new Vector3(0.0f, 4.5f, 0.0f) * Time.deltaTime);
            else if (_climbTime < _ledgeClimbXZTime)
                _pA.controller.Move(new Vector3(DISTANCE * Mathf.Sin(rotation), 0.0f, DISTANCE * Mathf.Cos(rotation)) * Time.deltaTime);
            else
            {
                _isStandingUp = true;
                _isClimbing = false;
                _isClimingOnLedge = false;
                _climbTime = 0.0f;
                Deactivate();
                _pA.state[(int)PlayerActor.StateIndex.WALKING].Activate();
            }

            _climbTime += Time.deltaTime;
        }
        else
        {
            float y = _climbAction.ReadValue<Vector2>().y;

            if (y < 0.0f)
            {
                if (_isClimbingUp)
                {
                    _isClimbingUp = false;
                    _pA.anim.SetFloat("animSpeed", -1.0f);
                }
            }
            else if (!_isClimbing)
            {
                _isClimbingUp = true;
                _pA.anim.SetFloat("animSpeed", 1.0f);
            }
            if (y < 0.0f)
                _pA.anim.speed = -y * 3.5f;
            else
                _pA.anim.speed = y * 3.5f;
            _pA.controller.Move(new Vector3(0.0f, y * 5.0f, 0.0f) * Time.deltaTime);

            y = _pA.controller.transform.position.y;
            if (y < _curLedgeMinY || _pA.controller.isGrounded)
            {
                _pA.anim.SetFloat("animSpeed", 1.0f);
                _pA.SwitchState(PlayerActor.StateIndex.WALKING);
            }

            if (y + _height > _curLedgeMaxY)
            {
                _isClimingOnLedge = true;

                _pA.anim.SetFloat("x", 1.0f);
            }
        }
    }

    void OnInput()
    {
        if (_actionTimer <= 0)
        {
            if (!_isClimbing && !_isStandingUp)
            {
                float y = _climbAction.ReadValue<Vector2>().y;
                //float y = Input.GetAxisRaw("Vertical");

                // jump
                if (_jumpAction.WasPressedThisFrame())
                {
                    _pA.SwitchState(PlayerActor.StateIndex.WALKING);
                    ((MovementJumpGravity)_pA.state[(int)PlayerActor.StateIndex.WALKING]).jump();
                }
                // drop
                else if (_dropAction.WasPressedThisFrame())
                {
                    _pA.SwitchState(PlayerActor.StateIndex.WALKING);
                    float alpha = Maths.ClampAngle((_curLedge.transform.eulerAngles.y * Maths.Deg2Rad));
                    Debug.Log(alpha);
                    float xx = 2.0f * Mathf.Cos(alpha);
                    float zz = 2.0f * Mathf.Sin(alpha);
                    _pA.controller.Move(new Vector3(xx, 0.0f, zz));
                }
            }
        }
        else
            _actionTimer -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_regrabTimer <= 0.0f && other.tag == "Ladder" && _pA.stateIndex != PlayerActor.StateIndex.LADDER && !_pA.isHoldingObject)
        {
            _actionTimer = _actionTime;
            _pA.SwitchState(PlayerActor.StateIndex.LADDER);
            _curLedge = other;

            BoxCollider bc = other.GetComponent<BoxCollider>();
            float sizeY = bc.size.y / 2.0f;
            float posY = bc.transform.position.y;
            _curLedgeMinY = posY - sizeY;
            _curLedgeMaxY = posY + sizeY;
            _pA.controller.Move(new Vector3(other.transform.position.x - _pA.transform.position.x, 0.0f, other.transform.position.z - _pA.transform.position.z));
            _pA.controller.transform.eulerAngles = new Vector3(0, other.transform.eulerAngles.y, 0);
        } 
    }
}
