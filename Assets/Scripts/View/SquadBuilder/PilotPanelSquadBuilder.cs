using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Ship;
using Mods;

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

        LoadImage();
        if (ShowFromModInfo) SetFromModeName();
        SetOnClickHandler();
    }

    private void LoadImage()
    {
        Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, Ship.ImageUrl));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (www.error == null)
        {
            if (thisGameObject != null)
            {
                SetImageFromWeb(thisGameObject.transform.Find("PilotImage").gameObject, www);
            }
        }
        else
        {
            ShowTextVersionOfCard();
        }
    }

    private void SetImageFromWeb(GameObject targetObject, WWW www)
    {
        Texture2D newTexture = new Texture2D(www.texture.height, www.texture.width);
        www.LoadImageIntoTexture(newTexture);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        this.gameObject.SetActive(true);
    }

    private void ShowTextVersionOfCard()
    {
        this.transform.Find("PilotInfo").GetComponent<Text>().text = Ship.PilotName;
        this.transform.Find("CostInfo").GetComponent<Text>().text = Ship.Cost.ToString();

        this.gameObject.SetActive(true);
    }

    private void SetFromModeName()
    {
        if (Ship.RequiredMods.Count != 0)
        {
            Mod mod = (Mod)Activator.CreateInstance(Ship.RequiredMods[0]);
            string postfix = (Ship.RequiredMods.Count > 1) ? " + ..." : "";
            this.transform.Find("FromModInfo").GetComponent<Text>().text = mod.Name + postfix;
        }
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
