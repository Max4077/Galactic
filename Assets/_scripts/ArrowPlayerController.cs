using System;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.InputSystem;

public class ArrowPlayerController : MonoBehaviour
{
    [SerializeField] private float thrustAcceleration;
    [SerializeField] private float rollAcceleration;
    [SerializeField] private float pitchAcceleration;
    [SerializeField] private float yawAcceleration;

    private float thrust, roll, pitch, yaw;
    private int flipRoll = -1;
    private int flipPitch = 1;
    private int flipYaw = 1;
    private bool isThrusting, isRolling, isPitching, isYawing;
    private float mouseInputX, mouseInputY;

    public int health = 3;

    private InputActionMap playerInput;
    [SerializeField] private Rigidbody playerRB;
    private InputAction thrustRollAction;
    private InputAction pitchYawAction;

    public static ArrowPlayerController Singleton;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 angularVelo;

    private float[] forwardThrustCurve;
    [SerializeField] VisualEffect leftBooster;
    [SerializeField] VisualEffect rightBooster;

    [SerializeField] private bool debug;
    private void Awake()
    {
        playerInput = InputSystem.actions.FindActionMap("Player", true);
        thrustRollAction = playerInput.FindAction("Thrust-Roll", true);
        pitchYawAction = playerInput.FindAction("Pitch-Yaw", true);
        Singleton = this;
        //playerRB = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        playerInput.Enable();
        thrustRollAction.Enable();
        pitchYawAction.Enable();
    }
    private void OnDisable()
    {
        playerInput.Disable();
        thrustRollAction.Disable();
        pitchYawAction.Disable();
    }

    public void FixedUpdate()
    {
        ApplyForces();
        velocity = playerRB.linearVelocity;
        angularVelo = playerRB.angularVelocity;
    }

    public void Update()
    {
        getInputs();
        AnimateRockets();

        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void getInputs()
    {
        Vector2 thrustRoll = thrustRollAction.ReadValue<Vector2>();
        Vector2 pitchYaw = pitchYawAction.ReadValue<Vector2>();
        thrust = thrustRoll.y;
        roll = -pitchYaw.x;
        pitch = pitchYaw.y;
        yaw = thrustRoll.x;


        isThrusting = (thrust != 0);
        isRolling = (roll != 0);
        isPitching = (pitch != 0);
        isYawing = (yaw != 0);

        if(debug)
        {
            Debug.Log($"Thrust: {thrust}");
            Debug.Log($"Roll: {roll}");
            Debug.Log($"Pitch: {pitch}");
            Debug.Log($"Yaw: {yaw}");
        }
    }

    private void ApplyForces() //swap to AddRelativeTorque
    {
        Vector3 localAngularVelocity = transform.InverseTransformDirection(playerRB.angularVelocity);

        //thrust
        if (isThrusting)
        {
            playerRB.AddForce(playerRB.transform.forward * thrust * thrustAcceleration, ForceMode.Acceleration);
        }

        //roll
        if (isRolling)
        {
            playerRB.AddRelativeTorque(new Vector3(0, 0, roll * rollAcceleration), ForceMode.Acceleration);
        }

        //pitch
        if (isPitching)
        {
            playerRB.AddRelativeTorque(new Vector3(pitch * pitchAcceleration, 0, 0), ForceMode.Acceleration);
        }

        //yaw
        if (isYawing)
        {
            playerRB.AddRelativeTorque(new Vector3(0, yaw * yawAcceleration, 0), ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision other)
    {
       health -= 1;
    }

    private void AnimateRockets()
    {
        leftBooster.SetVector3("Velocity 1", -velocity);
        rightBooster.SetVector3("Velocity", velocity);
    }
}

