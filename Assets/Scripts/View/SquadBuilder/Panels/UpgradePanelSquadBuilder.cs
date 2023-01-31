using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Upgrade;
using Mods;
using Editions;
using UnityEngine.Networking;

public class UpgradePanelSquadBuilder : MonoBehaviour {

    private string UpgradeName;
    private GenericUpgrade Upgrade;
    private UpgradeSlot Slot;
    private Action<UpgradeSlot, GenericUpgrade> OnClick;
    private const string TEXTURENAME = "UPGRADEPANEL_";
    private string textureCacheKey;
    private bool ShowFromModInfo;
    private bool Compact;

    public static int WaitingToLoad = 0;
    public static List<UpgradePanelSquadBuilder> AllLoadingPanels = new List<UpgradePanelSquadBuilder>();

    public Material GrayscaleMaterial;

    public void Initialize(string upgradeName, UpgradeSlot slot, GenericUpgrade upgrade = null, Action<UpgradeSlot, GenericUpgrade> onClick = null, bool showFromModInfo = false, bool compact = false)
    {
        UpgradeName = upgradeName;
        Upgrade = upgrade;
        OnClick = onClick;
        Slot = slot;
        ShowFromModInfo = showFromModInfo;
        Compact = compact;

        this.gameObject.GetComponent<RectTransform>().sizeDelta = (compact) ? Edition.Current.UpgradeCardCompactSize : Edition.Current.UpgradeCardSize;
    }

    void Start()
    {
        this.gameObject.SetActive(false);

        WaitingToLoad++;
        AllLoadingPanels.Add(this);

        if (IsSlotImage())
        {
            SetSlotImage();
            SetOnClickHandler();
        }
        else
        {
            textureCacheKey = TEXTURENAME + Upgrade.ImageUrl;
            LoadTooltipImage(gameObject, Upgrade.ImageUrl);
            if (ShowFromModInfo) SetFromModeName();

            if (Upgrade.IsWIP)
            {
                this.gameObject.transform.Find("UpgradeImage").GetComponent<Image>().material = GrayscaleMaterial;
                this.gameObject.transform.Find("WIPInfo").gameObject.SetActive(true);
            }
            else if (Content.XWingFormats.IsBanned(Upgrade))
            {
                this.gameObject.transform.Find("UpgradeImage").GetComponent<Image>().material = GrayscaleMaterial;
                this.gameObject.transform.Find("BannedInfo").gameObject.SetActive(true);
            }
            else
            {
                SetOnClickHandler();
            }
        }
    }

    private bool IsSlotImage()
    {
        return UpgradeName.StartsWith("Slot:");
    }

    private void SetSlotImage()
    {
        string slotTypeName = UpgradeName.Substring(5, UpgradeName.Length - 5);
        string editionName = "SecondEdition";
        Sprite sprite = (Sprite)Resources.Load("Sprites/SquadBuiler/UpgradeSlots/" + editionName + "/" + slotTypeName, typeof(Sprite));
        this.gameObject.transform.Find("UpgradeImage").GetComponent<Image>().sprite = sprite;

        ReadyToShow();
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!TextureCache.Cache.ContainsKey(textureCacheKey))
        {
            Global.Instance.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (thisGameObject != null && texture != null)
                {
                    SetObjectSprite(thisGameObject.transform.Find("UpgradeImage").gameObject, texture, false);
                }
                else
                {
                    ShowTextVersionOfCard();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(thisGameObject.transform.Find("UpgradeImage").gameObject, TextureCache.Cache[textureCacheKey], true);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture, bool textureIsScaled)
    {
        if (!textureIsScaled) TextureScale.Bilinear(newTexture, (int)Edition.Current.UpgradeCardSize.x, (int)Edition.Current.UpgradeCardSize.y);
        if (!TextureCache.Cache.ContainsKey(textureCacheKey)) TextureCache.Cache.Add(textureCacheKey, newTexture);
        Sprite newSprite = null;
        if (!Compact)
        {
            newSprite = Sprite.Create(
                newTexture,
                new Rect(0, 0, newTexture.width, newTexture.height),
                Vector2.zero
            );
        }
        else
        {
            newSprite = Sprite.Create(
                newTexture,
                new Rect(
                    (!Upgrade.UpgradeInfo.HasType(UpgradeType.Configuration)) ? Edition.Current.UpgradeCardCompactOffset.x : Edition.Current.UpgradeCardCompactOffset.x - 155,
                    Edition.Current.UpgradeCardCompactOffset.y,
                    Edition.Current.UpgradeCardCompactSize.x,
                    Edition.Current.UpgradeCardCompactSize.y
                ),
                Vector2.zero
            );
        }

        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        ReadyToShow();
    }

    private void ShowTextVersionOfCard()
    {
        try
        {
            this.transform.Find("UpgradeInfo").GetComponent<Text>().text = UpgradeName;

            ReadyToShow();
        }
        catch { }
    }

    private void SetFromModeName()
    {
        Text modText = this.transform.Find("FromModInfo").GetComponent<Text>();
        Text costText = this.transform.Find("CostInfo").GetComponent<Text>();

        if (Upgrade.FromMod != null)
        {
            Mod mod = (Mod)Activator.CreateInstance(Upgrade.FromMod);
            modText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(418, 0);
            modText.transform.localPosition = new Vector3(129, -325, 0);
            modText.text = mod.Name;
        }

        costText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(418, 0);
        costText.alignment = TextAnchor.MiddleRight;
        costText.fontSize = 50;
        costText.text = Upgrade.UpgradeInfo.Cost.ToString();
    }

    private void SetOnClickHandler()
    {
        if (OnClick != null)
        {
            EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { OnClick(Slot, Upgrade); });
            trigger.triggers.Add(entry);

            ScrollRect mainScroll = GameObject.Find("UI/Panels/SelectUpgradePanel/Panel/Scroll View").GetComponent<ScrollRect>();
            FixScrollRect fixScrollRect = this.gameObject.AddComponent<FixScrollRect>();
            fixScrollRect.MainScroll = mainScroll;
        }
    }

    private void ReadyToShow()
    {
        WaitingToLoad--;

        if (WaitingToLoad == 0) ShowAllLoadedPanels();
    }

    private void ShowAllLoadedPanels()
    {
        foreach (UpgradePanelSquadBuilder loadingPanel in AllLoadingPanels)
        {
            loadingPanel.FinallyShow();
        }
        AllLoadingPanels.Clear();

        GameObject loadingText = GameObject.Find("UI/Panels/SelectUpgradePanel/LoadingText");
        if (loadingText != null) loadingText.SetActive(false);
    }

    public void FinallyShow()
    {
        try
        {
            this.gameObject.SetActive(true);
        }
        catch { }
    }
}
