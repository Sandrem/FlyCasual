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

    private GameManagerScript Game;
    private Transform Camera;

    private const float SENSITIVITY_MOVE = 0.125f;
    private const float SENSITIVITY_TURN = 5;
    private const float SENSITIVITY_ZOOM = 5;

    private const float BORDER_SQUARE = 8f;
    private const float MAX_HEIGHT = 6f;
    private const float MIN_HEIGHT = 1.5f;
    private const float MAX_ROTATION = 80f;
    private const float MIN_ROTATION = 0f;

    // Use this for initialization
    void Start() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Camera = transform.Find("Main Camera");
    }

    // Update is called once per frame
    void Update() {
        //TODO: Call hide context menu only once
        CamMoveByAxis();
        CamZoomByMouseScroll();
        CamRotateByMouse();
        CamClampPosition();
    }

    // Movement, Rotation, Zoom

    private void CamMoveByAxis() {
        float x = Input.GetAxis("Horizontal") * SENSITIVITY_MOVE;
        float y = Input.GetAxis("Vertical") * SENSITIVITY_MOVE;
        if ((x != 0) || (y != 0)) WhenViewChanged();
        transform.Translate (x, y, 0);
	}

	private void CamZoomByMouseScroll() {
		float zoom = Input.GetAxis ("Mouse ScrollWheel") * SENSITIVITY_ZOOM;
		if (zoom != 0) {
			Vector3 newPosition = transform.position + (Camera.TransformDirection(0, 0, zoom));
			float zoomClampRate = 1;
			if (newPosition.y <= MIN_HEIGHT) {
				zoomClampRate = (transform.position.y - MIN_HEIGHT) / zoom;
			}
            if (newPosition.y >= MAX_HEIGHT)
            {
                zoomClampRate = (transform.position.y - MAX_HEIGHT) / zoom;
            }
            transform.Translate (transform.InverseTransformDirection (Camera.TransformDirection (0, 0, zoom * zoomClampRate)));

            WhenViewChanged();
        }	
	}

	private void CamRotateByMouse() {
		if (Input.GetKey(KeyCode.Mouse1)) {
			
			float turnX = Input.GetAxis ("Mouse Y") * -SENSITIVITY_TURN;
			turnX = CamClampRotation (turnX);
			Camera.Rotate (turnX, 0, 0);

			float turnY = Input.GetAxis ("Mouse X")  * -SENSITIVITY_TURN;
			transform.Rotate (0, 0, turnY);

            if ((turnX != 0) || (turnY != 0)) WhenViewChanged();
        }
	}

    // Restrictions for movement and rotation 

    private float CamClampRotation(float turnX) {
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

	private void CamClampPosition() {
		transform.position = new Vector3 (
			Mathf.Clamp(transform.position.x, -BORDER_SQUARE, BORDER_SQUARE),
			Mathf.Clamp(transform.position.y, MIN_HEIGHT, MAX_HEIGHT),
			Mathf.Clamp(transform.position.z, -BORDER_SQUARE, BORDER_SQUARE)
		);
	}

    // What to do when view is changed

    private void WhenViewChanged() {
        HideTemporaryMenus();
    }

    private void HideTemporaryMenus() {
        Game.UI.HideTemporaryMenus();
    }

}
