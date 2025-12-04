using System.Runtime.CompilerServices;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Rockets : MonoBehaviour
{
    public PlayerController Player;

    private GameObject target;
    private Vector3 waypoint;

    private float timer = 0;

    public float speed = 4.0f;

    void Awake()
    {
        target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            waypoint = target.transform.position;
            timer = 0.5f;

        }

        
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
