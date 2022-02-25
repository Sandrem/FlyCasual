using Editions;
using Mods;
using Ship;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Upgrade;

public class PilotPanelSquadBuilder : MonoBehaviour {

    private GenericShip Ship;
    private Action<GenericShip> OnClick;
    private bool ShowFromModInfo;

    public static int WaitingToLoad = 0;
    public static List<PilotPanelSquadBuilder> AllLoadingPanels = new List<PilotPanelSquadBuilder>();

    public Material GrayscaleMaterial;

    public void Initialize(GenericShip ship, Action<GenericShip> onClick = null, bool showFromModInfo = false)
    {
        Ship = ship;
        OnClick = onClick;
        ShowFromModInfo = showFromModInfo;
    }

    void Start()
    {
        this.gameObject.SetActive(false);

        WaitingToLoad++;
        AllLoadingPanels.Add(this);

        if (Ship.ImageUrl != null)
        {
            LoadTooltipImage(this.gameObject, Ship.ImageUrl);
        }
        else
        {
            ShowTextVersionOfCard();
        }

        if (ShowFromModInfo) SetFromModeName();

        if (Ship.IsWIP)
        {
            this.gameObject.transform.Find("PilotImage").GetComponent<Image>().material = GrayscaleMaterial;
            this.gameObject.transform.Find("WIPInfo").gameObject.SetActive(true);
        }
        else if ((Ship.PilotInfo as PilotCardInfo25).LegalityInfo.Contains(Content.Legality.StandartBanned))
        {
            this.gameObject.transform.Find("PilotImage").GetComponent<Image>().material = GrayscaleMaterial;
            this.gameObject.transform.Find("BannedInfo").gameObject.SetActive(true);
        }
        else
        {
            SetOnClickHandler();
        }
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!TextureCache.Cache.ContainsKey(url))
        {
            Global.Instance.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (texture != null && thisGameObject != null)
                {
                    if (!TextureCache.Cache.ContainsKey(url)) TextureCache.Cache.Add(url, texture);
                    SetObjectSprite(thisGameObject.transform.Find("PilotImage").gameObject, texture);
                }
                else
                {
                    ShowTextVersionOfCard();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(thisGameObject.transform.Find("PilotImage").gameObject, TextureCache.Cache[url]);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        ReadyToShow();
    }

    private void ShowTextVersionOfCard()
    {
        if (this == null) return;

        this.transform.Find("PilotName").GetComponent<Text>().text = Ship.PilotInfo.PilotName;
        this.transform.Find("PilotSkill").GetComponent<Text>().text = Ship.PilotInfo.Initiative.ToString();
        this.transform.Find("PilotAbility").GetComponent<Text>().text = Ship.PilotInfo.AbilityText + "\n\n" + Ship.ShipInfo.AbilityText;

        ReadyToShow();
    }

    private void ReadyToShow()
    {
        WaitingToLoad--;

        if (WaitingToLoad == 0) ShowAllLoadedPanels();
    }

    private void ShowAllLoadedPanels()
    {
        foreach (PilotPanelSquadBuilder loadingPanel in AllLoadingPanels)
        {
            loadingPanel.FinallyShow();
        }
        AllLoadingPanels.Clear();

        GameObject loadingText = GameObject.Find("UI/Panels/SelectPilotPanel/LoadingText");
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

    private void SetFromModeName()
    {
        if (Ship.RequiredMods.Count != 0)
        {
            Text infoText = this.transform.Find("FromModInfo").GetComponent<Text>();

            Mod mod = (Mod)Activator.CreateInstance(Ship.RequiredMods[0]);
            string postfix = (Ship.RequiredMods.Count > 1) ? " + ..." : "";
            infoText.text = mod.Name + postfix;
        }

        if (Edition.Current is SecondEdition)
        {
            this.transform.Find("FromModInfo").GetComponent<RectTransform>().localPosition += new Vector3(0, -30, 0);

            Text SeCostText = this.transform.Find("SeCostInfo").GetComponent<Text>();
            SeCostText.text = Ship.PilotInfo.Cost.ToString() + "\n" + (Ship.PilotInfo as PilotCardInfo25).LoadoutValue.ToString();

            // Show extra icons (that not present on all pilots of this ship)
            Text slotsText = this.transform.Find("SlotsInfo").GetComponent<Text>();
            for (int i = 0; i < CountUpgradeIcons(UpgradeType.Talent); i++) slotsText.text += "E";
            for (int i = 0; i < CountUpgradeIcons(UpgradeType.ForcePower); i++) slotsText.text += "F";

            if (Ship.Faction != Faction.Scum) for (int i = 0; i < CountUpgradeIcons(UpgradeType.Illicit); i++) slotsText.text += "I";

            if (Ship.Faction != Faction.FirstOrder && Ship.Faction != Faction.Resistance) for (int i = 0; i < CountUpgradeIcons(UpgradeType.Tech); i++) slotsText.text += "X";
        }
    }

    private int CountUpgradeIcons(UpgradeType upgradeType)
    {
        return Ship.ShipInfo.UpgradeIcons.Upgrades.Count(n => n == upgradeType) + Ship.PilotInfo.ExtraUpgrades.Count(n => n == upgradeType);
    }

    private void SetOnClickHandler()
    {
        if (OnClick != null)
        {
            EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { OnClick(Ship); });
            trigger.triggers.Add(entry);

            ScrollRect mainScroll = GameObject.Find("UI/Panels/SelectPilotPanel/Panel/Scroll View").GetComponent<ScrollRect>();
            FixScrollRect fixScrollRect = this.gameObject.AddComponent<FixScrollRect>();
            fixScrollRect.MainScroll = mainScroll;
        }
    }

}
