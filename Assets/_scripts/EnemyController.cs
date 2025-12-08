using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform player;
    private Vector3 currentTarget;
    
    LayerMask enemyMask, playerMask;

    float attackCooldown = 1f;
    float lastAttackTime = 0f;
    float targetCooldown = 2f;
    float lastTargetTime = 0f;

    int state = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        currentTarget = player.position;

        enemyMask = LayerMask.GetMask("Enemy");
        playerMask = LayerMask.GetMask("Player");

        //Debug.Log(AimedAtPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if(state == 0)
        {
            Chase();
        }
        else
        {
            Dodge();
        }
    }

    void Chase()
    {
        
        UpdateTarget();
        
        float playerDistance = Vector3.Distance(currentTarget, transform.position);
        Debug.Log("Distance to player: " +  playerDistance);

        if(playerDistance > 100f)
        {
            var targetRotation = Quaternion.LookRotation(currentTarget - transform.position);

            // Smoothly rotate towards the target point.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5 * Time.deltaTime);


            //transform.LookAt(currentTarget);
            rb.AddForce(transform.forward * 5f);
        }
        else
        {
            if(!rb.angularVelocity.Equals(Vector3.zero)) rb.AddForce(transform.forward * -5f);
        }

        if (Time.time - lastAttackTime < attackCooldown) return;
        lastAttackTime = Time.time;

        if (Target() == 1)
        {
            Fire();
        }
    }

    void Dodge()
    {
        //Physics.OverlapSphere(transform.position, scanDistance);
    }

    void Fire()
    {
        Debug.Log("Pew Pew!!");
    }

    int Target()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, enemyMask))
        {
            //Debug.Log("Hit an Enemy");
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return 0;
        }
        else if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, playerMask))
        {
            //Debug.Log("Hit a Player");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return 1;
        }
        else
        {
            //Debug.Log("Hit nothing :(");
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }
        return -1;
    }

    void UpdateTarget()
    {
        if (Time.time - lastTargetTime < targetCooldown) return;
        lastTargetTime = Time.time;

        currentTarget = player.position;
        Debug.Log("Target Updated");
    }

    float UtilityFunction(GameState currentState)
    {
        float score = 0;

        score += -Vector3.Distance(this.transform.position, player.position);

        return score;
    }
}

public class GameState
{
    public Transform playerPos {  get; set; }
    public float playerHealth { get; set; }

    GameState(Transform playerPos, float playerHealth)
    {
        this.playerPos = playerPos;  
        this.playerHealth = playerHealth;
    }
}