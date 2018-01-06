using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;

public class UpgradePanelSquadBuilder : MonoBehaviour {

    private string ImageUrl;
    private string UpgradeName;

    public void Initialize(string upgradeName, string imageUrl = null)
    {
        UpgradeName = upgradeName;
        ImageUrl = imageUrl;
    }

    void Start()
    {
        if (IsSlotImage())
        {
            SetSlotImage();
        }
        else
        {
            LoadImage();
        }
        //SetOnClickHandler();
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
    }

    private void LoadImage()
    {
        StartCoroutine(LoadTooltipImage(this.gameObject, ImageUrl));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (url == null)
        {
            Debug.Log(UpgradeName);
            url = SquadBuilder.AllUpgrades.Find(n => n.UpgradeName == UpgradeName).Instance.ImageUrl;
        }

        WWW www = new WWW(url);
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
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
    }

    /*private void SetOnClickHandler()
    {
        if (OnClick != null)
        {
            EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { OnClick(Ship, PilotName, ShipName); });
            trigger.triggers.Add(entry);
        }
    }*/
}
