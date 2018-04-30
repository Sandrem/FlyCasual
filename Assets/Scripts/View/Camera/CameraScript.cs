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

    private Transform Camera;

    private const float SENSITIVITY_MOVE = 0.125f;
    private const float SENSITIVITY_TURN = 5;
    private const float SENSITIVITY_ZOOM = 5;
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
    private CameraModes cameraMode = CameraModes.Free;

    // Use this for initialization
    void Start()
    {
        Camera = transform.Find("Main Camera");

        SetDefaultCameraPosition();
    }

    private void SetDefaultCameraPosition()
    {
        bool isSecondPlayer = (Network.IsNetworkGame && !Network.IsServer);

        Camera camera = Camera.GetComponent<Camera>();
        camera.orthographicSize = 6;

        Camera.localEulerAngles = (cameraMode == CameraModes.Free) ? new Vector3(-50, 0, 0) : new Vector3(0, 0, 0);
        transform.localEulerAngles = new Vector3(90, 0, (!isSecondPlayer) ? 0 : 180);
        transform.localPosition = (cameraMode == CameraModes.Free) ? new Vector3(0, 6, (!isSecondPlayer) ? -8 : 8) : Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: Call hide context menu only once
        CheckChangeMode();

        CamMoveByAxis();
        CamMoveByMouse();
        CamZoomByMouseScroll();
        CamRotateByMouse();
        CamClampPosition();
    }

    // CAMERA MODES

    private void CheckChangeMode()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Console.IsActive) ChangeMode();
    }

    private void ChangeMode()
    {
        cameraMode = (cameraMode == CameraModes.Free) ? CameraModes.TopDown : CameraModes.Free;

        Camera camera = Camera.GetComponent<Camera>();
        camera.orthographic = !camera.orthographic;

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

}
