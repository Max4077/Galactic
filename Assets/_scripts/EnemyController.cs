using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform player;
    private Vector3 playerPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        Chase(); 
    }

    private void FixedUpdate()
    {
        
        
    }

    void Chase()
    {
        //this.transform.LookAt(player.position);
        //rb.AddForce(player.position * 0.5f);

        this.transform.LookAt(playerPosition);
        this.transform.position = Vector3.MoveTowards(this.transform.position, playerPosition, 20f);
    }
    void Dodge()
    {

    }

    float UtilityFunction(GameState currentState)
    {
        float score = 0;

        score += -Vector3.Distance(this.transform.position, player.position);

        return score;
    }

    //bool AimedAtPlayer()
    //{
    //    RaycastHit hit;
    //}
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