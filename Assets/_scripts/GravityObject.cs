using UnityEngine;

public class GravityObject : MonoBehaviour
{
    public bool appliesGravity;
    public bool recievesGravity;
    public Vector3 initialVelocity;
    MavityManager mavityManger = null;
    public Rigidbody rb = null;
    private bool hasStarted = false;

    /// <summary>
    /// 1. cash our references to save performance
    /// 2. Add our object to the relavent MavityManager List based on the bool values we select in the Unity Inspector
    /// 3. Change a flag variable that skips OnEnable on the first run to avoid a null reference exception from the MavityManager Singleton
    /// </summary>
    public void Start()
    {
        cacheRefernces();
        AddToMavityManager();
        hasStarted = true;
    }

    // 1.
    private void cacheRefernces()
    {
        mavityManger = MavityManager.Singleton;
        rb = GetComponent<Rigidbody>();
    }

    // 2.
    private void AddToMavityManager()
    {
        rb.AddForce(initialVelocity, ForceMode.VelocityChange);
        MavityManager.Singleton.SetGravity(rb, recievesGravity, appliesGravity);
    }

    private void OnEnable()
    {
        if (!hasStarted) return; //3.
        AddToMavityManager();
    }

    /// <summary>
    /// We set both values to false so that the MavityManager does not waste time with it.
    /// </summary>
    private void OnDisable()
    {
        if (mavityManger == null || rb == null)
            cacheRefernces();

        mavityManger.SetGravity(rb, false, false);
    }
}
