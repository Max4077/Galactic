using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private int updatesUntilRecalibration;
    private Camera _camera;
    //private float offset;
    private Vector3 offset;

    private Quaternion previousRotation;
    private int updatesWithoutChange;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        offset = (target.position - transform.position);
        updatesWithoutChange = 0;
        previousRotation = target.rotation;
    }

    private void FixedUpdate()
    {
        if (previousRotation == target.rotation)
        {
            updatesWithoutChange++;
            if (updatesWithoutChange >= updatesUntilRecalibration)
            {
                transform.rotation = target.rotation;
            }
        }
        previousRotation = target.rotation;
        transform.LookAt(target);
        Debug.Log($"Offset: {offset}");
        transform.position = target.position - (target.rotation * offset);
    }
}
