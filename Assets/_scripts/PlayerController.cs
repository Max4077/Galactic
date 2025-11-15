using UnityEngine;

public class PlayerController : MonoBehaviour
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
        hMove = Input.GetAxis("Horizontal");    
        rMove = Input.GetAxis("Roll");

        mouseInputX = Input.GetAxis("Mouse X");
        mouseInputY = Input.GetAxis("Mouse Y");
    }

    private void FixedUpdate()
    {
        playerRB.AddForce(playerRB.transform.TransformDirection(Vector3.forward) * vMove * speed, ForceMode.VelocityChange);
        playerRB.AddForce(playerRB.transform.TransformDirection(Vector3.right) * hMove * speed, ForceMode.VelocityChange);

        playerRB.AddTorque(playerRB.transform.right * speedAngle * mouseInputY * -1, ForceMode.VelocityChange);
        playerRB.AddTorque(playerRB.transform.up * speedAngle * mouseInputX, ForceMode.VelocityChange);

        playerRB.AddTorque(playerRB.transform.forward * speedRollAngle * rMove, ForceMode.VelocityChange);
    }
}
