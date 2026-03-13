using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Inputs")]
    public bool inputEnabled = true;
    public InputActionAsset inputSystem;

    public bool GetAttackInput()
    {
        return inputSystem.FindAction("Click").ReadValue<float>() != 0;
    }

    public Vector2 GetMovementInput()
    {
        return inputSystem.FindAction("Move").ReadValue<Vector2>();
    }

    public bool GetSprintInput()
    {
        return inputSystem.FindAction("Sprint").ReadValue<float>() != 0;
    }

    public Vector2 GetCameraMovementInput()
    {
        return inputSystem.FindAction("Look").ReadValue<Vector2>();
    }
}
