﻿using System;
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
    private AvatarInfo Avatar;
    private Action<string> OnClick;
    public Action OnDownloaded;

    public void Initialize(string upgradeType, Action<string> onClick = null, Action onDownloaded = null)
    {
        UpgradeType = upgradeType;

        if (Type.GetType(upgradeType) == null)
        {
            Options.Avatar = Options.DefaultAvatar;
            Options.ChangeParameterValue("Avatar", Options.DefaultAvatar);
            upgradeType = Options.DefaultAvatar;
        }

        Upgrade = (GenericUpgrade)System.Activator.CreateInstance(Type.GetType(upgradeType));
        Avatar = Upgrade.Avatar;
        OnClick = onClick;
        OnDownloaded = onDownloaded;

        this.gameObject.SetActive(false);
        LoadTooltipImage(this.gameObject, (Upgrade.GetType().ToString().Contains("UpgradesList.FirstEdition")) ? Upgrade.ImageUrlFE : Upgrade.ImageUrl);

        SetOnClickHandler();
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!TextureCache.Cache.ContainsKey(url))
        {
            Global.Instance.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (texture != null && thisGameObject != null)
                {
                    if (!TextureCache.Cache.ContainsKey(url)) TextureCache.Cache.Add(url, texture); //Since we did not scale/modify this texture, just cache using the url for future use
                    SetObjectSprite(thisGameObject, texture);
                    OnDownloaded?.Invoke();
                }
                else
                {
                    ShowTextVersionOfCard();
                    OnDownloaded?.Invoke();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(thisGameObject, TextureCache.Cache[url]);
            OnDownloaded?.Invoke();
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(Avatar.AvatarOffset.x, newTexture.height-Avatar.AvatarSize.x-Avatar.AvatarOffset.y, Avatar.AvatarSize.x, Avatar.AvatarSize.y), Vector2.zero);
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
            entry.callback.AddListener(delegate {
                Debug.Log($"Avatar is now {UpgradeType.ToString()}");
                OnClick(UpgradeType); 
            });
            trigger.triggers.Add(entry);
        }
    }
}
