using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    Error,
    Info
}


public class MessageManagerScript : MonoBehaviour{

    public GameObject prefabMessagePanel;
    public GameObject messagePanels;
    private List<GameObject> allMessages = new List<GameObject>();

    public void ShowError(string text)
    {
        ShowMessage(text, MessageType.Error);
    }

    //Double
    public void ShowInfo(string text)
    {
        ShowMessage(text, MessageType.Info);
    }

    private void ShowMessage(string text, MessageType type)
    {

        Vector2 startingPosition = new Vector3(Screen.width / 2, -50, 0);
        foreach (var message in allMessages)
        {
            if (message != null)
            {
                startingPosition = message.transform.position + new Vector3(0, -55, 0);
            }
        }

        foreach (var message in allMessages)
        {
            if (message != null)
            {
                message.GetComponent<MessageContainer>().ShiftTargetPosition();
            }
        }

        GameObject Message = MonoBehaviour.Instantiate(prefabMessagePanel, startingPosition, prefabMessagePanel.transform.rotation, messagePanels.transform);
        Message.GetComponent<MessageContainer>().Initialize(text, type);
        allMessages.Add(Message);
    }

}
