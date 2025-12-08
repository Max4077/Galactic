//Used this for the basics of the flocking: https://codeheir.com/blog/2021/03/27/the-flocking-algorithm/


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdvancedAgent : MonoBehaviour
{
    //Properties
    [SerializeField] private float startingHealth;
    private float currentHealth;
    [SerializeField] private float scanDistance;
    [SerializeField] private float obstacleScanRate;
    [SerializeField] private float allyScanRate;
    [SerializeField] private float updateStateRate;
    [SerializeField] private float thrustForce;
    //[SerializeField] int extrapolationDistance;
    //[SerializeField] private float extrapolationStepSize;
    //[SerializeField] private float shipWidth;

    [Header("Flocking Weights")]
    [SerializeField] private float separationWeight;
    [SerializeField] private float alignmentWeight;
    [SerializeField] private float cohesionWeight;
    [SerializeField] private float targetWeight;
    [SerializeField] private float avoidanceWeight;
    [SerializeField] private float cohesionForce;
    [SerializeField] private float alignRadius;
    [SerializeField] private float safetyMargin;


    //Sensors
    private Transform player;
    private Collider[] obstacles;
    private Collider[] allies;

    //Logic
    private bool hasStarted;
    [SerializeField] private Transform tTarget;
    private Vector3 currentTarget;
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
        //player = ArrowPlayerController.Singleton.transform;
        currentHealth = startingHealth;
        hasStarted = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        currentTarget = tTarget.position;
    }

    private void OnEnable()
    {
        if(hasStarted)
        {
            StartCoroutine(scanForObstacles());
            StartCoroutine(scanForAllies());
        } 
        else
        {

        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void FixedUpdate()
    {
        Vector3 currentDirection = GetDirection();
        rb.MoveRotation(Quaternion.LookRotation(currentDirection.normalized));
        rb.AddForce(currentDirection * thrustForce, ForceMode.Force);
    }

    /*private IEnumerator scanForObstacles()
    {
        obstacles = Physics.OverlapSphere(transform.position, scanDistance);
        Bounds[] bounds = new Bounds[obstacles.Length];
        for (int i = 0; i < obstacles.Length; i++) 
            bounds[i] = obstacles[i].bounds;
        Rigidbody[] rigidbodies = new Rigidbody[bounds.Length];
        for (int i = 0;i < rigidbodies.Length; i++)
            rigidbodies[i] = obstacles[i].GetComponent<Rigidbody>();

       /* List<Bounds[]> boundsSequence = new List<Bounds[]>(); //sequence of bounds over time - more accurate but harder to work with
        boundsSequence.Add(bounds);

        for(int i = 1; i < extrapolationDistance; i++)
        {
            Bounds[] nextSequence = (Bounds[])boundsSequence[i-1].Clone();
            for(int b = 0; b < bounds.Length; b++)
            {
                nextSequence[b].center = nextSequence[b].center + (rigidbodies[b].linearVelocity * extrapolationStepSize);
            }
        }

        for(int i = 0; i < extrapolationDistance; i++) //sum bounds over time
        {
            Bounds nextUpdate = bounds[i];
            nextUpdate.center = nextUpdate.center + (rigidbodies[i].linearVelocity * extrapolationDistance);
            bounds[i].Encapsulate(nextUpdate);
        }

        Vector3 direction = transform.forward;

        if(rb.linearVelocity.magnitude > 0)
        {
            direction = rb.linearVelocity.normalized;
        }


        Ray leftRay = new Ray(transform.TransformPoint(new Vector3((-shipWidth / 2), 0, 0)), direction);
        Ray rightRay = new Ray(transform.TransformPoint(new Vector3((shipWidth / 2), 0, 0)), direction);

        for (int b = 0; b < bounds.Length; b++)
        {

        }
            

        yield return new WaitForSeconds(scanRate);
    }*/

    /*


    private bool checkRayIntersect(Ray ray, List<Bounds[]> BS)
    {
        foreach(Bounds[] S in BS)
        {
            foreach(Bounds B in S)
                if (B.IntersectRay(ray))
                    return true;
        }
        return false;
    }*/

    public IEnumerator scanForObstacles()
    {
        LayerMask layerMask = LayerMask.GetMask("AdvancedAgent", "Player");
        layerMask = ~layerMask;
        obstacles = Physics.OverlapSphere(transform.position, scanDistance, layerMask);
        yield return new WaitForSeconds(obstacleScanRate);
        StartCoroutine(scanForObstacles());
    }

    public IEnumerator scanForAllies()
    {
        LayerMask layerMask = LayerMask.GetMask("AdvancedAgent");
        allies = Physics.OverlapSphere(transform.position, scanDistance,layerMask);
        yield return new WaitForSeconds(allyScanRate);
        StartCoroutine(scanForAllies());
    }


    private Vector3 GetDirection() {

        //Get Forces
        Vector3 cohesion = getCohesionForce();
        Vector3 separation = getSeparationForce();
        Vector3 alignment = getAlignmentForce();
        Vector3 target = (currentTarget - transform.position).normalized;
        Vector3 avoidance = getAvoidanceForce();

        //Combine with Weights
        Vector3 direction = (separation * separationWeight) + (alignment * alignmentWeight) + (cohesion * cohesionWeight) + (target * targetWeight) + (avoidance * avoidanceWeight);
        return direction.normalized;
    }

    public Vector3 getCohesionForce()
    {
        Vector3 force = Vector3.zero;

        if (allies == null) return force;


        foreach(Collider c in allies)
        {
            force += c.transform.position;
        }

        force /= allies.Length;

        return (force - transform.position).normalized;
    }

    public Vector3 getSeparationForce()
    {
        Vector3 force = Vector3.zero;
        if (allies == null) return force;

        foreach (Collider c in allies)
        {
            Vector3 distance = transform.position - c.transform.position;
            force += distance;
        }

        force /= allies.Length;

        return force.normalized;
    }

    public Vector3 getAlignmentForce()
    {
        Vector3 force = Vector3.zero;
        if (allies == null) return force;

        foreach (Collider c in allies) 
        {
            Rigidbody c_rb = c.GetComponent<Rigidbody>();
            if((c_rb.position - transform.position).magnitude < alignRadius)
            {
                force += c_rb.linearVelocity;
            }
        }

        force /= allies.Length;

        return force.normalized;
    }

    public Vector3 getAvoidanceForce() {
        Vector3 force = Vector3.zero;

        if (obstacles == null) return force;

        foreach (Collider c in obstacles) 
        {
            // Inflate obstacle size for safety margin
            Bounds inflatedBounds = c.bounds;
            inflatedBounds.Expand(safetyMargin * 2f);

            Vector3 direction = transform.position - c.transform.position;
            force += direction;
        }

        force /= obstacles.Length;

        return force.normalized;
    }

    private IEnumerator updateState()
    {
        yield return new WaitForSeconds(updateStateRate);
    }
}
