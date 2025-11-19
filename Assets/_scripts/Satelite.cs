using UnityEngine;
/// <summary>
/// Used when we want the Initial Velocity to cause the object to orbit another
/// Right now it only works when Y is zero
/// And does not consider any other gravitational forces so the moon is not working yet
/// </summary>
public class Satelite : GravityObject
{
    [SerializeField] private Rigidbody objectToOrbit;

    /// <summary>
    /// 0. we get the velocity we need to maintain orbit
    /// 1. We flatten the position of the object to orbit (remove the y axis)
    /// 2. We flatten the position of the satelite
    /// 3. We get a 2D vector that points from the satelite towards the object to orbit (follows allong the lines of gravitational force)
    /// 4. We get the direction perpindicular to the gravitational force
    /// 5. We un flatten the vector and return and multiply by the necesary velocity
    /// </summary>
    public void Awake()
    {
        float velocity = getOrbitVelocity(); //0.
        Vector2 objectToOrbit2Dpos= new Vector2(objectToOrbit.position.x,objectToOrbit.position.z); //1.
        Vector2 satelite2Dpos = new Vector2(transform.position.x, transform.position.z); //2.
        Vector2 direction2D = (objectToOrbit2Dpos - satelite2Dpos).normalized; //3.
        Vector2 perpindecular = Vector2.Perpendicular(direction2D); //4.
        Vector3 direction = new Vector3(perpindecular.x, 0, perpindecular.y); //5.
        initialVelocity = direction * velocity;
    }

    public void Start()
    {
        setConstantVelocity();

        base.Start();
    }

    private void setConstantVelocity()
    {
        Satelite target = objectToOrbit.GetComponent<Satelite>();
        if (target == null) return;

        initialVelocity += target.initialVelocity;
    }


    /// <summary>
    /// 0. Is just the equation for orbit velocity
    /// velocity = sqrt((Gravittional Constant * Mass of the object to orbit)/distance between the objects)
    /// v = sqrt(GM/r)
    /// </summary>
    /// <returns></returns>
    private float getOrbitVelocity()
    {
        float G = MavityManager.G;
        float r = Vector3.Distance(transform.position, objectToOrbit.position);
        float M = objectToOrbit.mass;
        float v = Mathf.Sqrt((G*M)/r);
        return v;
    }
}
