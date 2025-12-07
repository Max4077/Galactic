using UnityEngine;

public class MousePlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float speedAngle = 0.5f;
    [SerializeField]
    float speedRollAngle = 0.05f;

    float vMove, hMove, rMove;
    float mouseInputX, mouseInputY;

    private Rigidbody playerRB;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerRB = GetComponent<Rigidbody>();        
    }

    // Update is called once per frame
    void Update()
    {
        vMove = Input.GetAxis("Vertical");
        Debug.Log($"Vmove: {vMove}");
        hMove = Input.GetAxis("Horizontal");
        Debug.Log($"hMove: {hMove}");
        rMove = Input.GetAxis("Roll");
        Debug.Log($"rMove: {rMove}");

        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        Debug.Log("Fixed Update");
        playerRB.AddForce(playerRB.transform.TransformDirection(Vector3.forward) * vMove * speed, ForceMode.VelocityChange);
        playerRB.AddForce(playerRB.transform.TransformDirection(Vector3.right) * hMove * speed, ForceMode.VelocityChange);

        playerRB.AddTorque(playerRB.transform.right * speedAngle * mouseInputY * -1, ForceMode.VelocityChange);
        playerRB.AddTorque(playerRB.transform.up * speedAngle * mouseInputX, ForceMode.VelocityChange);

        playerRB.AddTorque(playerRB.transform.forward * speedRollAngle * rMove, ForceMode.VelocityChange);
    }
}
