using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Upgrade;
using Mods;
using UnityEngine.Networking;

public class AvatarFromUpgrade : MonoBehaviour {

    private string UpgradeType;
    private GenericUpgrade Upgrade;
    private Vector2 Offset;
    private Action<string> OnClick;

    public void Initialize(string upgradeType, Action<string> onClick = null)
    {
        UpgradeType = upgradeType;

        if (Type.GetType(upgradeType) == null)
        {
            Options.Avatar = Options.DefaultAvatar;
            Options.ChangeParameterValue("Avatar", Options.DefaultAvatar);
            upgradeType = Options.DefaultAvatar;
        }

        Upgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));
        Offset = Upgrade.Avatar.AvatarOffset;
        OnClick = onClick;

        this.gameObject.SetActive(false);
        LoadTooltipImage(this.gameObject, Upgrade.ImageUrlFE);

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
                    if (!SquadBuilder.TextureCache.ContainsKey(url)) SquadBuilder.TextureCache.Add(url, texture); //Since we did not scale/modify this texture, just cache using the url for future use
                    SetObjectSprite(thisGameObject, texture);
                }
                else
                {
                    ShowTextVersionOfCard();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(thisGameObject, SquadBuilder.TextureCache[url]);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(Offset.x, newTexture.height-100-Offset.y, 100, 100), Vector2.zero);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        this.gameObject.SetActive(true);
    }

    private void ShowTextVersionOfCard()
    {
        try
        {
            this.gameObject.SetActive(true);
        }
        catch {}
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
