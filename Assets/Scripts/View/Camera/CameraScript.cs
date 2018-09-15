﻿using System.Collections;
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
    private const float MOUSE_MOVE_START_OFFSET = 5f;
    private const float BORDER_SQUARE = 8f;
    private const float MAX_HEIGHT = 6f;
    private const float MIN_HEIGHT = 1.5f;
    private const float MAX_ROTATION = 89.99f;
    private const float MIN_ROTATION = 0f;

    private const float SENSITIVITY_TOUCH_MOVE = 0.015f;
    private const float SENSITIVITY_TOUCH_TURN = 0.125f;
    private const float SENSITIVITY_TOUCH_ZOOM = 0.0375f; // was .075//TODO: Turn down a bit? Seems a bit too fast still, especially in 2d mode
    //TODO: need to ensure thresholds are resolution-independant?
    private const float THRESHOLD_TOUCH_MOVE_MOMENTUM = 10f;
    private const float THRESHOLD_TOUCH_TURN = 0.05f;  //TODO: right value? seems ok, but could be lower if needed now that other values are set?
    private const float THRESHOLD_TOUCH_TURN_SWITCH = 40f; // TODO: was 30 -- better on ipad??
    private const float THRESHOLD_TOUCH_TURN_START = 20f;
    private const float THRESHOLD_TOUCH_ZOOM = 0.06f; //TODO: right value? seems ok, but could be lower if needed now that other values are set?
    private const float THRESHOLD_TOUCH_ZOOM_SWITCH = 30f;
    private const float THRESHOLD_TOUCH_ZOOM_START = 20f; // was 12 -- is that better on ipad? probably!!! needs to be higher on iphone!!
    private const float FRICTION_TOUCH_MOVE_MOMENTUM = 0.9f; //or .95?

    private float initialPinchMagnitude = 0f; // Magnitude of the pinch when 2 fingers are first put on the screen
    private float lastProcessedPinchMagnitude = 0f; // Magnitude of the pinch when we last actually zoomed
    private Vector2 initialRotateCenter = new Vector2(0.0f, 0.0f);
    private Vector2 lastProcessedRotateCenter = new Vector2(0.0f, 0.0f);
    private Vector2 panningMomentum = new Vector2(0.0f, 0.0f);
    private float totalTouchMoveDuration = 0f;
    private Vector2 totalTouchMove = new Vector2(0.0f, 0.0f);

    public static bool InputAxisAreDisabled = false;
    public static bool InputMouseIsDisabled = false;
    public static bool InputTouchIsEnabled = false; //TODO: Key everytihng off this so it can be disabled etc. add console command prolly?
    public static bool TouchMovePaused = false;

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

        InputTouchIsEnabled = Input.touchSupported;
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

        if (InputTouchIsEnabled) {
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

        if (Input.touchCount > 0 && (Input.GetTouch(0).position.x > Screen.width || Input.GetTouch(0).position.y > Screen.height ||
                                     TouchMovePaused)) {
            // Don't listen to touches that are off-screen, or being handled elsewhere
            return;
        }

        // If there are two touches on the device
        if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)) // TODO: need to check phase too...?
        {
            // Store both touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Normalize for DPI
            touchDeltaMag = touchDeltaMag / Screen.dpi * 265;

            // Initialize values when 2 fingers are first touched to the screen
            if (initialPinchMagnitude == 0f) {
                initialPinchMagnitude = touchDeltaMag;
                lastProcessedPinchMagnitude = touchDeltaMag;
            }

            float startThreshold = 0;

            if (initialRotateCenter != lastProcessedRotateCenter) {
                // A pinch is in progress
                startThreshold = THRESHOLD_TOUCH_ZOOM_SWITCH;
            }
            else if (initialPinchMagnitude == lastProcessedPinchMagnitude) {
                // A zoom is not yet in progress
                startThreshold = THRESHOLD_TOUCH_ZOOM_START;
            }

            // Try to pinch zoom if we pass a start threshold
            if (Mathf.Abs(initialPinchMagnitude-touchDeltaMag) > startThreshold)
            {
                if (Console.IsActive) Console.Write("Zoom:" + Mathf.Abs(initialPinchMagnitude - touchDeltaMag), LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in
                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = lastProcessedPinchMagnitude - touchDeltaMag;

                if (startThreshold != 0)
                {
                    deltaMagnitudeDiff = (Mathf.Abs(deltaMagnitudeDiff) - startThreshold) * Mathf.Sign(deltaMagnitudeDiff);
                }

                if (Mathf.Abs(deltaMagnitudeDiff) > THRESHOLD_TOUCH_ZOOM)
                {
                    if (Console.IsActive) Console.Write("Zoom2:" + deltaMagnitudeDiff, LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in
                    float zoom = deltaMagnitudeDiff * -SENSITIVITY_TOUCH_ZOOM;
                    ZoomByFactor(zoom);

                    lastProcessedPinchMagnitude = touchDeltaMag;

                    // Turn of pan for now
                    initialRotateCenter = lastProcessedRotateCenter;
                }
            }

            // Try to rotate by dragging two fingers
            if (cameraMode == CameraModes.Free)
            {
                // Find the difference between the average of the positions
                Vector2 centerPos = Vector2.Lerp(touchZero.position, touchOne.position, 0.5f);

                if (initialRotateCenter.magnitude == 0) {
                    initialRotateCenter = centerPos;
                    lastProcessedRotateCenter = centerPos;
                }

                startThreshold = 0f;
                if (initialPinchMagnitude != lastProcessedPinchMagnitude)
                {
                    // A pinch is in progress
                    startThreshold = THRESHOLD_TOUCH_TURN_SWITCH;
                }
                else if (initialRotateCenter == lastProcessedRotateCenter)
                {
                    // A zoom is not yet in progress
                    startThreshold = THRESHOLD_TOUCH_TURN_START;
                }

                // If we pass a start threshold, try the rotation
                if (Mathf.Abs((initialRotateCenter - centerPos).magnitude) > startThreshold)
                {
                    if (Console.IsActive) Console.Write("rot mag:" + (Mathf.Abs((initialRotateCenter - centerPos).magnitude)), LogTypes.Errors, true, "cyan");

                    Vector2 deltaCenterPos = centerPos - lastProcessedRotateCenter;

                    if (startThreshold != 0)
                    {
                        deltaCenterPos = deltaCenterPos - Vector2.ClampMagnitude(deltaCenterPos, startThreshold);
                    }

                    if (Mathf.Abs(deltaCenterPos.magnitude) > THRESHOLD_TOUCH_TURN)
                    {
                        if (Console.IsActive) Console.Write("rot mag2:" + (deltaCenterPos.magnitude), LogTypes.Errors, true, "cyan");
                        // Rotate!

                        float turnX = deltaCenterPos.y * -SENSITIVITY_TOUCH_TURN;
                        turnX = CamClampRotation(turnX);
                        Camera.Rotate(turnX, 0, 0);

                        float turnY = deltaCenterPos.x * -SENSITIVITY_TOUCH_TURN;
                        transform.Rotate(0, 0, turnY);

                        if ((turnX != 0) || (turnY != 0)) WhenViewChanged();

                        //TODO: some of that code above is redundant code with the mouse handling code, might make sense to move to a function 

                        lastProcessedRotateCenter = centerPos;

                        // Turn off zooming until it passes it's start threshold again
                        initialPinchMagnitude = lastProcessedPinchMagnitude;
                    }
                }
            }
        }
        else if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved) {
            // TODO: Needs to scale with zoom level -- hmmm. current speed is good for zoom out, too fast for zoom all the way in
            // TODO: do we need a threshold, for moves or for momentum specifically? hmm
            Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
            // Adjust sensitivity based on zoom level so the view always moves with your finger
            // That means the view moves more when zoomed out than when zoomed in for the same physical movement
            // TODO: may be better to do this by just figuring out the coordinates in world space the current position and last position of the finger represent, using the vecotr between them? This is probably faster though
            float sensitivity = Mathf.Lerp(SENSITIVITY_TOUCH_MOVE / 5, SENSITIVITY_TOUCH_MOVE, (transform.position.y - MIN_HEIGHT) / (MAX_HEIGHT - MIN_HEIGHT));
            //TODO: new constant for low end of sensitivity
            deltaPosition = deltaPosition * -sensitivity;


            totalTouchMoveDuration += Time.deltaTime;
            totalTouchMove += deltaPosition;
            // don't use momentum on time <  .5s or distance < ~10px
            if (Console.IsActive) Console.Write("totaltouchmagnitude:" + (totalTouchMove.magnitude / Screen.dpi), LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in

            if (totalTouchMoveDuration > .5 && (totalTouchMove.magnitude/Screen.dpi) > .02) //TODO: make constant?
            {
                panningMomentum = totalTouchMove / totalTouchMoveDuration;
            }
            else {
                panningMomentum = Vector2.zero;
            }

            float x = deltaPosition.x;
            float y = deltaPosition.y;

            if ((x != 0) || (y != 0)) WhenViewChanged();
            transform.Translate(x, y, 0);

        }
        else if (Input.touchCount == 0 && panningMomentum.magnitude > .5) {
            // Keep panning with momentum

            panningMomentum *= Mathf.Pow(FRICTION_TOUCH_MOVE_MOMENTUM, Time.deltaTime);
            Console.Write("momentum:" + panningMomentum.magnitude, LogTypes.Errors, true, "cyan"); //TODO: remove logs when things are dialed in

            float x = panningMomentum.x * Time.deltaTime;
            float y = panningMomentum.y * Time.deltaTime;

            if ((x != 0) || (y != 0)) WhenViewChanged();
            transform.Translate(x, y, 0);

        }
        else if (Input.touchCount > 2 && Input.GetTouch(2).phase == TouchPhase.Ended) {
            // TODO: this is mostly for debugging, will probably remove. we do probably need a close button at least for the console on mobile though
            Console.IsActive = !Console.IsActive;
        }

        if (Input.touchCount < 2) {
            initialPinchMagnitude = 0f;
            lastProcessedPinchMagnitude = 0f;

            initialRotateCenter = Vector2.zero;
            lastProcessedRotateCenter = initialRotateCenter;

            totalTouchMoveDuration = 0f;
            totalTouchMove = Vector2.zero; 
        }
    }

}
