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
        movement.Set(horizontal, 0f, vertical);
        bool isRunningForward = runningForward(movement, playerToMouseAngle());
        float speed = isRunningForward ? 6f : 5.2f;

        movement = movement.normalized * speed * Time.deltaTime;
        
 
        //TODO: handle other directions too.
        animator.SetBool("Forward", isRunningForward);
        animator.SetBool("Backward", !isRunningForward);

        //Current position plus the input
        playerRigidbody.MovePosition(transform.position + movement);
    }

    //TODO:
    //Decides only between forward and backward (not having the option to move differently to other directions)
    bool runningForward(Vector3 movement, Vector3 direction) {
        float angleDifference = Vector3.Angle(movement, direction);
        return angleDifference < 90;
    }

    void Turning()
    {
        Vector3 playerToMouse = playerToMouseAngle();
        playerToMouse.y = 0f;
        playerRigidbody.MoveRotation(Quaternion.LookRotation(playerToMouse)); 
    }

    Vector3 playerToMouseAngle() {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
            return floorHit.point - transform.position;
        }
        else {
            return new Vector3();
        }
    }

    void Animating(float horizontal, float vertical)
    {
        bool running = horizontal != 0f || vertical != 0f;

        //Set value of 'running' variable to the boolean condition 'IsRunning' (created in unity, animation) 
        animator.SetBool("IsRunning", running);
    }
}
