﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Upgrade;

public class UpgradePanelSquadBuilder : MonoBehaviour {

    private string UpgradeName;
    private GenericUpgrade Upgrade;
    private UpgradeSlot Slot;
    private Action<UpgradeSlot, GenericUpgrade> OnClick;

    public void Initialize(string upgradeName, UpgradeSlot slot, GenericUpgrade upgrade = null, Action<UpgradeSlot, GenericUpgrade> onClick = null)
    {
        UpgradeName = upgradeName;
        Upgrade = upgrade;
        OnClick = onClick;
        Slot = slot;
    }

    void Start()
    {
        this.gameObject.SetActive(false);

        if (IsSlotImage())
        {
            SetSlotImage();
        }
        else
        {
            LoadImage();
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
        Sprite sprite = (Sprite)Resources.Load("Sprites/SquadBuiler/UpgradeSlots/" + slotTypeName, typeof(Sprite));
        this.gameObject.transform.Find("UpgradeImage").GetComponent<Image>().sprite = sprite;

        this.gameObject.SetActive(true);
    }

    private void LoadImage()
    {
        Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, Upgrade.ImageUrl));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (www.error == null)
        {
            if (thisGameObject != null)
            {
                SetImageFromWeb(thisGameObject.transform.Find("UpgradeImage").gameObject, www);
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
        this.transform.Find("UpgradeInfo").GetComponent<Text>().text = UpgradeName;
        this.transform.Find("CostInfo").GetComponent<Text>().text = Upgrade.Cost.ToString();

        this.gameObject.SetActive(true);
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
}
