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

    private const float TARGET_TOOLTIP_HEIGHT = .95F;
    private const float TARGET_TOOLTIP_WIDTH = .75F;

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

            LoadTooltipImage(url);
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

            LoadTooltipImage(url);
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
                    SetObjectSprite(TooltipsPanel.Find("TooltipImage").gameObject, texture);
                }
            }, url));
        }
        else
        {
            SetObjectSprite(TooltipsPanel.Find("TooltipImage").gameObject, SquadBuilder.TextureCache[url]);
        }
    }

    private static void SetObjectSprite(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero, 100, 0, SpriteMeshType.Tight, Vector4.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
        SetSpriteScaleForWindow();
        TooltipImageReady = true;
    }

    private static void SetSpriteScaleForWindow()
    {
        float targetHeight = Screen.height * TARGET_TOOLTIP_HEIGHT;
        float targetWidth = Screen.width * TARGET_TOOLTIP_WIDTH;
        float width = TooltipsPanel.Find("TooltipImage").GetComponent<Image>().sprite.rect.width;
        float height = TooltipsPanel.Find("TooltipImage").GetComponent<Image>().sprite.rect.height;
        float scale = 1;
        if (height > targetHeight)
        {
            scale = targetHeight / height;
        }
        else if (width > targetWidth)
        {
            scale = targetWidth / width;
        }
        TooltipsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(width * scale, height * scale);
        TooltipsPanel.Find("TooltipImage").GetComponent<RectTransform>().sizeDelta = new Vector2(width * scale, height * scale);
    }

    private static void SetTooltipPosition()
    {
        RectTransform rect = TooltipsPanel.GetComponent<RectTransform>();
        float height = rect.sizeDelta.y;

        float positionX = Input.mousePosition.x;
        float positionY = Input.mousePosition.y;

        bool mouseIsLeft = positionX < (Screen.width / 2);

        if (mouseIsLeft) //Move tooltip to right of mouse cursor
        {
            positionX += 5;
            rect.pivot = new Vector2(0f, .5f);
        }
        else //Move tooltip to left of mouse cursor
        {
            positionX -= 10;
            rect.pivot = new Vector2(1f, .5f);
        }

        //Determine pivot y-value, .5 y-value means directly in the middle, 0 y-value cursor is on the bottom of tooltip, 1 y-value cursor is on top of the tooltip
        float spaceTop = Screen.height - positionY;
        float spaceBottom = positionY;
        float spaceNeededForHalf = height / 2;

        if (spaceTop < spaceNeededForHalf) //lower image
        {
            float spaceNeeded = spaceNeededForHalf - spaceTop;
            float percentNeeded = (spaceNeeded / height) + .06f;
            rect.pivot = new Vector2(rect.pivot.x, rect.pivot.y + percentNeeded);
        }
        else if (spaceBottom < spaceNeededForHalf) //raise image
        {
            float spaceNeeded = spaceNeededForHalf - spaceBottom;
            float percentNeeded = (spaceNeeded / height) + .06f;
            rect.pivot = new Vector2(rect.pivot.x, rect.pivot.y - percentNeeded);
        }
        TooltipsPanel.transform.position = new Vector3(positionX, positionY);
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
        entry.callback.AddListener((data) => StartTooltip(tooltipUrl, sender));
    }

}
