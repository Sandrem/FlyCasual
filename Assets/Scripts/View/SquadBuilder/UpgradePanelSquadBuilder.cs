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

        if (IsSlotImage())
        {
            SetSlotImage();
        }
        else
        {
            textureCacheKey = TEXTURENAME + Upgrade.ImageUrl;
            LoadTooltipImage(gameObject, Upgrade.ImageUrl);
            if (ShowFromModInfo) SetFromModeName();
        }

        SetOnClickHandler();
    }

    private bool IsSlotImage()
    {
        return UpgradeName.StartsWith("Slot:");
    }

    private void SetSlotImage()
    {
        string slotTypeName = UpgradeName.Substring(5, UpgradeName.Length - 5);
        string editionName = (Edition.Current is FirstEdition) ? "FirstEdition" : "SecondEdition";
        Sprite sprite = (Sprite)Resources.Load("Sprites/SquadBuiler/UpgradeSlots/" + editionName + "/" + slotTypeName, typeof(Sprite));
        this.gameObject.transform.Find("UpgradeImage").GetComponent<Image>().sprite = sprite;

        this.gameObject.SetActive(true);
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!SquadBuilder.TextureCache.ContainsKey(textureCacheKey))
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
            SetObjectSprite(thisGameObject.transform.Find("UpgradeImage").gameObject, SquadBuilder.TextureCache[textureCacheKey], true);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture, bool textureIsScaled)
    {
        if (!textureIsScaled) TextureScale.Bilinear(newTexture, (int)Edition.Current.UpgradeCardSize.x, (int)Edition.Current.UpgradeCardSize.y);
        if (!SquadBuilder.TextureCache.ContainsKey(textureCacheKey)) SquadBuilder.TextureCache.Add(textureCacheKey, newTexture);
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

        FinallyShow();
    }

    private void ShowTextVersionOfCard()
    {
        try
        {
            this.transform.Find("UpgradeInfo").GetComponent<Text>().text = UpgradeName;
            if (Edition.Current is FirstEdition) this.transform.Find("CostInfo").GetComponent<Text>().text = Upgrade.UpgradeInfo.Cost.ToString();

            FinallyShow();
        }
        catch { }
    }

    private void SetFromModeName()
    {
        Text modText = this.transform.Find("FromModInfo").GetComponent<Text>();
        Text costText = this.transform.Find("CostInfo").GetComponent<Text>();

        if (Edition.Current is FirstEdition)
        {
            if (Upgrade.FromMod != null)
            {
                Mod mod = (Mod)Activator.CreateInstance(Upgrade.FromMod);
                this.transform.Find("FromModInfo").GetComponent<Text>().text = mod.Name;
            }
        }
        else if (Edition.Current is SecondEdition)
        {
            if (Upgrade.FromMod != null)
            {
                Mod mod = (Mod)Activator.CreateInstance(Upgrade.FromMod);
                modText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(418, 0);
                modText.text = mod.Name;
            }

            costText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(418, 0);
            costText.alignment = TextAnchor.MiddleRight;
            costText.fontSize = 50;
            costText.text = Upgrade.UpgradeInfo.Cost.ToString();
        }
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

    private void FinallyShow()
    {
        if (this.gameObject != null) this.gameObject.SetActive(true);
        WaitingToLoad--;

        if (WaitingToLoad == 0  && !IsSlotImage())
        {
            GameObject loadingText = GameObject.Find("UI/Panels/SelectUpgradePanel/LoadingText");
            if (loadingText != null) loadingText.SetActive(false);
        }
    }
}
