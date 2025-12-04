using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsPlayerController : MonoBehaviour
{
    [SerializeField] private float thrustAcceleration;
    [SerializeField] private float thrustDeceleration;
    [SerializeField] private float rollAcceleration;
    [SerializeField] private float rollDeceleration;
    [SerializeField] private float pitchAcceleration;
    [SerializeField] private float pitchDeceleration;
    [SerializeField] private float yawAcceleration;
    [SerializeField] private float yawDeceleration;

    private float thrust, roll, pitch, yaw;
    private int flipRoll = -1;
    private int flipPitch = 1;
    private int flipYaw = 1;
    private bool isThrusting, isRolling, isPitching, isYawing;
    private float mouseInputX, mouseInputY;

    private InputActionMap playerInput;
    [SerializeField] private Rigidbody playerRB;
    private InputAction thrustRollAction;
    private InputAction pitchYawAction;

    public static PhysicsPlayerController Singleton;
    [SerializeField] Vector3 velocity;
    [SerializeField] Vector3 angularVelo;

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
        else
        {
            if (playerRB.linearVelocity.magnitude != 0f)
            {
               // playerRB.AddForce(playerRB.linearVelocity.normalized * thrustDeceleration * -1);
            }
        }

        //roll
        if (isRolling)
        {
            playerRB.AddRelativeTorque(new Vector3(0, 0, roll * rollAcceleration), ForceMode.Acceleration);
        }
        else
        {
            /*if (localAngularVelocity.z != 0f)
            {
                playerRB.AddRelativeTorque(new Vector3(0, 0, Mathf.Sign(localAngularVelocity.z) * rollDeceleration * -1f), ForceMode.Acceleration);
            }*/
        }

        //pitch
        if (isPitching)
        {
            playerRB.AddRelativeTorque(new Vector3(pitch * pitchAcceleration, 0, 0), ForceMode.Acceleration);
        }
        else
        {
            /**if(localAngularVelocity.x != 0f)
            {
                playerRB.AddRelativeTorque(new Vector3(Mathf.Sign(localAngularVelocity.x) * -1, 0, 0), ForceMode.Acceleration);
            }*/
        }

        //yaw
        if (isYawing)
        {
            playerRB.AddRelativeTorque(new Vector3(0, yaw * yawAcceleration, 0), ForceMode.Acceleration);
        } 
        else
        {
            /*if(localAngularVelocity.y != 0f)
            {
                playerRB.AddRelativeTorque(new Vector3(0, localAngularVelocity.y * yawDeceleration * -1, 0), ForceMode.Force);
            }*/
        }

        /*if(!isRolling && !isPitching && !isYawing)
        {
            if (localAngularVelocity.magnitude != 0f)
            {
                playerRB.AddTorque(-1 * yawDeceleration * localAngularVelocity);
            }
        }*/
    }
}
