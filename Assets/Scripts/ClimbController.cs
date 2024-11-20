using UnityEngine;

public class ClimbController : MonoBehaviour
{
    public float speed;        // Speed of climbing
    private bool isClimbing = false;     // Tracks whether the player is climbing

    private CharacterController characterController; // CharacterController reference
    private Vector3 climbDirection;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isClimbing)
        {
            Climb();
        }
    }

    // private void HandleClimbing()
    // {
    //     // Use the "move.y" input for climbing (W/S keys or Up/Down arrows)
    //     if (_input.move.y > 0) // W is pressed
    //     {
    //         Vector3 climbMovement = Vector3.up * ClimbSpeed * Time.deltaTime;
    //         _characterController.Move(climbMovement);
    //     }
    // }

    private void Climb()
    {
        // Get vertical input (e.g., W to move up, S to move down)
        float vertical = Input.GetAxis("Vertical");

        // Calculate climbing direction (up/down along Y axis)
        climbDirection = new Vector3(0, vertical * speed, 0);

        // Move the player along the climb direction
        characterController.Move(climbDirection * Time.deltaTime);

        // Disable gravity while climbing
        characterController.Move(Vector3.zero);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if the player collides with a climbable object
        if (hit.collider.CompareTag("Climbable"))
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false; // Exit climbing mode if no longer touching a climbable object
        }
    }
}
