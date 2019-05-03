using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using GameModes;
using GameCommands;
using UnityEngine.Networking;
using SquadBuilderNS;

public static class InformCrit
{
    private static Transform InformCritPanel;
    private static MonoBehaviour Behavior;

    private static bool NetworkAnyPlayerConfirmedCrit;

    public static void Initialize()
    {
        InformCritPanel = GameObject.Find("UI").transform.Find("InformCritPanel").transform;
        Behavior = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void LoadAndShow(object sender, System.EventArgs e)
    {
        if (InformCritPanel == null) Initialize();
        NetworkAnyPlayerConfirmedCrit = false;

        LoadTooltipImage(Combat.CurrentCriticalHitCard.ImageUrl);
    }

    private static void LoadTooltipImage(string url)
    {
        InformCritPanel.Find("CritCardImage").gameObject.SetActive(false);

        if (!SquadBuilder.TextureCache.ContainsKey(url))
        {
            Behavior.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (texture != null)
                {
                    if (!SquadBuilder.TextureCache.ContainsKey(url)) SquadBuilder.TextureCache.Add(url, texture);
                    SetObjectSprite(InformCritPanel.Find("CritCardImage").gameObject, texture);
                }
                else
                {
                    SetTextInfo();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(InformCritPanel.Find("CritCardImage").gameObject, SquadBuilder.TextureCache[url]);
        }
    }

    private static void SetObjectSprite(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
        targetObject.transform.Find("TextInfo").GetComponent<Text>().text = "";
        InformCritPanel.Find("CritCardImage").gameObject.SetActive(true);

        ShowPanel();
    }

    private static void SetTextInfo()
    {
        InformCritPanel.Find("CritCardImage").GetComponent<Image>().sprite = null;
        InformCritPanel.Find("CritCardImage").gameObject.SetActive(true);
        InformCritPanel.Find("CritCardImage").Find("TextInfo").GetComponent<Text>().text = Combat.CurrentCriticalHitCard.Name;

        ShowPanel();
    }

    private static void ShowPanel()
    {
        Phases.CurrentSubPhase.IsReadyForCommands = true;
        Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).InformAboutCrit();
    }

    public static void ShowPanelVisible()
    {
        Phases.CurrentSubPhase.Pause();

        InformCritPanel.gameObject.SetActive(true);
    }

    public static void ShowConfirmButton()
    {
        InformCritPanel.Find("Confirm").gameObject.SetActive(true);
    }

    public static void ButtonConfirm()
    {
        DisableConfirmButton();

        GameCommand command = GameController.GenerateGameCommand(
            GameCommandTypes.ConfirmCrit,
            Phases.CurrentSubPhase.GetType()
        );
        GameMode.CurrentGameMode.ExecuteCommand(command);
    }

    public static void ConfirmCrit()
    {
        if (Network.IsNetworkGame && !NetworkAnyPlayerConfirmedCrit)
        {
            NetworkAnyPlayerConfirmedCrit = true;

            Phases.CurrentSubPhase.IsReadyForCommands = true;
            GameController.CheckExistingCommands();
        }
        else
        {
            HidePanel();
            Phases.CurrentSubPhase.IsReadyForCommands = false;
            Triggers.FinishTrigger();
        }
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
