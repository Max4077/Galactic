using UnityEditor;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnCooldown = 1f;
    private float lastSpawnTime = 0f;

    public Transform spawnLocation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnShip();
    }

    void SpawnShip()
    {
        if (Time.time - lastSpawnTime < spawnCooldown) return;
        lastSpawnTime = Time.time;

        Debug.Log("Baddie time");
        Instantiate(Resources.Load("SMA"), spawnLocation.position, Quaternion.identity);
    }
}
