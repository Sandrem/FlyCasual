using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    private const float MOUSE_MOVE_START_OFFSET = 50f;
    private const float BORDER_SQUARE = 9f;
    private const float MAX_HEIGHT = 6f;
    private const float MIN_HEIGHT = 1.5f;
    private const float MAX_ROTATION = 89.99f;
    private const float MIN_ROTATION = 0f;

    // Constants for touch controls
    private const float SENSITIVITY_TOUCH_MOVE = 0.010f;
    private const float SENSITIVITY_TOUCH_MOVE_ZOOMED_IN = SENSITIVITY_TOUCH_MOVE / 25f;
    private const float SENSITIVITY_TOUCH_TURN = 0.125f;
    private const float SENSITIVITY_TOUCH_ZOOM = 0.0375f;
    //TODO: need to scale any of the thresholds by DPI? (zoom may already account for that, but the rest?)
    private const float THRESHOLD_TOUCH_MOVE_MOMENTUM = 10f;
    private const float THRESHOLD_TOUCH_TURN = 0.05f;
    private const float THRESHOLD_TOUCH_TURN_SWITCH = 40f;
    private const float THRESHOLD_TOUCH_TURN_START = 20f;
    private const float THRESHOLD_TOUCH_ZOOM = 0.06f;
    private const float THRESHOLD_TOUCH_ZOOM_SWITCH = 30f;
    private const float THRESHOLD_TOUCH_ZOOM_START = 20f;
    private const float FRICTION_TOUCH_MOVE_MOMENTUM = 0.2f;
    private const float MOMENTUM_THRESHOLD = 1500f;
    private const float MOMENTUM_MINIMUM = 0.5f;

    // State for touch controls
    private float initialPinchMagnitude = 0f; // Magnitude of the pinch when 2 fingers are first put on the screen
    private float lastProcessedPinchMagnitude = 0f; // Magnitude of the pinch when we last actually zoomed
    private Vector2 initialRotateCenter = new Vector2(0.0f, 0.0f);
    private Vector2 lastProcessedRotateCenter = new Vector2(0.0f, 0.0f);
    private Vector2 panningMomentum = new Vector2(0.0f, 0.0f);
    private float totalTouchMoveDuration = 0f;
    private Vector2 totalTouchMove = new Vector2(0.0f, 0.0f);

    public static bool InputAxisAreEnabled = true;
    static bool _inputMouseIsEnabled = true;
    public static bool InputMouseIsEnabled
    {
        get { return _inputMouseIsEnabled; }
        set {
            _inputMouseIsEnabled = value;

            // Mouse and touch are exclusive
            if (_inputMouseIsEnabled) {
                _inputTouchIsEnabled = false;
            }
        }
    }
    static bool _inputTouchIsEnabled = false;
    public static bool InputTouchIsEnabled
    {
        get { return _inputTouchIsEnabled; }
        set
        {
            _inputTouchIsEnabled = value;

            // Mouse and touch are exclusive
            if (_inputTouchIsEnabled)
            {
                _inputMouseIsEnabled = false;
            }
        }
    }
    public static bool TouchInputsPaused = false;

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
        GameObjectTransform.localPosition = (cameraMode == CameraModes.Free) ? new Vector3(0, 6, (!isSecondPlayer) ? -9 : 9) : new Vector3(0, 0, (!isSecondPlayer) ? 0.85f: -0.85f);
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Call hide context menu only once
        CheckChangeMode();

        if (InputTouchIsEnabled)
        {
            CamRotateZoomByTouch();
            CamMoveByTouch();
        }

        if (InputMouseIsEnabled)
        {
            CamMoveByMouse();
            CamZoomByMouseScroll();
            CamRotateByMouse();
        }

        if (InputAxisAreEnabled)
        {
            CamMoveByAxis();
        }

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

        float x = Input.GetAxis("Horizontal") * SENSITIVITY_MOVE;
        float y = Input.GetAxis("Vertical") * SENSITIVITY_MOVE;
        if ((x != 0) || (y != 0)) WhenViewChanged();
        transform.Translate (x, y, 0);
	}

    private void CamMoveByMouse()
    {
        float x = 0;
        if (Input.mousePosition.x < MOUSE_MOVE_START_OFFSET && Input.mousePosition.x >= 0) x = -1f * SENSITIVITY_MOVE;
        else if (Input.mousePosition.x > Screen.width - MOUSE_MOVE_START_OFFSET && Input.mousePosition.x <= Screen.width) x = 1f * SENSITIVITY_MOVE;

        float y = 0;
        if (Input.mousePosition.y < MOUSE_MOVE_START_OFFSET && Input.mousePosition.y >= 0) y = -1f * SENSITIVITY_MOVE;
        else if (Input.mousePosition.y > Screen.height - MOUSE_MOVE_START_OFFSET && Input.mousePosition.y <= Screen.height) y = 1f * SENSITIVITY_MOVE;

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

    // Pinch zoom, two finger rotate for touch controls

    void CamRotateZoomByTouch()
    {
        if (Input.touchCount > 0 && (Input.GetTouch(0).position.x > Screen.width ||
                                     Input.GetTouch(0).position.y > Screen.height ||
                                     TouchInputsPaused))
        {
            // Don't listen to touches that are off-screen, or being handled elsewhere
            return;
        }

        // If there are two touches on the device
        if (Input.touchCount == 2 && 
            (Input.GetTouch(0).phase == TouchPhase.Moved ||
             Input.GetTouch(1).phase == TouchPhase.Moved))
        {
            // Store both touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Normalize for DPI
            touchDeltaMag = touchDeltaMag / Screen.dpi * 265;

            // Initialize values when 2 fingers are first touched to the screen
            if (initialPinchMagnitude == 0f)
            {
                initialPinchMagnitude = touchDeltaMag;
                lastProcessedPinchMagnitude = touchDeltaMag;
            }

            float startThreshold = 0;

            if (initialRotateCenter != lastProcessedRotateCenter)
            {
                // A pinch is in progress
                startThreshold = THRESHOLD_TOUCH_ZOOM_SWITCH;
            }
            else if (initialPinchMagnitude == lastProcessedPinchMagnitude)
            {
                // A zoom is not yet in progress
                startThreshold = THRESHOLD_TOUCH_ZOOM_START;
            }

            // Try to pinch zoom if we pass a start threshold
            if (Mathf.Abs(initialPinchMagnitude - touchDeltaMag) > startThreshold)
            {
                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = lastProcessedPinchMagnitude - touchDeltaMag;

                if (startThreshold != 0)
                {
                    deltaMagnitudeDiff = (Mathf.Abs(deltaMagnitudeDiff) - startThreshold) * Mathf.Sign(deltaMagnitudeDiff);
                }

                if (Mathf.Abs(deltaMagnitudeDiff) > THRESHOLD_TOUCH_ZOOM)
                {
                    float zoom = deltaMagnitudeDiff * -SENSITIVITY_TOUCH_ZOOM;
                    ZoomByFactor(zoom);

                    lastProcessedPinchMagnitude = touchDeltaMag;

                    // Turn off rotate for now
                    initialRotateCenter = lastProcessedRotateCenter;
                }
            }

            // Try to rotate by dragging two fingers
            if (cameraMode == CameraModes.Free)
            {
                // Find the difference between the average of the positions
                Vector2 centerPos = Vector2.Lerp(touchZero.position, touchOne.position, 0.5f);

                if (initialRotateCenter.magnitude == 0)
                {
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
                    Vector2 deltaCenterPos = centerPos - lastProcessedRotateCenter;

                    if (startThreshold != 0)
                    {
                        deltaCenterPos = deltaCenterPos - Vector2.ClampMagnitude(deltaCenterPos, startThreshold);
                    }

                    if (Mathf.Abs(deltaCenterPos.magnitude) > THRESHOLD_TOUCH_TURN)
                    {
                        // Rotate!
                        float turnX = deltaCenterPos.y * -SENSITIVITY_TOUCH_TURN;
                        turnX = CamClampRotation(turnX);
                        Camera.Rotate(turnX, 0, 0);

                        float turnY = deltaCenterPos.x * -SENSITIVITY_TOUCH_TURN;
                        transform.Rotate(0, 0, turnY);

                        if ((turnX != 0) || (turnY != 0)) WhenViewChanged();

                        lastProcessedRotateCenter = centerPos;

                        // Turn off zooming until it passes it's start threshold again
                        initialPinchMagnitude = lastProcessedPinchMagnitude;
                    }
                }
            }
        }

        // When gestures aren't in progress, reset values used to track them
        if (Input.touchCount < 2)
        {
            initialPinchMagnitude = 0f;
            lastProcessedPinchMagnitude = 0f;

            initialRotateCenter = Vector2.zero;
            lastProcessedRotateCenter = initialRotateCenter;
        }
    }

    // One finger pan for touch controls

    void CamMoveByTouch()
    {
        if (Input.touchCount > 0 && (Input.GetTouch(0).position.x > Screen.width ||
                                     Input.GetTouch(0).position.y > Screen.height ||
                                     TouchInputsPaused ||
                                     (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId) && totalTouchMoveDuration == 0f)))
        {
            // Don't listen to touches that are off-screen, or being handled elsewhere
            return;
        }

        // Note: in 2D mode we could also do this when 2 fingers are down (and thus a zoom is happening), since rotates can't also happen in 2D mode
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Stop momentum as soon as one finger is touched to the screen
                panningMomentum = Vector2.zero;

                // Setup to start a new move
                totalTouchMoveDuration = 0f;
                totalTouchMove = Vector2.zero;
            }
            else
            {
                // Adjust sensitivity based on zoom level so the view always moves with your finger
                // That means the view moves more when zoomed out than when zoomed in for the same physical movement
                // TODO: could be better to do this by just figuring out the coordinates in world space the current position and last position of the finger represent, using the vector between them? Would probably require an invisible plane just to use for this raycast though
                float moveSensitivityForCurrentZoom = SENSITIVITY_TOUCH_MOVE;
                float zoomPercent = 1;
                if (cameraMode == CameraModes.Free)
                {
                    // +1 to numerator and denominator so it never goes to 0. +1 again to denominator to make Free / TopDown match more 
                    // (since TopDown doesn't go all the way to it's theoretical max zoom out for some reason)
                    zoomPercent = (transform.position.y - MIN_HEIGHT + 1) / (MAX_HEIGHT - MIN_HEIGHT + 1 + 1);

                }
                else if (cameraMode == CameraModes.TopDown)
                {
                    // +1 to numerator and denominator so it never goes to 0
                    zoomPercent = (Camera.GetComponent<Camera>().orthographicSize - 1 + 1) / (6 + 1);
                }
                moveSensitivityForCurrentZoom = Mathf.Min(SENSITIVITY_TOUCH_MOVE,
                                            Mathf.Lerp(SENSITIVITY_TOUCH_MOVE_ZOOMED_IN,
                                                       SENSITIVITY_TOUCH_MOVE,
                                                       zoomPercent));

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
                    deltaPosition = deltaPosition * -moveSensitivityForCurrentZoom;

                    // Add momentum
                    totalTouchMove += deltaPosition;
                   
                    // Move camera
                    float x = deltaPosition.x;
                    float y = deltaPosition.y;

                    if ((x != 0) || (y != 0)) WhenViewChanged();
                    transform.Translate(x, y, 0);
                }

                // Keep incrementing duration while 1 finger is down even if no movement is happening
                totalTouchMoveDuration += Time.deltaTime;

                if (totalTouchMove.magnitude / totalTouchMoveDuration > MOMENTUM_THRESHOLD * moveSensitivityForCurrentZoom)
                {
                    panningMomentum = totalTouchMove / totalTouchMoveDuration;
                }
                else
                {
                    panningMomentum = Vector2.zero;
                }
            }

        }
        else if (Input.touchCount == 0 && panningMomentum.magnitude > MOMENTUM_MINIMUM)
        {
            // Keep panning with momentum
            panningMomentum *= Mathf.Pow(FRICTION_TOUCH_MOVE_MOMENTUM, Time.deltaTime);

            float x = panningMomentum.x * Time.deltaTime;
            float y = panningMomentum.y * Time.deltaTime;

            if ((x != 0) || (y != 0)) WhenViewChanged();
            transform.Translate(x, y, 0);

        }
    }


    // Restrictions for movement and rotation 

    private float CamClampRotation(float turnX)
    {
        float currentTurnX = Camera.eulerAngles.x;
        float newTurnX = turnX + currentTurnX;
        if (newTurnX > MAX_ROTATION)
        {
            turnX = MAX_ROTATION - currentTurnX;
        }
        else if (newTurnX < MIN_ROTATION)
        {
            turnX = MIN_ROTATION - currentTurnX;
        }
        return turnX;
    }

    private void CamClampPosition()
    {
        transform.position = new Vector3(
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

}
