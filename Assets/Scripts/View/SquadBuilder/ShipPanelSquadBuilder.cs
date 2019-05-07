using SquadBuilderNS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipPanelSquadBuilder : MonoBehaviour {

    public string ImageUrl;
    public string ShipName;
    public string FullType;
    private const string TEXTURENAME = "SHIPPANEL_";
    private string textureCacheKey = "";

    public static int WaitingToLoad = 0;
    public static List<ShipPanelSquadBuilder> AllLoadingPanels = new List<ShipPanelSquadBuilder>();

    // Use this for initialization
    void Start()
    {
        this.gameObject.SetActive(false);

        AllLoadingPanels.Add(this);
        WaitingToLoad++;

        textureCacheKey = TEXTURENAME + ImageUrl;
        LoadTooltipImage(gameObject, ImageUrl);
        SetName();
        SetOnClickHandler();
    }

    private void SetName()
    {
        transform.Find("ShipName").GetComponent<Text>().text = FullType;
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!SquadBuilder.TextureCache.ContainsKey(textureCacheKey))
        {
            Global.Instance.StartCoroutine(ImageManager.GetTexture((texture) => 
            {
                if (thisGameObject != null && texture != null)
                {
                    SetObjectSprite(thisGameObject.transform.Find("ShipImage").gameObject, texture, false);
                }
                else
                {
                    ShowTextVersionOfCard();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(thisGameObject.transform.Find("ShipImage").gameObject, SquadBuilder.TextureCache[textureCacheKey], true);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture, bool textureIsScaled)
    {
        if (!textureIsScaled) TextureScale.Bilinear(newTexture, 600, 836);
        if (!SquadBuilder.TextureCache.ContainsKey(textureCacheKey)) SquadBuilder.TextureCache.Add(textureCacheKey, newTexture); //Since this Texture has been modified to fit this particular sprite we should just cache it for the ShipPanel
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, newTexture.height - 248, newTexture.width, 248), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
        
        ReadyToShow();
    }

    private void ShowTextVersionOfCard()
    {
        ReadyToShow();
    }

    private void SetOnClickHandler()
    {
        EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(delegate { SelectShip(ShipName); });
        trigger.triggers.Add(entry);
    }

    private void SelectShip(string shipName)
    {
        SquadBuilder.CurrentShip = shipName;
        MainMenu.CurrentMainMenu.ChangePanel("SelectPilotPanel");
    }

    private void ReadyToShow()
    {
        WaitingToLoad--;

        if (WaitingToLoad == 0) ShowAllLoadedPanels();
    }

    private void ShowAllLoadedPanels()
    {
        foreach (ShipPanelSquadBuilder loadingPanel in AllLoadingPanels)
        {
            loadingPanel.FinallyShow();
        }
        AllLoadingPanels.Clear();

        GameObject loadingText = GameObject.Find("UI/Panels/SelectShipPanel/LoadingText");
        if (loadingText != null) loadingText.SetActive(false);
    }

    public void FinallyShow()
    {
        if (this.gameObject != null) this.gameObject.SetActive(true);
    }
}
