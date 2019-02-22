using Editions;
using Mods;
using Ship;
using SquadBuilderNS;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Upgrade;

public class PilotPanelSquadBuilder : MonoBehaviour {

    private GenericShip Ship;
    private Action<GenericShip> OnClick;
    private bool ShowFromModInfo;

    public void Initialize(GenericShip ship, Action<GenericShip> onClick = null, bool showFromModInfo = false)
    {
        Ship = ship;
        OnClick = onClick;
        ShowFromModInfo = showFromModInfo;
    }

    void Start()
    {
        this.gameObject.SetActive(false);

        LoadTooltipImage(this.gameObject, Ship.ImageUrl);
        if (ShowFromModInfo) SetFromModeName();
        SetOnClickHandler();
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!SquadBuilder.TextureCache.ContainsKey(url))
        {
            Global.Instance.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (texture != null && thisGameObject != null)
                {
                    if (!SquadBuilder.TextureCache.ContainsKey(url)) SquadBuilder.TextureCache.Add(url, texture);
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
            SetObjectSprite(thisGameObject.transform.Find("PilotImage").gameObject, SquadBuilder.TextureCache[url]);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        this.gameObject.SetActive(true);
    }

    private void ShowTextVersionOfCard()
    {
        if (this == null) return;

        this.transform.Find("PilotInfo").GetComponent<Text>().text = Ship.PilotInfo.PilotName;
        if (Edition.Current is FirstEdition) this.transform.Find("CostInfo").GetComponent<Text>().text = Ship.PilotInfo.Cost.ToString();

        this.gameObject.SetActive(true);
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
            SeCostText.text = Ship.PilotInfo.Cost.ToString();

            // Show extra icons (that not present on all pilots of this ship)
            Text slotsText = this.transform.Find("SlotsInfo").GetComponent<Text>();
            for (int i = 0; i < CountUpgradeIcons(UpgradeType.Talent); i++) slotsText.text += "E";
            for (int i = 0; i < CountUpgradeIcons(UpgradeType.Force); i++) slotsText.text += "F";
            if (Ship is Ship.SecondEdition.YT2400LightFreighter.YT2400LightFreighter) for (int i = 0; i < CountUpgradeIcons(UpgradeType.Crew); i++) slotsText.text += "W";
            if (Ship.Faction != Faction.Scum) for (int i = 0; i < CountUpgradeIcons(UpgradeType.Illicit); i++) slotsText.text += "I";
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
