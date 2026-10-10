using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public CharacterController characterController;

    public AudioSource stepSource;

    public AudioSource jumpSource;

    public float jumpPower;

    public float sensitivity;

    public float speed;

    public float gravity = -9.81f;

    private Vector3 velocity;
    private bool IsGrounded;
    void Update()
    {
        IsGrounded = characterController.isGrounded;
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");


        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        characterController.Move(move * speed * Time.deltaTime);

        if (IsGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpPower * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);


        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * mouseX);

        //Debug.Log("H = " + Input.GetAxisRaw("Horizontal") + " | V = " + Input.GetAxisRaw("Vertical"));

    }


}
