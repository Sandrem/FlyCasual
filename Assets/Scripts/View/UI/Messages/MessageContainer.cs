using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageContainer : MonoBehaviour {

    private float delaySeconds = 3f;
    private Vector3 targetPosition;
    private const float MOVE_SPEED = 100;
    private bool doomed = false;
    private float preferredHeight = 0;

    // Use this for initialization
    void Start() {
        
    }

    public float Initialize(string text, MessageType type)
    {
        transform.gameObject.name = text;

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

        preferredHeight = transform.Find("MessageText").GetComponent<Text>().preferredHeight;
        if (preferredHeight < 50f) preferredHeight = 50f;
        preferredHeight += 20f;

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(700f, preferredHeight);
        transform.Find("MessageText").GetComponent<RectTransform>().sizeDelta = new Vector2(670f, preferredHeight);

        return preferredHeight;
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

    public void ShiftTargetPosition(float messageHeight)
    {
        targetPosition = targetPosition + new Vector3(0, messageHeight + 10f, 0);
    }
}
