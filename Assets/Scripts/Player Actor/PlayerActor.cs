using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class PlayerActor : MonoBehaviour
{
    // --

    public enum StateIndex
    {
        WALKING,            // main state
        ON_LEDGE,           // main state
        PUSHING_OBJECT,     // main state
        //HOLDING_OBJECT    // attribute state
        LADDER,             // main state
        SIZE
    }
    
    public StateIndex stateIndex { get; set; }
    public State[] state;

    public void SwitchState(StateIndex s)
    {
        state[(int)stateIndex].Deactivate();
        state[(int)s].Activate();
        stateIndex = s;
    }

    // --

    // attributes
    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    public float airSpeed = 10.0f;
    public float airSpeedClamp = 0.1f;
    public float turnSpeed = 17.5f;
    public float turnSpeedAir = 7.5f;

    public float idleMag = 0.1f;
    public float walkMag = 0.2f;
    public float runMag = 0.7f;
    public int idleFrames = 4;
    public int walkFrames = 4;

    public float idleAnimParam = 0.0f;
    public float walkAnimParam = 0.7f;
    public float runAnimParam = 1.0f;

    public float jumpVelocity = 10.0f;
    public int jumpLockFrames = 3;
    public float gravity = -0.4f;
    public float groundedGravity = -4.0f;

    public float pushPower = 2.0f;
    public float throwPower = 250.0f;
    
    [HideInInspector]
    public new Camera camera;

    public MultiAimConstraint ChestAim;
    public MultiAimConstraint HeadAim;
    public GameObject ShieldTarget;
    public GameObject ShieldTargetStand;
    public GameObject ShieldTargetCrouch;
    public GameObject ShieldBack;

    public bool isController = true;

    // state
    public bool isWalking       { get; set; } = false;
    public bool isJumping       { get; set; } = false;
    public bool isStrafe        { get; set; } = false;
    public bool isStrafeShield  { get; set; } = false;
    public bool isCrouch        { get; set; } = false;
    public bool isOnLedge       { get; set; } = false;
    public bool isClimbing      { get; set; } = false;
    public bool isHoldingObject { get; set; } = false;

    public float fallVelocity { get; set; } = 0.0f;
    
    public Transform head;
    public Animator anim { get; set; }
    public CharacterController controller { get; set; }
    [SerializeField]
    private GameObject _grabLedgeBox;
    [SerializeField]
    private GameObject _grabObjectBox;
    public GameObject Shield;
    public float shieldMaxAngle = 20.0f;
    public Rig HandRig;
    public Rig AngleRig;

    public delegate void OnPlayerDeathDelegate();
    //public static event OnPlayerDeathDelegate onPlayerDeathEvent;
    public OnPlayerDeathDelegate onPlayerDeath;

    void Awake()
    {
        camera = FindObjectOfType<Camera>();

        // cursor settings
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // grab components for public use
        anim = GetComponentInChildren<Animator>();
        controller = gameObject.GetComponent<CharacterController>();

        // set default state
        stateIndex = StateIndex.WALKING;

        state = new State[(int)(StateIndex.SIZE)];
        // attach and setup script components
        //GrabLedge grabLedge = _grabBox.AddComponent<GrabLedge>();

        state[(int)StateIndex.WALKING] = gameObject.AddComponent<MovementJumpGravity>();
        state[(int)StateIndex.WALKING].SetValues(this);
        state[(int)StateIndex.WALKING].Activate();

        state[(int)StateIndex.LADDER] = _grabLedgeBox.AddComponent<LadderState>();
        state[(int)StateIndex.LADDER].SetValues(this);
        
        //_grabLedgeBox.AddComponent<GrabLedge>().SetValues(this);
        //_grabObjectBox.AddComponent<GrabObject>().SetValues(this);
        //gameObject.AddComponent<MoveObject>().SetValues(this);
        gameObject.AddComponent<AnimationState>().SetValues(this);
        //gameObject.AddComponent<FallingPlatformCollide>();
        //gameObject.AddComponent<Button>().SetValues(this);
    }

    private void OnLevelWasLoaded(int level)
    {
        camera = FindObjectOfType<Camera>();
    }

    public void SwitchState()
    {

    }
}
