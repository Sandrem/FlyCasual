using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageContainer : MonoBehaviour {

    private float delaySeconds = 3f;
    private Vector3 targetPosition;
    private const float MOVE_SPEED = 100;
    private bool doomed = false;

    // Use this for initialization
    void Start() {
        
    }

    public void Initialize(string text, MessageType type)
    {
        targetPosition = new Vector3(transform.position.x, 5, transform.position.z);
        transform.Find("MessageText").GetComponent<Text>().text = text;
        switch (type)
        {
            case MessageType.Error:
                this.gameObject.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
                break;
            case MessageType.Info:
                this.gameObject.GetComponent<Image>().color = new Color32(0, 0, 0, 255);
                break;
            default:
                break;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        transform.localPosition = transform.localPosition + new Vector3(0, Time.deltaTime * MOVE_SPEED, 0);
        if (transform.localPosition.y > targetPosition.y) transform.localPosition = new Vector3(transform.localPosition.x, targetPosition.y, transform.localPosition.z);

        if ((!doomed) && (transform.localPosition.y > 0))
        {
            PlanSelfDestruction();
            doomed = true;
        }
    }

    private void PlanSelfDestruction()
    {
        MonoBehaviour.Destroy(this.gameObject, delaySeconds);
    }

    public void ShiftTargetPosition()
    {
        targetPosition = targetPosition + new Vector3(0, 85, 0);
    }
}
