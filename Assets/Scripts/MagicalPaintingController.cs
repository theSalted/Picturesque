using UnityEngine;

public class MagicalPaintingController : Movable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isBeingMoved)
        {
            Vector3 targetPosition = PaintingPlacementDetector.Instance.placePosition;

            // Always move the object to the target position
            Vector3 adjustedPosition;

            adjustedPosition = targetPosition;

            // Lerp towards the adjusted target position
            transform.position = Vector3.Lerp(transform.position, adjustedPosition, lerpSpeed * Time.deltaTime);

            // Update the outline color based on the isPlaceable state
            if (PaintingPlacementDetector.Instance.isPlaceable)
            {
                outline.OutlineColor = new Color(0.4196f, 0.8706f, 0.4392f); // green color
            }
            else
            {
                outline.OutlineColor = Color.red;
            }
        }
    }

    override public void OnStopMoving()
    {

    }

    override public void OnInteractCallback(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (isBeingMoved && this.enabled)
        {
            // Try to place the object
            if (PaintingPlacementDetector.Instance.isPlaceable)
            {
                // Place the object and stop moving
                isBeingMoved = false;
            }
        }
    }

    override public void OnDrawGizmos() {

    }
}
