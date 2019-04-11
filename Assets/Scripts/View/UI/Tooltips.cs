using SquadBuilderNS;
using System.Linq;
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
    private static Transform ImagePanel;

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

    public static void StartTooltip(GameObject sender, TooltipImageDelegate tooltipImageDelegate)
    {
        StartTooltip(sender, tooltipImageDelegate.Invoke(sender));
    }

    public static void StartTooltip(GameObject sender, string tooltipUrl)
    {
        TooltipsPanel = GameObject.Find("UI").transform.Find("TooltipPanel").transform;

        if (tooltipUrl != null)
        {
            TooltipIsCalled = true;
            TooltipActivationSchedule = Time.time + TooltipActivationDelay;
            TooltipImageReady = false;

            LoadTooltipImage(tooltipUrl);
        }
    }

    private static void LoadTooltipImage(string url)
    {
        if (!SquadBuilder.TextureCache.ContainsKey(url))
        {
            Behavior.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (texture != null)
                {
                    if (!SquadBuilder.TextureCache.ContainsKey(url)) SquadBuilder.TextureCache.Add(url, texture);
                    SetTooltipTexture(texture);
                }
            }, url));
        }
        else
        {
            SetTooltipTexture(SquadBuilder.TextureCache[url]);
        }
    }

    private static void SetTooltipTexture(Texture2D newTexture)
    {
        Sprite sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero, 100, 0, SpriteMeshType.Tight, Vector4.zero);
        string panelName = GetNameOfImagePanelBySize(sprite);
        ImagePanel = TooltipsPanel.Find(panelName);
        ImagePanel.GetComponent<Image>().sprite = sprite;
        PrepareImagePanel();
        SetSpriteScaleForWindow();
        TooltipImageReady = true;
    }

    private static void PrepareImagePanel()
    {
        foreach (Transform transform in TooltipsPanel)
        {
            transform.gameObject.SetActive(false);
        }
        ImagePanel.gameObject.SetActive(true);
    }

    private static void SetSpriteScaleForWindow()
    {
        float targetHeight = Screen.height * 0.9f;
        float targetWidth = Screen.width * 0.9f;
        float width = ImagePanel.GetComponent<Image>().sprite.rect.width;
        float height = ImagePanel.GetComponent<Image>().sprite.rect.height;
        float scaleX = 1, scaleY = 1, scale = 1;
        if (height > targetHeight)
        {
            scaleX = targetHeight / height;
        }
        else if (width > targetWidth)
        {
            scaleY = targetWidth / width;
        }
        scale = Mathf.Min(scaleX, scaleY);
        TooltipsPanel.GetComponent<RectTransform>().localScale = new Vector2(scale, scale);
    }

    private static void SetTooltipPosition()
    {
        float uiScale = GameObject.Find("UI").transform.localScale.x;

        RectTransform imageRect = ImagePanel.GetComponent<RectTransform>();
        float imageWidth = imageRect.sizeDelta.x * uiScale;
        float imageHeight = imageRect.sizeDelta.y * uiScale;

        float mousePositionX = Input.mousePosition.x;
        float mousePositionY = Input.mousePosition.y;

        float windowPositionX = 0;
        float windowPositionY = 0;

        if (mousePositionX + imageWidth + 25f > Screen.width)
        {
            windowPositionX = mousePositionX - imageWidth - 25f;
        }
        else
        {
            windowPositionX = mousePositionX + 25f;
        }

        if ((Screen.height - mousePositionY) + imageHeight > Screen.height)
        {
            windowPositionY = imageHeight;
        }
        else
        {
            windowPositionY = mousePositionY;
        }

        TooltipsPanel.transform.position = new Vector3(windowPositionX, windowPositionY);
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
        SetTooltipPosition();
        TooltipsPanel.gameObject.SetActive(true);
    }

    public static void EndTooltip()
    {
        TooltipIsCalled = false;
        if (TooltipsPanel != null) TooltipsPanel.gameObject.SetActive(false);
    }

    public static void AddTooltip(TooltipImageDelegate tooltipImageDelegate, GameObject sender)
    {
        AddTooltip(sender, tooltipImageDelegate.Invoke(sender));
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
        entry.callback.AddListener((data) => { StartTooltip(sender, tooltipUrl); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { EndTooltip(); });
        trigger.triggers.Add(entry);
    }

    public static void ReplaceTooltip(GameObject sender, string tooltipUrl)
    {
        EventTrigger trigger = sender.GetComponent<EventTrigger>();
        var entry = trigger.triggers
            .Where(e => e.eventID == EventTriggerType.PointerEnter)
            .FirstOrDefault();
        if(entry == null)
        {
            return;
        }
        entry.callback.RemoveAllListeners();
        entry.callback.AddListener((data) => StartTooltip(sender, tooltipUrl));
    }

    private static string GetNameOfImagePanelBySize(Sprite sprite)
    {
        if (sprite.rect.height > 650)
        {
            return "ImagePilot";
        }
        else if (sprite.rect.width > 650)
        {
            return "ImageUpgradeSE";
        }
        else if (sprite.rect.height > 400 && sprite.rect.width < 310)
        {
            return "ImagePilot";
        }
        else if (sprite.rect.height > 290 && sprite.rect.width < 210)
        {
            return "ImageUpgradeFE";
        }
        else
        {
            return "ImageUpgradeSE";
        }
    }

}
