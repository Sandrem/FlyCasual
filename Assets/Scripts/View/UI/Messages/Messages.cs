using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public enum MessageType
{
    Error,
    Info
}

public static class Messages{

    private static List<GameObject> AllMessages = new List<GameObject>();

    private static readonly float FreeSpace = 10f;

    public static void ShowError(string text, bool allowCopies = false)
    {
        ShowMessage(text, MessageType.Error);
    }

    public static void ShowErrorToHuman(string text, bool allowCopies = false)
    {
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
        {
            ShowMessage(text, MessageType.Error, allowCopies);
        }
    }

    public static void ShowInfo(string text, bool allowCopies = false)
    {
        ShowMessage(text, MessageType.Info, allowCopies);
    }

    public static void ShowInfoToHuman(string text, bool allowCopies = false)
    {
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
        {
            ShowMessage(text, MessageType.Info, allowCopies);
        }
    }

    public static void ShowInfoToOpponent(string text, bool allowCopies = false)
    {
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() != typeof(Players.HumanPlayer))
        {
            ShowMessage(text, MessageType.Info, allowCopies);
        }
    }

    private static void ShowMessage(string text, MessageType type, bool allowCopies = false)
    {
        if (!allowCopies)
        {
            if (AllMessages.LastOrDefault() != null && AllMessages.LastOrDefault().name == text) return;
        }

        GameObject prefab = (GameObject)Resources.Load("Prefabs/MessagePanel", typeof(GameObject));
        GameObject MessagePanelsHolder = GameObject.Find("UI/MessagesContainer/MessagePanels");
        GameObject Message = MonoBehaviour.Instantiate(prefab, MessagePanelsHolder.transform);
        float messageHeight = Message.GetComponent<MessageContainer>().Initialize(text, type);

        Vector2 startingPosition = new Vector3(0, -messageHeight, 0);

        if (AllMessages.Count != 0 && AllMessages.Last() != null)
        {
            startingPosition = AllMessages.Last().transform.localPosition + new Vector3(0, -(messageHeight + FreeSpace), 0);
        }

        foreach (var message in AllMessages)
        {
            if (message != null)
            {
                message.GetComponent<MessageContainer>().ShiftTargetPosition(messageHeight);
            }
        }

        Message.transform.localPosition = startingPosition;
        AllMessages.Add(Message);
    }

}
