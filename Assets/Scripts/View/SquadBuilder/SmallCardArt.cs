using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;
using Upgrade;
using Mods;

public class SmallCardArt : MonoBehaviour {

    private string ImageUrl;

    public void Initialize(string imageUrl)
    {
        ImageUrl = imageUrl;

        this.gameObject.SetActive(false);

        if (ImageUrl != null) LoadImage();
    }

    private void LoadImage()
    {
        Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, ImageUrl));
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

        if (newTexture.width > 250)
        {
            AdaptToPilotArt(targetObject, newTexture);
        }
        else
        {
            AdaptToUpgradeArt(targetObject, newTexture);
        }

        this.gameObject.SetActive(true);
    }

    private void AdaptToUpgradeArt(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, newTexture.height - 106, newTexture.width, 106), Vector2.zero);
        targetObject.GetComponent<RectTransform>().sizeDelta = new Vector2(188, 100);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;
    }

    private void AdaptToPilotArt(GameObject targetObject, Texture2D newTexture)
    {
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, newTexture.height - 124, newTexture.width, 124), Vector2.zero);
        targetObject.GetComponent<RectTransform>().sizeDelta = new Vector2(188, 78);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;
    }

    private void ShowTextVersionOfCard()
    {
        this.gameObject.SetActive(true);
    }
}
