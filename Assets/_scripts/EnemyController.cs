using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform player;
    private Vector3 playerPosition;
    LayerMask enemyMask, playerMask;

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
        //playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Chase(); 
        
    }

    private void FixedUpdate()
    {
        //Debug.Log(Target());
        
    }

    void Chase()
    {
        float playerDistance = Vector3.Distance(player.position, transform.position);

        if(playerDistance > 10f)
        {
            transform.LookAt(player.position);
            rb.AddForce(transform.forward * 0.5f);
        }

        if(Target() == 1)
        {
            Fire();
        }
    }
    void Dodge()
    {

    }

    void Fire()
    {
        Debug.Log("Pew Pew!!");
    }

    float UtilityFunction(GameState currentState)
    {
        float score = 0;

        score += -Vector3.Distance(this.transform.position, player.position);

        return score;
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
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            return 1;
        }
        else
        {
            //Debug.Log("Hit nothing :(");
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }
        return -1;
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