using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameCharacterController : MonoBehaviour
{
    // Movement variables
    public float walkSpeed = 2f;
    public float sprintBoost = 1.5f;
    public float crouchedHeight = 0.75f;
    // Camera Mouse-Look variables
    private float mouseYaw = 0.0f;
    private float mousePitch = 0.0f;
    public float mouseSensitivity = 2.5f;
    // Character RigidBody component variable
    private Rigidbody playerRb;
    // Input Manager
    public InputActionAsset PlayersInputSystem;
    // Input Actions
    InputAction PlayerSprint;
    InputManager inputManager;
    public CinemachineCamera CinemachineFPS;
    public GameObject Player;
    private Vector2 movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Locks the cursor to the center of screen
        Cursor.lockState = CursorLockMode.Locked;
        // Assigns the Character's rigid body component to the variable
        playerRb = gameObject.GetComponent<Rigidbody>();
        // Assigns player's input actions
        PlayerSprint = PlayersInputSystem.FindAction("Sprint");

        inputManager = GameManager.instance.inputManager;
    }

    private void FixedUpdate()
    {
        // Player's Movement function
        CharacterMove();
    }

    void Update()
    {
        // Player's Camera - Mouse Look function
        CharacterMouseLook();
        movement = inputManager.GetCameraMovementInput();
    }

    // Mouse Look function
    void CharacterMouseLook()
    {
        // Gets the mouse up/down movement
        mousePitch -= movement.y * mouseSensitivity * Time.deltaTime;
        // Not letting the camera spinning above or under character 
        mousePitch = Mathf.Clamp(mousePitch, -90.0f, 90.0f);
        // Gets the mouse left/right movement
        mouseYaw += movement.x * mouseSensitivity * Time.deltaTime;
        // Moves the camera around based on the mouse movement
        CinemachineFPS.transform.localRotation = Quaternion.Euler(mousePitch, mouseYaw, 0);
        Player.transform.localRotation = Quaternion.Euler(0, mouseYaw, 0);
    }

    // Character's movement function
    void CharacterMove()
    {
        // Vector used to store player's movement 
        Vector2 axis = inputManager.GetMovementInput();
        
        // Getting the camera's rotation and passing it into Vector3 variable
        Vector3 fwd = new Vector3(-Camera.main.transform.right.z, 0.0f, Camera.main.transform.right.x);
        // Calculates the final direction in which the player wants to move
        Vector3 moveToDirection = (fwd * axis.y * walkSpeed + Camera.main.transform.right * axis.x * walkSpeed + Vector3.up * playerRb.linearVelocity.y);
        // Moves the player's character
        playerRb.linearVelocity = moveToDirection;
    }
}
