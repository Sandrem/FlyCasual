using RuleSets;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Upgrade;

public class SmallCardArt : MonoBehaviour {

    private string ImageUrl;
    private IImageHolder ImageSource;

    public void Initialize(IImageHolder imageSource)
    {
        ImageSource = imageSource;
        ImageUrl = ImageSource.ImageUrl;

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

        SetAdaptedArt(newTexture, ImageSource, targetObject);

        this.gameObject.SetActive(true);
    }

    private void SetAdaptedArt(Texture2D newTexture, object imageSource, GameObject targetObject)
    {
        Rect imageRect = new Rect();
        if (imageSource is GenericShip)
        {
            imageRect = (Edition.Current is SecondEdition) ? new Rect(0, 0, 503, 205) : new Rect(0, 0, 298, 124);
        }
        else if (imageSource is GenericUpgrade)
        {
            imageRect = (Edition.Current is SecondEdition) ? new Rect(281, 0, 394, 202) : new Rect(0, 0, 194, 106);
        }

        Sprite newSprite = Sprite.Create(
            newTexture,
            new Rect(
                imageRect.x,
                newTexture.height - imageRect.height - imageRect.y,
                imageRect.width,
                imageRect.height),
            Vector2.zero
        );

        targetObject.transform.GetComponent<Image>().sprite = newSprite;
        targetObject.GetComponent<RectTransform>().sizeDelta = new Vector2(188, 188 / imageRect.width * imageRect.height);
    }

    private void ShowTextVersionOfCard()
    {
        this.gameObject.SetActive(true);
    }
}
