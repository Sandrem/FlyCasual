using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Upgrade;

public class UpgradePanelSquadBuilder : MonoBehaviour {

    private string UpgradeName;
    private string ImageUrl;
    private UpgradeSlot Slot;
    private Action<UpgradeSlot, string> OnClick;

    public void Initialize(string upgradeName, UpgradeSlot slot, string imageUrl = null, Action<UpgradeSlot, string> onClick = null)
    {
        UpgradeName = upgradeName;
        ImageUrl = imageUrl;
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
        Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, ImageUrl));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (url == null)
        {
            url = SquadBuilder.AllUpgrades.Find(n => n.UpgradeName == UpgradeName).Instance.ImageUrl;
        }

        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (www.texture != null)
        {
            SetImageFromWeb(thisGameObject.transform.Find("UpgradeImage").gameObject, www);
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

    private void SetOnClickHandler()
    {
        if (OnClick != null)
        {
            EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { OnClick(Slot, UpgradeName); });
            trigger.triggers.Add(entry);
        }
    }
}
