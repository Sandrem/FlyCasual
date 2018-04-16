using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Upgrade;
using Mods;

public class AvatarFromUpgrade : MonoBehaviour {

    private string UpgradeType;
    private GenericUpgrade Upgrade;
    private Vector2 Offset;
    private Action<string> OnClick;

    public void Initialize(string upgradeType, Action<string> onClick = null)
    {
        UpgradeType = upgradeType;
        Upgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));
        Offset = Upgrade.AvatarOffset;
        OnClick = onClick;

        this.gameObject.SetActive(false);
        LoadImage();
        SetOnClickHandler();
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
                SetImageFromWeb(thisGameObject, www);
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
        Sprite newSprite = Sprite.Create(newTexture, new Rect(Offset.x, newTexture.height-100-Offset.y, 100, 100), Vector2.zero);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        this.gameObject.SetActive(true);
    }

    private void ShowTextVersionOfCard()
    {
        this.gameObject.SetActive(true);
    }

    private void SetOnClickHandler()
    {
        if (OnClick != null)
        {
            EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { OnClick(UpgradeType); });
            trigger.triggers.Add(entry);
        }
    }
}
