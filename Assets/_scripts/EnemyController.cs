using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Chase(); 
    }

    private void FixedUpdate()
    {
        
        
    }

    void Chase()
    {
        this.transform.LookAt(player.position);
        rb.AddForce(player.position * 20);
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