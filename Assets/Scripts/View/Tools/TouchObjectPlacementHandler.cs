using System;
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


    public TouchObjectPlacementHandler()
    {
    }

    public SetShip(Ship.GenericShip ship) { //TODO: call this from other class
        this.ChosenShip = ship;
        this.ChosenObstacle = null;
    }

    public SetObstacle (GenericObstacle obstacle) { //TODO: call this from other class
        // TODO: no way to unify these, right?
        this.ChosenShip = null;
        this.ChosenObstacle = obstacle;
    }

    public Update() {
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

            // Handle drags -- on touch, ships / obstacles must be dragged instead of always moving with the mouse
            float distanceFromObject = (GetObjectLocation() - new Vector3(hit.point.x, 0f, hit.point.z)).magnitude;

            if (Console.IsActive) Console.Write("distance from object:" + distanceFromObject, LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in

            if (touchDownLastUpdate && !mouseOverObjectLastUpdate)
            {
                // Don't move if something other than a ship drag is in progress
                touchDownLastUpdate = true;
                return;
            }
            else if (!mouseOverObjectLastUpdate && (distanceFromObject > GetDistanceThreshold()))
            {
                // Don't move if the first touch is too far from the ship
                touchDownLastUpdate = true;
                mouseOverObjectLastUpdate = false;
                return;
            }
            else
            {
                // Otherwise, move the ship!
                touchDownLastUpdate = true;
                mouseOverObjectLastUpdate = true;
                CameraScript.TouchInputsPaused = true;
            }

            // Do ship rotation on touchscreens
            if (Input.touchCount == 2)
            {

                Vector2 currentRotationVector = Input.GetTouch(1).position - Input.GetTouch(0).position;

                if (lastRotationVector != Vector2.zero)
                {
                    float rotationAngle = Vector2.SignedAngle(lastRotationVector, currentRotationVector) * -2;
                    ChosenObstacle.ObstacleGO.transform.localEulerAngles += new Vector3(0, rotationAngle, 0); //TODO: store results in vars instead of doing anything with them
                }
                lastRotationVector = currentRotationVector;

            }
            else
            {
                lastRotationVector = Vector2.zero;
            }
        }

    }

    // Returns the new position, or vector.zero if no change this update
    public Vector3 GetNewPosition() {
        // TODO: return 0 if no change / if same as last update?
    }

    // Returns the new rotation, or 0f if no change this update
    public float GetNewRotation() {
        // TODO: Future work: show a nice UI handle on the screen for rotation instead of using two-fingers. Two fingers makes it really hard to see what you're doing with small ships
        // TODO: return 0 if no change / if same as last update?
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
            // TODO: or a switch statement?
            if (ChosenShip.ShipBaseSize == BaseSize.Ship.BaseSize.Small) {
                return 0.4f;
            }
            else if (ChosenShip.ShipBaseSize == BaseSize.Ship.BaseSize.Medium)
            {
                return 0.6f;

            }
            else if (ChosenShip.ShipBaseSize == BaseSize.Ship.BaseSize.Large)
            {
                return 0.8f;
            }
            // TODO: tweak thresholds?
            // TODO: any way to get the actual base sizes or anything? maybe this is good enough though
            // TODO: do the threshold in physical space not world space though? hmm might not matter, but in theory would get closer to the intent

        }
        else
        {
            return 0.8f;
        }
    }
}
