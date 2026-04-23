using UnityEngine;

public class PickUpEffects : MonoBehaviour
{
    public float rotationSpeed = 3.5f;

    private void Start()
    {

        // Rotates object in line with terrain surface
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit))
        {
            transform.up = hit.normal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
