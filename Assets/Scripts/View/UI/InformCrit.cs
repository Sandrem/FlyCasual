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

    public static void LoadAndShow(object sender, System.EventArgs e)
    {
        if (Roster.Player1.GetType() == Roster.Player2.GetType() && Roster.Player1.UsesHotacAiRules)
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
        WWW www = ImageManager.GetImage(url);
        yield return www;

        //TODO: add exception handler here
        if (www.error == null)
        {
            SetImageFromWeb(InformCritPanel.Find("CritCardImage").gameObject, www);
        }
        else
        {
            SetTextInfo();
        }

        ShowPanel();
    }

    private static void SetImageFromWeb(GameObject targetObject, WWW www)
    {
        Texture2D newTexture = new Texture2D(www.texture.height, www.texture.width);
        www.LoadImageIntoTexture(newTexture);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
        targetObject.transform.Find("TextInfo").GetComponent<Text>().text = "";
    }

    private static void SetTextInfo()
    {
        InformCritPanel.Find("CritCardImage").GetComponent<Image>().sprite = null;
        InformCritPanel.Find("CritCardImage").Find("TextInfo").GetComponent<Text>().text = Combat.CurrentCriticalHitCard.Name;
    }

    private static void ShowPanel()
    {
        GameMode.CurrentGameMode.ShowInformCritPanel();
    }

    public static void ShowPanelVisible()
    {
        Phases.CurrentSubPhase.Pause();

        InformCritPanel.gameObject.SetActive(true);
        InformCritPanel.Find("Confirm").gameObject.SetActive(true);
    }

    public static void ButtonConfirm()
    {
        GameMode.CurrentGameMode.ConfirmCrit();
    }

    public static void ConfirmCrit()
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;
        Triggers.FinishTrigger();
    }

    public static void HidePanel()
    {
        InformCritPanel.gameObject.SetActive(false);
        Phases.CurrentSubPhase.Resume();
    }

    public static void DisableConfirmButton()
    {
        InformCritPanel.Find("Confirm").gameObject.SetActive(false);
    }

}
