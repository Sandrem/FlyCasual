using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MessageType
{
    Error,
    Info
}

public static class Messages{

    private static GameManagerScript Game;

    private static List<GameObject> allMessages = new List<GameObject>();

    static Messages()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void ShowError(string text)
    {
        ShowMessage(text, MessageType.Error);
    }

    //Double
    public static void ShowInfo(string text)
    {
        ShowMessage(text, MessageType.Info);
    }

    private static void ShowMessage(string text, MessageType type)
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

        GameObject Message = MonoBehaviour.Instantiate(Game.PrefabsList.MessagePanel, startingPosition, Game.PrefabsList.MessagePanelsHolder.transform.rotation, Game.PrefabsList.MessagePanelsHolder.transform);
        Message.GetComponent<MessageContainer>().Initialize(text, type);
        allMessages.Add(Message);
    }

}
