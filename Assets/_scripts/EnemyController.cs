using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform player;
    private Vector3 currentTarget;
    
    LayerMask enemyMask, playerMask;

    float attackCooldown = 1f;
    float lastAttackTime;

    int mode = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        enemyMask = LayerMask.GetMask("Enemy");
        playerMask = LayerMask.GetMask("Player");

        //Debug.Log(AimedAtPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if(mode == 0)
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
        float playerDistance = Vector3.Distance(player.position, transform.position);

        if(playerDistance > 10f)
        {
            transform.LookAt(player.position);
            rb.AddForce(transform.forward * 5f);
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
        float objectDistance = Vector3.Distance(currentTarget, transform.position);

        if (objectDistance < 20f)
        {
            //transform.LookAt(player.position);
            //rb.AddForce(-transform.forward * 5f);
            Debug.Log("In dodge mode");
        }

        mode = 0; 
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

    private void OnTriggerEnter(Collider other)
    {
        mode = 1;
        currentTarget = other.transform.position;
        Debug.Log("Triggered by" + other.gameObject.name);
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