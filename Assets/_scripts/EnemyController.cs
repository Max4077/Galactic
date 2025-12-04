using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetDirection = (player.position - transform.position).normalized;

        Vector3 avoidance = Vector3.zero;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 20f))
        {            
            avoidance = hit.normal * 30f;
        }

        Vector3 desiredDirection = (targetDirection + avoidance).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(desiredDirection, Vector3.up);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 2.5f * Time.fixedDeltaTime));

        float distanceToPlayer = Vector3.Distance(this.transform.position, player.position);
        //Debug.Log("Distance to player: " +  distanceToPlayer);

        rb.AddForce(transform.forward * 20f, ForceMode.Acceleration);
        
    }
}
