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
    [SerializeField] private float shootRate;
    [SerializeField] private float thrustForce;
    //[SerializeField] int extrapolationDistance;
    //[SerializeField] private float extrapolationStepSize;
    //[SerializeField] private float shipWidth;

    [Header("Stratagy Values")]
    [SerializeField] float startFlankingDistance;
    [SerializeField] float startDivingDistance;
    public AdvancedAgent[] squad;
    public bool readyToDive = false;
    public string phase = "chase";
    private bool isLeader = false;

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
    private Vector3 currentTarget;
    private Rigidbody rb;
    private Collider agentCollider;
    private string[] flankingPositions = { "in front", "below", "above", "behind"};

    //phases chase -> flank -> dive

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
        hasStarted = true;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentHealth <= 0) gameObject.SetActive(false);
        Debug.Log($"Phase: {phase}");
    }

    private void OnEnable()
    {
        if(hasStarted)
        {
            StartCoroutine(scanForObstacles());
            StartCoroutine(scanForAllies());
            StartCoroutine(shoot());
            StartCoroutine(updateState());
        } 
        else
        {

        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        currentHealth = startingHealth;
        squad = null;
        allies = null;
    }

    private void FixedUpdate()
    {
        SetTarget();
        Vector3 currentDirection = GetDirection();
        transform.forward = currentDirection;
        //rb.MoveRotation(Quaternion.LookRotation(currentDirection.normalized));
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

    public void SetTarget()
    {
        switch (phase)
        {
            case ("chase"):
                currentTarget = player.position;
                shootRate = 60f * Random.Range(2f, 5f);
                break;
            case ("behind"):
                currentTarget = player.position - (player.forward * 100);
                break;
            case ("above"):
                currentTarget = player.position + (player.up * 100);
                break;
            case ("in front"):
                currentTarget = player.position + (player.forward * 100);
                break;
            case ("below"):
                currentTarget = player.position - (player.up * 100);
                break;
            case ("dive"):
                currentTarget = player.position;
                shootRate = 20f;
                break;
        }
    }

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

    public IEnumerator shoot()
    {
        yield return new WaitForSeconds(shootRate);
        //Do Shoot
        StartCoroutine(shoot());
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
        if(phase == "chase")
        {
            if ((transform.position - player.position).magnitude <= startFlankingDistance)
            {
                squad = new AdvancedAgent[allies.Length];
                squad[0] = this;
                isLeader = true;
                phase = "in front";
                rb.linearVelocity = Vector3.zero;
                separationWeight = 0f;
                cohesionWeight = 0f;
                alignmentWeight = 0f;
                for (int i = 1; i < allies.Length; i++)
                {
                    AdvancedAgent aA = allies[i].GetComponent<AdvancedAgent>();
                    if (aA.phase == "chase")
                    {
                        squad[i] = aA;
                        aA.phase = flankingPositions[i % flankingPositions.Length];
                        aA.squad = squad;
                        aA.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                        squad[i].cohesionWeight = 0f;
                        squad[i].alignmentWeight = 0f;
                    }
                }
            }
            

        }
        else if (phase == "dive")
        {
            if ((transform.position - player.position).magnitude > startFlankingDistance)
            {

                foreach (AdvancedAgent aA in squad)
                {
                    aA.phase = "chase";
                    aA.squad = null;
                    aA.isLeader = false;
                }
                squad = null;
                isLeader = false;

            }
        }
        else
        {
            if((transform.position - currentTarget).magnitude <= startDivingDistance)
            {
                readyToDive = true;
                if (isLeader )
                {
                    bool doDive = true;
                    for (int i = 0; i < squad.Length; i++)
                    {
                        if (!squad[i].readyToDive) doDive = false;
                    }
                    if (doDive)
                    {
                        for (int i = 0; i < squad.Length; i++)
                        {
                            squad[i].phase = "dive";
                            squad[i].GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                            squad[i].separationWeight = 0f;
                            squad[i].cohesionWeight = 0f;
                            squad[i].alignmentWeight = 0f;

                        }
                    }
                }
            }
        }
            yield return new WaitForSeconds(updateStateRate);
        StartCoroutine(updateState());
    }


}
