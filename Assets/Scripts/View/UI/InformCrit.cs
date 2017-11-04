using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameModes;

public static class InformCrit
{
    private static Transform InformCritPanel;
    private static MonoBehaviour Behavior;

    public static void Initialize()
    {
        InformCritPanel = GameObject.Find("UI").transform.Find("InformCritPanel").transform;
        Behavior = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void LoadAndShow()
    {
        if (Roster.Player1.GetType() == Roster.Player2.GetType() && Roster.Player1.GetType() == typeof(Players.HotacAiPlayer))
        {
            Triggers.FinishTrigger();
        }
        else
        {
            if (InformCritPanel == null) Initialize();
            Behavior.StartCoroutine(LoadTooltipImage(Combat.CurrentCriticalHitCard.ImageUrl));
        }
    }

    private static IEnumerator LoadTooltipImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        //TODO: add exception handler here
        SetImageFromWeb(InformCritPanel.Find("CritCardImage").gameObject, www);

        ShowPanel();
    }

    private static void SetImageFromWeb(GameObject targetObject, WWW www)
    {
        Texture2D newTexture = new Texture2D(www.texture.height, www.texture.width);
        www.LoadImageIntoTexture(newTexture);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
    }
    
    private static void ShowPanel()
    {
        GameMode.CurrentGameMode.ShowInformCritPanel();
    }

    public static void ShowPanelVisible()
    {
        InformCritPanel.gameObject.SetActive(true);
        InformCritPanel.Find("Confirm").gameObject.SetActive(true);
    }

    public static void ButtonConfirm()
    {
        GameMode.CurrentGameMode.ConfirmCrit();
    }

    public static void HidePanel()
    {
        InformCritPanel.gameObject.SetActive(false);
    }

    public static void DisableConfirmButton()
    {
        InformCritPanel.Find("Confirm").gameObject.SetActive(false);
    }

}
