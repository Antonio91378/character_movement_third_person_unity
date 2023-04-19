using UnityEngine;
using Cinemachine;

public class Moviment : MonoBehaviour
{
    public float moveSpeed = 6.0f;
    public float speed = 5.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float rotationSpeed = 360.0f;
    private bool isMoving = false;

    [SerializeField] private CinemachineFreeLook freeLookCamera;
    private CharacterController controller;
    private Transform cameraTransform;
    public Animator animator;
    public Camera mainCamera;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = freeLookCamera.transform;

        if (controller == null)
        {
            Debug.LogError("CharacterController component not found on " + gameObject.name);
        }

        if (cameraTransform == null)
        {
            Debug.LogError("Camera with tag 'MainCamera' not found");
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        if (controller == null || cameraTransform == null)
        {
            return;
        }

        // Get the movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move character in the direction of input
        Vector3 cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0f;
        Vector3 moveDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        moveDirection = moveDirection.normalized;

        // Apply gravity
        if (controller.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        if (isMoving)
        {
            // Move character
            Vector3 moveDirectionRelativeToCamera = mainCamera.transform.TransformDirection(moveDirection);
            controller.Move(moveDirectionRelativeToCamera.normalized * moveSpeed * Time.deltaTime);


            // Update the animator
            if (animator != null)
            {
                if (controller.velocity.magnitude > 0.1f)
                {
                    // Rotate the character to face the direction of movement
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirectionRelativeToCamera, Vector3.up);

                    // Block rotation around Y && Z axis
                    Vector3 eulerAngles = targetRotation.eulerAngles;
                    eulerAngles.x = 0f;
                    eulerAngles.z = 0f;
                    targetRotation.eulerAngles = eulerAngles;

                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    animator.SetBool("IsRunning", true);
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                }
            }
        }
        // Check if the player is moving

        if (horizontal != 0 || vertical != 0 )
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
            animator.SetBool("IsRunning", false);
        }
     
        Debug.DrawLine(transform.position, transform.position + moveDirection.normalized, Color.blue);
        Debug.DrawLine(transform.position, transform.position + controller.velocity.normalized, Color.red);
    }
}
