using System.Runtime.CompilerServices;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Rockets : MonoBehaviour
{
    public Transform player;
    private Vector3 waypoint;
    private Rigidbody rb;

    private float timer = 0;
    [SerializeField] float targetUpdateTime;
    [SerializeField] private float speed;

    void Start()
    {
        player = PhysicsPlayerController.Singleton.transform;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            waypoint = player.position;
            timer = targetUpdateTime;

        }
    }

    private void FixedUpdate()
    {
        transform.LookAt(waypoint);
        rb.AddForce(transform.forward*speed, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision other)
    {
        PlayerController playerTarget = other.gameObject.GetComponent<PlayerController>();
        if (playerTarget)
        {
            //playerTarget.health -= 10;
        }
        Destroy(this);
    }
}
