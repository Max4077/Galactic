using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Mavity? - https://www.youtube.com/watch?v=5AGhQst987A
/// </summary>
public class MavityManager : MonoBehaviour
{
    [SerializeField] private float mavityUpdateTime;
    private float oldMavityUpdateTime;
    private List<Rigidbody> appliesGravity; //Contains every rigidbody that we want to inflict gravitational force on other objects
    private List<Rigidbody> recievesGravity; //Contains every rigidbody that we want to recieve gravitational forces from other objects
    public const float G = 10f; //10f; //Gravitational constant is normaly 6.67x10^-11. Is currently 1 for simplicity
    public static MavityManager Singleton; //Uses a singleton structure so that it can be accesssed by any object

    /// <summary>
    /// Set the reference for the Singleton class.
    /// Any reference to this singleton from another class
    /// must come after Awake()
    /// (Specifically this instance's Awake())
    /// </summary>
    private void Awake()
    {
        Singleton = this;
        appliesGravity = new List<Rigidbody>();
        recievesGravity = new List<Rigidbody>();
        oldMavityUpdateTime = mavityUpdateTime;
        handleMavityTime();
            
    }

    public void FixedUpdate()
    {
        DoMravity();
    }

    public void Start()
    {
        //StartCoroutine(mavityRoutine());
    }

    public IEnumerator mavityRoutine()
    {
        DoMravity();
        handleMavityTime();
        yield return new WaitForSeconds(mavityUpdateTime);
        yield return StartCoroutine(mavityRoutine());
    }

    private void handleMavityTime()
    {
        if (oldMavityUpdateTime <= 0)
            oldMavityUpdateTime = 0.2f;

        if (mavityUpdateTime <= 0)
            mavityUpdateTime = oldMavityUpdateTime;
        else
            oldMavityUpdateTime = mavityUpdateTime;
    }
    
    /// <summary>
    /// Applies the gravataional force of every
    /// object big enough to enact relavent gravitational
    /// to every object we want to respond to gravitational force
    /// </summary>
    private void DoMravity()
    {
        foreach (Rigidbody celestialObject in appliesGravity)
        {
            foreach (Rigidbody satelite in recievesGravity)
            {
                satelite.AddForce(getGravitationalForce(celestialObject, satelite));
            }
        }
    }

    /// <summary>
    /// This is just the equation for Univercal Gravitational Force: 
    /// Force of Gravity = (Gravitational Constant)(Mass of one object)(mass of the other object)/(distance between the objects^2)
    /// F_G = (GMm)/(r^2)
    /// </summary>
    /// <param name="body">The body that is enacting the gravitational force</param>
    /// <param name="satelite">The body that the gravitational force is acting upon</param>
    /// <returns></returns>
    private Vector3 getGravitationalForce(Rigidbody body, Rigidbody satelite)
    {
        float r = Vector3.Distance(satelite.position, body.position);
        if (Mathf.Approximately(r,0f))
            return Vector3.zero;
        float M = body.mass;
        float m = satelite.mass;
        float f = (G * M * m) / Mathf.Pow(r, 2);
        Vector3 direction = (body.position - satelite.position).normalized;
        Vector3 F = direction * f;
        return F;
    }

    /// <summary>
    /// Adds or removes rigidbodies from the applies gravity list based on bool b
    /// </summary>
    /// <param name="rb">Rigidbody to add to/remove from List</param>
    /// <param name="b">true = add, false = remove</param>
    public void SetAsApplieslGravity(Rigidbody rb, bool b)
    {
        if(b)
            appliesGravity.Add(rb);
        else
            appliesGravity.Remove(rb);
    }

    /// <summary>
    /// Adds or removes rigidbodies from the recieves gravity list based on bool b
    /// </summary>
    /// <param name="rb">Rigidbody to add to/remove from List</param>
    /// <param name="b">true = add, false = remove</param>
    public void SetAsRecievesGravity(Rigidbody rb, bool b)
    {
        if(b)
            recievesGravity.Add(rb);
        else
            recievesGravity.Remove(rb);
    }

    /// <summary>
    /// Nests the above to functions into one to make code easier to read elswhere
    /// </summary>
    /// <param name="rb">Rigidbody to add to/remove from List</param>
    /// <param name="recievesGravity">Whether to add to or remove from recieves gravity List</param>
    /// <param name="appliesGravity">Whether to add to or remove from the applies gravity List</param>
    public void SetGravity(Rigidbody rb, bool recievesGravity, bool appliesGravity)
    {
        SetAsRecievesGravity(rb, recievesGravity);
        SetAsApplieslGravity(rb, appliesGravity);
    }
}
