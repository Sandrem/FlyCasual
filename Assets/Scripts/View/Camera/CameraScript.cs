using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * DESCRIPTION
 * Movement, vertical rotation and zoom of main camera.
 * Movement, vertical rotation and zoom have MIN and MAX values.
 * Event when view is changed.
*/

public class CameraScript : MonoBehaviour {

    private static Transform Camera;
    private static Transform GameObjectTransform;

    private const float SENSITIVITY_MOVE = 0.125f;
    private const float SENSITIVITY_TURN = 5;
    private const float SENSITIVITY_ZOOM = 5;
    private const float SENSITIVITY_TOUCH_MOVE = 0.015f;
    private const float SENSITIVITY_TOUCH_TURN = 0.125f;
    private const float SENSITIVITY_TOUCH_ZOOM = 0.075f;
    private const float THRESHOLD_TOUCH_TURN = 0.05f;
    private const float THRESHOLD_TOUCH_ZOOM = 0.06f;
    private const float MOUSE_MOVE_START_OFFSET = 5f;
    private const float BORDER_SQUARE = 8f;
    private const float MAX_HEIGHT = 6f;
    private const float MIN_HEIGHT = 1.5f;
    private const float MAX_ROTATION = 89.99f;
    private const float MIN_ROTATION = 0f;

    public static bool InputAxisAreDisabled = false;
    public static bool InputMouseIsDisabled = false;

    private enum CameraModes
    {
        Free,
        TopDown
    }
    private static CameraModes cameraMode = CameraModes.Free;

    // Use this for initialization
    void Start()
    {
        Camera = transform.Find("Main Camera");
        GameObjectTransform = transform;

        ChangeMode(CameraModes.Free);
    }

    private static void SetDefaultCameraPosition()
    {
        bool isSecondPlayer = (Network.IsNetworkGame && !Network.IsServer);

        Camera camera = Camera.GetComponent<Camera>();
        camera.orthographicSize = 6;

        Camera.localEulerAngles = (cameraMode == CameraModes.Free) ? new Vector3(-50, 0, 0) : new Vector3(0, 0, 0);
        GameObjectTransform.localEulerAngles = new Vector3(90, 0, (!isSecondPlayer) ? 0 : 180);
        GameObjectTransform.localPosition = (cameraMode == CameraModes.Free) ? new Vector3(0, 6, (!isSecondPlayer) ? -8 : 8) : Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Call hide context menu only once
        CheckChangeMode();

        if (true) {//Input.touchSupported) {
            CamAdjustByTouch();
        }
        else
        {
            CamMoveByMouse();
            CamZoomByMouseScroll();
            CamRotateByMouse();
        }

        CamMoveByAxis();
        CamClampPosition();
    }

    // CAMERA MODES

