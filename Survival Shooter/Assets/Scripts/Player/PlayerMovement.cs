using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float forwardSpeed = 6f;
    public float backwardSpeed = 4f;

    Vector3 movement;
    Animator animator;
    Rigidbody playerRigidbody;
    int floorMask;
    float camRayLength = 100f;

    //Automatically called on start by unity
    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        animator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    /*
     * Automatically called by unity, every time on physics component change.
     * Applies to every physics character that has RigidBody attached.
     */
    private void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Move(horizontal, vertical);
        Turning();
        Animating(horizontal, vertical);
    }
   
    void Move(float horizontal, float vertical)
    {
        float speed = 6f;
        movement.Set(horizontal, 0f, vertical);
        movement = movement.normalized * speed * Time.deltaTime;

        //Current position plus the input
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning()
    {
        playerRigidbody.MoveRotation(playerToMouseAngle()); 
    }

    Quaternion playerToMouseAngle() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;
            return Quaternion.LookRotation(playerToMouse);
        }
        else {
            return new Quaternion();
        }
    }

    void Animating(float horizontal, float vertical)
    {
        bool running = horizontal != 0f || vertical != 0f;

        //animator.speed = -1;

        //Set value of 'running' variable to the boolean condition 'IsRunning' (created in unity, animation) 
        animator.SetBool("IsRunning", running);
    }
}
