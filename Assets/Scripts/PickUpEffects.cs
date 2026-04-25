using UnityEngine;
using UnityEngine.UIElements;

public class PickUpEffects : MonoBehaviour
{
    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Material")]
    public Color mainColour;
    public Color secColour;
    private bool swapFromMainToSec = true;
    private float lerpColourAmount = 0.0f;
    private Material material;
    private Renderer matRenderer;

    [Header("Up&Down movement")]
    public float moveUpBy = 1.5f;
    private float lerpMoveAmount = 0.0f;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool moveUp = true;

    private void Start()
    {
        // Up&Down movement
        startPos = transform.position;
        endPos = transform.position;
        endPos.y += moveUpBy; // pushes the end position up by X amount

        // Colour change
        matRenderer = GetComponent<Renderer>();
        material = matRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        RotateAround();
        HandleColourSwap();
        HandleUpDown();
    }

    // Up & Down movement
    void HandleUpDown()
    {
        if (transform.position == startPos)
        {
            moveUp = true;
            lerpMoveAmount = 0.0f; // Resets the movement amount
        } else if (transform.position == endPos)
        {
            moveUp = false;
            lerpMoveAmount = 0.0f; // Resets the movement amount
        }

        if (moveUp)
        {
            // Moves up
            MoveUpDown(startPos, endPos);
        } 
        else
        {
            // Moves down
            MoveUpDown(endPos, startPos);
        }
    }


    void MoveUpDown(Vector3 from, Vector3 to)
    {
        lerpMoveAmount += Time.deltaTime;
        transform.position = Vector3.Lerp(from, to, Mathf.SmoothStep(0.0f, 1.0f, lerpMoveAmount));
    }

    // Colour change
    void HandleColourSwap()
    {

        if (material.GetColor("_BaseColor") == mainColour)
        {
            swapFromMainToSec = true;
            lerpColourAmount = 0.0f; // Resets the lerp amount
        }
        else if (material.GetColor("_BaseColor") == secColour)
        {
            swapFromMainToSec = false;
            lerpColourAmount = 0.0f; // Resets the lerp amount
        }


        if (swapFromMainToSec)
        {
            ShiftColour(mainColour, secColour);
        }
        else
        {
            ShiftColour(secColour, mainColour);
        }
    }

    void ShiftColour(Color from, Color to)
    {
        lerpColourAmount += Time.deltaTime;
        var newColour = Color.Lerp(from, to, Mathf.SmoothStep(0.0f, 1.0f, lerpColourAmount));
        material.SetColor("_BaseColor", newColour);
    }

    // Object rotation
    void RotateAround()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }
}
