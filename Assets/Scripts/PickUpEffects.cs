using UnityEngine;
using UnityEngine.UIElements;

public class PickUpEffects : MonoBehaviour
{
    public float rotationSpeed = 10f;
    public float updownMovementSpeed = 2f;
    public float newPositionOffset = 2f;
    private Vector3 startLocalPos;
    private Vector3 endLocalPos;
    private bool isMovingUp = true;

    private void Start()
    {
        startLocalPos = transform.position;
        endLocalPos = new(startLocalPos.x, startLocalPos.y + newPositionOffset, startLocalPos.z);
    }

    // Update is called once per frame
    void Update()
    {
        RotateAround();
        MoveUpDown();
    }

    void MoveUpDown()
    {
        if (transform.position == startLocalPos) { isMovingUp = true; }
        if (transform.position == endLocalPos) { isMovingUp = false; }

        if (isMovingUp)
        {
            transform.position = Vector3.Lerp(startLocalPos, endLocalPos, Time.deltaTime * updownMovementSpeed);
        } else
        {
            transform.position = Vector3.Lerp(endLocalPos, startLocalPos, Time.deltaTime * updownMovementSpeed);
        }
    }

    void RotateAround()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }
}
