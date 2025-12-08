using UnityEngine;

public class PredictionRockets : MonoBehaviour
{
    public Transform player;
    private Vector3 waypoint;
    private Rigidbody rb;
    

    [SerializeField] private float speed;

    void Start()
    {
        player = ArrowPlayerController.Singleton.transform;
        rb = GetComponent<Rigidbody>();
        waypoint = player.position + new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2));
    }


    private void FixedUpdate()
    {
        transform.LookAt(waypoint);
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);

        if (this.gameObject.transform.position == waypoint)
        {
            Destroy(this.gameObject);
        }

    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Player")
        {
            ArrowPlayerController playerTarget = other.gameObject.GetComponent<ArrowPlayerController>();
            Debug.Log("Collided with player");
            playerTarget.health -= 1;
        }
        Destroy(this.gameObject);
    }
}
