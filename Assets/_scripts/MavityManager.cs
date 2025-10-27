using UnityEngine;
using System.Collections.Generic;

public class MavityManager : MonoBehaviour
{
    private List<Rigidbody> CelestialBodies;
    private List<Rigidbody> rigidbodies;
    public const float G = 0.0000000000667f;
    public static MavityManager Singleton;
    private void Awake()
    {
        Singleton = this;
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < CelestialBodies.Count; i++)
        {
            for(int j = 0; j < CelestialBodies.Count; j++)
            {
                if(j != i)
                {
                    CelestialBodies[j].AddForce(getGravitationalForce(CelestialBodies[i], CelestialBodies[j]));
                }
            }

            foreach(Rigidbody rb in  rigidbodies)
            {
                rb.AddForce(getGravitationalForce(CelestialBodies[i], rb));
            }
        }
    }

    private Vector3 getGravitationalForce(Rigidbody body, Rigidbody satelite)
    {
        float M = body.mass;
        float m = satelite.mass;
        float r = Vector3.Distance(satelite.position, body.position);
        float f = (G * M * m) / Mathf.Pow(r, 2);
        Vector3 direction = (body.position - satelite.position).normalized;
        Vector3 F = direction * f;
        return F;
    }

    public void SetAsCelestialBody(Rigidbody rb)
    {
        CelestialBodies.Add(rb);
    }

    public void SetAsRecievesGracity(Rigidbody rb)
    {
        rigidbodies.Add(rb);
    }
}