    private void CheckChangeMode()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Console.IsActive) ToggleMode();
    }

    public static void ToggleMode()
    {
        ChangeMode((cameraMode == CameraModes.Free) ? CameraModes.TopDown : CameraModes.Free);
    }

    private static void ChangeMode(CameraModes mode)
    {
        cameraMode = mode;

        Camera camera = Camera.GetComponent<Camera>();
        camera.orthographic = (mode == CameraModes.Free) ? false : true;

        SetDefaultCameraPosition();
    }

    // Movement, Rotation, Zoom

    private void CamMoveByAxis()
    {
        if (Console.IsActive || Input.GetKey(KeyCode.LeftControl)) return;
        if (InputAxisAreDisabled) return;

        float x = Input.GetAxis("Horizontal") * SENSITIVITY_MOVE;
        float y = Input.GetAxis("Vertical") * SENSITIVITY_MOVE;
        if ((x != 0) || (y != 0)) WhenViewChanged();
        transform.Translate (x, y, 0);
	}

    private void CamMoveByMouse()
    {
        if (InputMouseIsDisabled) return;

        float x = 0;
        if (Input.mousePosition.x < MOUSE_MOVE_START_OFFSET && (Screen.fullScreen || Input.mousePosition.x >= 0)) x = -1f * SENSITIVITY_MOVE;
        else if (Input.mousePosition.x > Screen.width - MOUSE_MOVE_START_OFFSET && (Screen.fullScreen || Input.mousePosition.x <= Screen.width)) x = 1f * SENSITIVITY_MOVE;

        float y = 0;
        if (Input.mousePosition.y < MOUSE_MOVE_START_OFFSET && (Screen.fullScreen || Input.mousePosition.y >= 0)) y = -1f * SENSITIVITY_MOVE;
        else if (Input.mousePosition.y > Screen.height - MOUSE_MOVE_START_OFFSET && (Screen.fullScreen || Input.mousePosition.y <= Screen.height)) y = 1f * SENSITIVITY_MOVE;

        if ((x != 0) || (y != 0)) WhenViewChanged();
        transform.Translate(x, y, 0);
    }

    private void CamZoomByMouseScroll()
    {
		float zoom = Input.GetAxis ("Mouse ScrollWheel") * SENSITIVITY_ZOOM;
		if (zoom != 0)
        {
            ZoomByFactor(zoom);
        }	
	}

    private void ZoomByFactor(float zoom) {
        if (cameraMode == CameraModes.Free)
        {
            Vector3 newPosition = transform.position + (Camera.TransformDirection(0, 0, zoom));
            float zoomClampRate = 1;
            if (newPosition.y <= MIN_HEIGHT)
            {
                zoomClampRate = (transform.position.y - MIN_HEIGHT) / zoom;
            }
            if (newPosition.y >= MAX_HEIGHT)
            {
                zoomClampRate = (transform.position.y - MAX_HEIGHT) / zoom;
            }
            transform.Translate(transform.InverseTransformDirection(Camera.TransformDirection(0, 0, zoom * zoomClampRate)));
        }
        else
        {
            Camera camera = Camera.GetComponent<Camera>();
            camera.orthographicSize -= zoom;
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 1, 6);
        }

        WhenViewChanged();
    }

	private void CamRotateByMouse()
    {
        if (cameraMode == CameraModes.Free)
        {
            if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
            {

                float turnX = Input.GetAxis("Mouse Y") * -SENSITIVITY_TURN;
                turnX = CamClampRotation(turnX);
                Camera.Rotate(turnX, 0, 0);

                float turnY = Input.GetAxis("Mouse X") * -SENSITIVITY_TURN;
                transform.Rotate(0, 0, turnY);

                if ((turnX != 0) || (turnY != 0)) WhenViewChanged();
            }
        }
	}

    // Restrictions for movement and rotation 

    private float CamClampRotation(float turnX)
    {
		float currentTurnX = Camera.eulerAngles.x;
		float newTurnX = turnX + currentTurnX;
		if (newTurnX > MAX_ROTATION) {
			turnX = MAX_ROTATION - currentTurnX;
		}
		else if (newTurnX < MIN_ROTATION) {
			turnX = MIN_ROTATION - currentTurnX;
		}
		return turnX;
	}

	private void CamClampPosition()
    {
		transform.position = new Vector3 (
			Mathf.Clamp(transform.position.x, -BORDER_SQUARE, BORDER_SQUARE),
			Mathf.Clamp(transform.position.y, MIN_HEIGHT, MAX_HEIGHT),
			Mathf.Clamp(transform.position.z, -BORDER_SQUARE, BORDER_SQUARE)
		);
	}

    // What to do when view is changed

    private void WhenViewChanged()
    {
        UI.HideTemporaryMenus();
    }

    // Pinch zoom for mobile

    void CamAdjustByTouch()
    {
            //**** reverse rotation???? to make it match panning -- or vice versa...?

        if (Input.touchCount > 0 && (Input.GetTouch(0).position.x > Screen.width || Input.GetTouch(0).position.y > Screen.height)) {
            // Don't listen to off-screen touches
            return;
        }

        // If there are two touches on the device
        if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) // TODO: need to check phase too...?
        {
            //TODO: pan vs zoom just by how far apart fingers are....? hmmmmmm ***
            //TODO: or the threshold approach that I had in mind but overcompliced
            //TODO: or find best practice!!
            // Store both touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Console.Write("Zoom:" + deltaMagnitudeDiff, LogTypes.Errors, true, "cyan");
            // Try to pinch zoom
            if (Mathf.Abs(deltaMagnitudeDiff) > THRESHOLD_TOUCH_ZOOM)
            { //**** still need - threshold??h
                //****or differentiate by distance between fingers...? not delta of that????
                float zoom = deltaMagnitudeDiff * -SENSITIVITY_TOUCH_ZOOM;
                ZoomByFactor((Mathf.Abs(zoom) - THRESHOLD_TOUCH_ZOOM) * Mathf.Sign(zoom)); // TODO: cleaner...?
            }

            // Try to rotate by dragging two fingers
            if (cameraMode == CameraModes.Free)
            {

                // Find the difference between the average of the positions
                Vector2 centerPrevPos = Vector2.Lerp(touchZeroPrevPos, touchOnePrevPos, 0.5f);
                Vector2 centerPos = Vector2.Lerp(touchZero.position, touchOne.position, 0.5f);
                Vector2 deltaCenterPos = centerPos - centerPrevPos;

                Console.Write("rot mag:" + (deltaCenterPos.magnitude), LogTypes.Errors, true, "cyan");

                if (Mathf.Abs(deltaCenterPos.magnitude) > THRESHOLD_TOUCH_TURN) {
                    // Rotate!

                    float turnX = deltaCenterPos.y * -SENSITIVITY_TOUCH_TURN;
                    turnX = CamClampRotation(turnX);
                    Camera.Rotate(turnX, 0, 0);

                    float turnY = deltaCenterPos.x * -SENSITIVITY_TOUCH_TURN;
                    transform.Rotate(0, 0, turnY);

                    if ((turnX != 0) || (turnY != 0)) WhenViewChanged();

                    //TODO: dedupe? not much code though
                }
            }
        }
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {

            //TODO: or reverse pan back to non-direct manipulation....? it's weird that it's reversed from the others maybe??
            //// TODO: momentum?
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;

            float x = deltaPosition.x * -SENSITIVITY_TOUCH_MOVE;
            float y = deltaPosition.y * -SENSITIVITY_TOUCH_MOVE;

            Console.Write("pan x:"+x+" y:"+x, LogTypes.Errors, true, "cyan");

          //  if (touchPanAmountX < 1 || touchPanAmountY < 1) return; //TODO: threshold value? make constant

            if ((x != 0) || (y != 0)) WhenViewChanged();
            transform.Translate(x, y, 0);
       
        }
        else if (Input.touchCount > 2 && Input.GetTouch(2).phase == TouchPhase.Ended) {
            // TODO: move this...? to non-camera hanlding??
            Console.IsActive = !Console.IsActive;
        }
    }

}
