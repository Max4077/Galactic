using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AdvancedAgent : MonoBehaviour
{
    //Properties
    [SerializeField] private float startingHealth;
    private float currentHealth;
    [SerializeField] private float scanDistance;
    [SerializeField] private float scanRate;
    [SerializeField] private float updateStateRate;
    [SerializeField] int extrapolationDistance;
    [SerializeField] private float extrapolationStepSize;


    //Sensors
    private Transform player;
    private Collider[] obstacles;

    //Logic
    private bool hasStarted;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private Collider agentCollider;

    private void Awake()
    {
        hasStarted = false;
        rb = GetComponent<Rigidbody>();
        agentCollider = GetComponent<Collider>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = ArrowPlayerController.Singleton.transform;
        currentHealth = startingHealth;
        StartCoroutine(scanForObstacles());
        hasStarted = true;
    }

    private void OnEnable()
    {
        if(hasStarted)
        {
            StartCoroutine(scanForObstacles());
            StartCoroutine(updateState());
        } 
        else
        {

        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private IEnumerator scanForObstacles()
    {
        obstacles = Physics.OverlapSphere(transform.position, scanDistance);
        Bounds[] bounds = new Bounds[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++) 
            bounds[i] = obstacles[i].bounds;
        Rigidbody[] rigidbodies = new Rigidbody[bounds.Length];
        for (int i = 0;i < rigidbodies.Length; i++)
            rigidbodies[i] = obstacles[i].GetComponent<Rigidbody>();

        Vector3[] positions = new Vector3[extrapolationDistance];

        for(int i = 0; i < positions.Length; i++)
        {
            positions[i] = transform.position + (rb.linearVelocity * extrapolationStepSize * i);
        }

        for(int i = 0; i < positions.Length; i++)
        {
            for (int b = 0; b < bounds.Length; b++)
                bounds[b].center = bounds[b].center + (rb.linearVelocity * extrapolationStepSize * i);

            int clearCount = 0;
            for(int b = 0; b < bounds.Length; b++)
            {
                if (bounds[b].Intersects(agentCollider.bounds))
            }

        }


        yield return new WaitForSeconds(scanRate);
    }

    private IEnumerator updateState()
    {
        yield return new WaitForSeconds(updateStateRate);
    }
}
