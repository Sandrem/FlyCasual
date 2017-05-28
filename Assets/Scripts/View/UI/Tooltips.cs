using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public static class Tooltips {

    private static GameManagerScript Game;

    private static bool TooltipIsCalled;
    private static float TooltipActivationSchedule;
    private static readonly float TooltipActivationDelay = 1f;
    private static bool TooltipImageReady;

    static Tooltips()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void StartTooltip(string url)
    {
        if (url != null)
        {
            TooltipIsCalled = true;
            TooltipActivationSchedule = Time.time + TooltipActivationDelay;
            TooltipImageReady = false;
            Game.StartCoroutine(LoadTooltipImage(url));
        }
    }

    private static IEnumerator LoadTooltipImage(string url)
    {
        WWW www = new WWW(url);
        yield return www;

        SetImageFromWeb(Game.PrefabsList.TooltipPanel.transform.Find("TooltipImage").gameObject, www);
        AdaptTooltipWindowSize();

        TooltipImageReady = true;
    }

    private static void SetImageFromWeb(GameObject targetObject, WWW www)
    {
        Texture2D newTexture = new Texture2D(www.texture.height, www.texture.width);
        www.LoadImageIntoTexture(newTexture);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
    }

    private static void AdaptTooltipWindowSize()
    {
        float width = Game.PrefabsList.TooltipPanel.transform.Find("TooltipImage").GetComponent<Image>().sprite.rect.width;
        float height = Game.PrefabsList.TooltipPanel.transform.Find("TooltipImage").GetComponent<Image>().sprite.rect.height;
        Game.PrefabsList.TooltipPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 10, height + 10);
        Game.PrefabsList.TooltipPanel.transform.Find("TooltipImage").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }

    private static void SetFixedWindowPosition()
    {
        float width = Game.PrefabsList.TooltipPanel.GetComponent<RectTransform>().sizeDelta.x;
        float height = Game.PrefabsList.TooltipPanel.GetComponent<RectTransform>().sizeDelta.y;

        float positionX = Input.mousePosition.x + 5;
        float positionY = Input.mousePosition.y - 5;

        if (positionY < height) positionY = height + 5;
        if ((positionX + width) > Screen.width) positionX = positionX - width - 10;

        Game.PrefabsList.TooltipPanel.transform.position = new Vector2(positionX, positionY);
    }

    public static void CheckTooltip()
    {
        if (TooltipIsCalled)
        {
            if (Time.time > TooltipActivationSchedule)
            {
                if (TooltipImageReady)
                {
                    ShowTooltip();
                }
            }
        }
    }

    private static void ShowTooltip()
    {
        SetFixedWindowPosition();
        Game.PrefabsList.TooltipPanel.SetActive(true);
    }

    public static void EndTooltip()
    {
        TooltipIsCalled = false;
        Game.PrefabsList.TooltipPanel.SetActive(false);
    }

    public static void AddTooltip(GameObject uiElement, string imageUrl)
    {
        uiElement.AddComponent<EventTrigger>();
        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { Game.UI.CallShowTooltip(imageUrl); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { Game.UI.CallHideTooltip(); });
        trigger.triggers.Add(entry);
    }

}
