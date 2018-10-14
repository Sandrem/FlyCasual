using System;
using Obstacles;
using UnityEngine;
using UnityEngine.EventSystems;

//TODO: better package for this?
/// <summary>
/// Given an object, handles touch inputs to 
/// </summary>
public class TouchObjectPlacementHandler //TODO: move more code in to this class?
{

    private GenericObstacle ChosenObstacle = null;
    private Ship.GenericShip ChosenShip = null;

    bool touchDownLastUpdate = false; // TODO: cleaner?
    private bool mouseOverObjectLastUpdate = false;
    private Vector2 lastRotationVector = Vector2.zero;

    private Vector3 newPosition = Vector3.zero;
    private float newRotation = 0f;


    public TouchObjectPlacementHandler()
    {
    }

    public void SetShip(Ship.GenericShip ship) {
        this.ChosenShip = ship;
        this.ChosenObstacle = null;
    }

    public void SetObstacle (GenericObstacle obstacle) {
        // TODO: no way to unify these, right?
        this.ChosenShip = null;
        this.ChosenObstacle = obstacle;
    }

    public void Update() {

        CameraScript.TouchInputsPaused = false; //TODO: verify this works fine here -- I think it should though

        RaycastHit hit;
        Vector3 pointerPosition = Vector3.zero;

        if (CameraScript.InputTouchIsEnabled)
        {
            if (Input.touchCount >= 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                pointerPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            }
            else
            {
                touchDownLastUpdate = false;
                mouseOverObjectLastUpdate = false;
                return;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(pointerPosition); // TODO: don't need pointerposition var any more, simplify code?

        if (Physics.Raycast(ray, out hit))
        {

            // Handle drags -- on touch devices, ships / obstacles must be dragged instead of always moving with the mouse
            float distanceFromObject = (GetObjectLocation() - new Vector3(hit.point.x, 0f, hit.point.z)).magnitude;

            if (Console.IsActive) Console.Write("distance from object:" + distanceFromObject, LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in

            if (touchDownLastUpdate && !mouseOverObjectLastUpdate)
            {
                // Don't move if something other than a ship drag is in progress
                touchDownLastUpdate = true;
                newPosition = Vector3.zero;
                return;
            }
            else if (!mouseOverObjectLastUpdate && (distanceFromObject > GetDistanceThreshold()))
            {
                // Don't move if the first touch is too far from the ship
                touchDownLastUpdate = true;
                mouseOverObjectLastUpdate = false;
                newPosition = Vector3.zero;
                return;
            }
            else
            {
                // Otherwise, move the ship if needed!
                touchDownLastUpdate = true;
                mouseOverObjectLastUpdate = true;
                CameraScript.TouchInputsPaused = true;

                if (distanceFromObject > 0f)
                {
                    newPosition = new Vector3(hit.point.x, 0f, hit.point.z);
                }
                else {
                    newPosition = Vector3.zero;
                }
            }

            // Do ship rotation on touchscreens
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
            switch (ChosenShip.ShipBaseSize)
            {
                case Ship.BaseSize.Small:
                    return 0.4f;
                case Ship.BaseSize.Medium:
                    return 0.6f;
                case Ship.BaseSize.Large:
                    return 0.8f;
            }
            // TODO: tweak thresholds?
            // TODO: any way to get the actual physical base sizes for this? this seems good enough though
            // TODO: do the threshold in physical space not world space though? hmm might not matter, this seems good enoguh, but in theory would get closer to the intent
        }

        // For obstacles, anything else
        return 0.8f;
    }
}
