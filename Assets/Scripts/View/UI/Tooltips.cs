using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate string TooltipImageDelegate(GameObject panel);

public static class Tooltips {

    private static bool TooltipIsCalled;
    private static float TooltipActivationSchedule;
    private static readonly float TooltipActivationDelay = 1f;
    private static bool TooltipImageReady;

    private static MonoBehaviour Behavior;
    private static Transform TooltipsPanel;

    static Tooltips()
    {
        //TEMPORARY
        if (GameObject.Find("Global") == null)
        {
            Behavior = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }
        else
        {
            Behavior = GameObject.Find("Global").GetComponent<Global>();
        }
    }

    public static void StartTooltip(TooltipImageDelegate tooltipImageDelegate, GameObject sender)
    {
        TooltipsPanel = GameObject.Find("UI").transform.Find("TooltipPanel").transform;
        string url = tooltipImageDelegate.Invoke(sender);

        if (url != null)
        {
            TooltipIsCalled = true;
            TooltipActivationSchedule = Time.time + TooltipActivationDelay;
            TooltipImageReady = false;

            Behavior.StartCoroutine(LoadTooltipImage(url));
        }
    }

    public static void StartTooltip(string tooltipUrl, GameObject sender)
    {
        TooltipsPanel = GameObject.Find("UI").transform.Find("TooltipPanel").transform;
        string url = tooltipUrl;

        if (url != null)
        {
            TooltipIsCalled = true;
            TooltipActivationSchedule = Time.time + TooltipActivationDelay;
            TooltipImageReady = false;

            Behavior.StartCoroutine(LoadTooltipImage(url));
        }
    }

    private static IEnumerator LoadTooltipImage(string url)
    {
        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (TooltipsPanel != null && www.texture != null)
        {
            SetImageFromWeb(TooltipsPanel.Find("TooltipImage").gameObject, www);
            AdaptTooltipWindowSize();

            TooltipImageReady = true;
        }
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
        float width = TooltipsPanel.Find("TooltipImage").GetComponent<Image>().sprite.rect.width;
        float height = TooltipsPanel.Find("TooltipImage").GetComponent<Image>().sprite.rect.height;
        TooltipsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 10, height + 10);
        TooltipsPanel.Find("TooltipImage").GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
    }

    private static void SetFixedWindowPosition()
    {
        float width = TooltipsPanel.GetComponent<RectTransform>().sizeDelta.x;
        float height = TooltipsPanel.GetComponent<RectTransform>().sizeDelta.y;

        float positionX = Input.mousePosition.x + 5;
        float positionY = Input.mousePosition.y - 5;

        if (positionY < height) positionY = height + 5;
        if ((positionX + width) > Screen.width) positionX = positionX - width - 10;

        TooltipsPanel.position = new Vector2(positionX, positionY);
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
        TooltipsPanel.gameObject.SetActive(true);
    }

    public static void EndTooltip()
    {
        TooltipIsCalled = false;
        if (TooltipsPanel != null) TooltipsPanel.gameObject.SetActive(false);
    }

    public static void AddTooltip(GameObject sender, TooltipImageDelegate tooltipImageDelegate)
    {
        sender.AddComponent<EventTrigger>();
        EventTrigger trigger = sender.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { StartTooltip(tooltipImageDelegate, sender); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { EndTooltip(); });
        trigger.triggers.Add(entry);
    }

    public static void AddTooltip(GameObject sender, string tooltipUrl)
    {
        EventTrigger trigger = sender.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            sender.AddComponent<EventTrigger>();
            trigger = sender.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { StartTooltip(tooltipUrl, sender); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { EndTooltip(); });
        trigger.triggers.Add(entry);
    }

}
