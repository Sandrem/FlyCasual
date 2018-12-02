using System;
using Obstacles;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Given an object, handles touch inputs to move and rotate it, and outputs
/// the changes that should be made to the object's location and rotation to
/// match the touch inputs.
/// </summary>
public class TouchObjectPlacementHandler
{

    private GenericObstacle ChosenObstacle = null;
    private Ship.GenericShip ChosenShip = null;

    bool touchDownLastUpdate = false;
    private bool draggingObjectLastUpdate = false;
    private Vector2 lastRotationVector = Vector2.zero;

    private Vector3 newPosition = Vector3.zero;
    private float newRotation = 0f;
    private int firstFinger = -1;

    public TouchObjectPlacementHandler()
    {
    }

    public void SetShip(Ship.GenericShip ship) {
        this.ChosenShip = ship;
        this.ChosenObstacle = null;

        newPosition = Vector3.zero;
        newRotation = 0f;
    }

    public void SetObstacle (GenericObstacle obstacle) {
        // TODO: may be better to operate on the GameObject for the ship and obtacles, since that's the commonality? Then we couldn't get things like ship base size, but the GameObject renderer / colider size is probably just as good for this purpose
        this.ChosenShip = null;
        this.ChosenObstacle = obstacle;

        newPosition = Vector3.zero;
        newRotation = 0f;
    }

    public void Update() {

        CameraScript.TouchInputsPaused = false;

        // No fingers are down, first finger is over the UI, or the first finger has been lifted while the second is still down
        if (Input.touchCount == 0 || EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) ||
           (firstFinger != -1 && Input.GetTouch(0).fingerId != firstFinger))
        { 
            // Reset touch tracking
            touchDownLastUpdate = false;
            draggingObjectLastUpdate = false;
            firstFinger = -1;

            // Stop rotation
            lastRotationVector = Vector2.zero;
            newRotation = 0f;

            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Input.GetTouch(0).position.x, 
                                                           Input.GetTouch(0).position.y));

        if (Physics.Raycast(ray, out hit))
        {
            // Handle drags -- on touch devices, ships / obstacles must be dragged instead of always moving with the mouse
        
            float distanceFromObject = (GetObjectLocation() - new Vector3(hit.point.x, 0f, hit.point.z)).magnitude;
            bool touchOverObject = (distanceFromObject <= GetDistanceThreshold());
        
            if (Console.IsActive) Console.Write("distance from object:" + distanceFromObject, LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in

            if (touchDownLastUpdate && !draggingObjectLastUpdate)
            {
                // Don't move if touch was down last update but wasn't on the object
                // That means something other than an object drag is in progress
                touchDownLastUpdate = true;
                newPosition = Vector3.zero;
                newRotation = 0f;
                return;
            }
            else if (!draggingObjectLastUpdate && !touchOverObject)
            {
                // Don't move if it's a new touch but didn't hit the object
                touchDownLastUpdate = true;
                draggingObjectLastUpdate = false;
                newPosition = Vector3.zero;
                newRotation = 0f;
                return;
            }
            else if ((touchDownLastUpdate && draggingObjectLastUpdate) || 
                     (!touchDownLastUpdate && touchOverObject))
            {
                // Otherwise, we're continuing a drag or starting a new one
                // Move the object if needed!
                touchDownLastUpdate = true;
                draggingObjectLastUpdate = true;
                CameraScript.TouchInputsPaused = true;

                if (distanceFromObject > 0f)
                {
                    newPosition = new Vector3(hit.point.x, 0f, hit.point.z);
                }
                else {
                    newPosition = Vector3.zero;
                }

                // Record which finger is the first one 
                firstFinger = Input.GetTouch(0).fingerId;
            }

            // Do object rotation on touchscreens
            if (Input.touchCount == 2)
            {

                Vector2 currentRotationVector = Input.GetTouch(1).position - Input.GetTouch(0).position;

                if (lastRotationVector != Vector2.zero)
                {
                    newRotation = Vector2.SignedAngle(lastRotationVector, currentRotationVector) * -2;
                }
                lastRotationVector = currentRotationVector;

            }
            else
            {
                lastRotationVector = Vector2.zero;
                newRotation = 0f;
            }
        }

    }

    // Returns the new position, or vector.zero if no change this update
    public Vector3 GetNewPosition() {
        return newPosition;
    }

    // Returns the new rotation, or 0f if no change this update
    public float GetNewRotation() {
        // TODO: Future work: show a nice UI handle on the screen for rotation instead of using two-fingers. Two fingers makes it really hard to see what you're doing with small ships
        return newRotation;
    }

    private Vector3 GetObjectLocation() {
        if (ChosenObstacle != null) {
            return ChosenObstacle.ObstacleGO.transform.position;
        }
        else if (ChosenShip != null) {
            return ChosenShip.GetCenter();
        }
        else {
            Console.Write("Tried to move an obstacle or ship by touch but no obstacle or ship was selected", LogTypes.Errors, true, "red");
            return Vector3.zero;
        }
    }

    private float GetDistanceThreshold() {
        if (ChosenShip != null)
        {
            switch (ChosenShip.ShipInfo.BaseSize)
            {
                case Ship.BaseSize.Small:
                    return 0.4f;
                case Ship.BaseSize.Medium:
                    return 0.6f;
                case Ship.BaseSize.Large:
                    return 0.8f;
            }
            // TODO: do the threshold in physical space not world space though? hmm might not matter, this seems good enoguh, but in theory would get closer to the intent
        }

        // For obstacles, anything else
        return 0.8f;
    }
}
