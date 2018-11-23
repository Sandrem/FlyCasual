using RuleSets;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmallCardPanel : MonoBehaviour {

    private string CardName;
    private string CardTooltip;
    private int Count;
    private Action<string> OnClick;
    private DecisionViewTypes DecisionViewType;
    private static readonly Vector2 ImagesDamageCardSize = new Vector2(194, 300);

    public void Initialize(string damageCardName, string damageCardTooltip, Action<string> onClick = null, DecisionViewTypes decisionViewType = DecisionViewTypes.ImagesUpgrade, int count = -1)
    {
        CardName = damageCardName;
        CardTooltip = damageCardTooltip;
        OnClick = onClick;
        Count = count;
        DecisionViewType = decisionViewType;
    }

    void Start()
    {
        this.gameObject.SetActive(false);

        if (!String.IsNullOrEmpty(CardTooltip))
        {
            LoadImage();
        }
        else
        {
            ShowTextVersionOfCard();
        }

        SetOnClickHandler();
    }

    private void LoadImage()
    {
        Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, CardTooltip));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (www.error == null)
        {
            if (thisGameObject != null)
            {
                SetImageFromWeb(thisGameObject.transform.Find("CardImage").gameObject, www);
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

        switch (DecisionViewType)
        {
            case DecisionViewTypes.ImagesUpgrade:
                TextureScale.Bilinear(
                    newTexture,
                    (int)Edition.Instance.UpgradeCardSize.x,
                    (int)Edition.Instance.UpgradeCardSize.y
                );
                break;
            case DecisionViewTypes.ImagesDamageCard:
                TextureScale.Bilinear(
                    newTexture,
                    (int)ImagesDamageCardSize.x,
                    (int)ImagesDamageCardSize.y
                );
                break;
            default:
                break;
        }

        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        Image image = targetObject.transform.GetComponent<Image>();
        image.sprite = newSprite;

        ShowCounter();

        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(newTexture.width, newTexture.height);
        this.gameObject.SetActive(true);
    }

    private void ShowTextVersionOfCard()
    {
        this.transform.Find("CardInfo").GetComponent<Text>().text = CardName;

        ShowCounter();

        this.gameObject.SetActive(true);
    }

    private void ShowCounter()
    {
        this.transform.Find("CountPanel").gameObject.SetActive(Count != -1);
        this.transform.Find("CountPanel").Find("CountText").GetComponent<Text>().text = String.Format("x{0}", Count.ToString());
    }

    private void SetOnClickHandler()
    {
        if (OnClick != null)
        {
            EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(delegate { OnClick(CardName); });
            trigger.triggers.Add(entry);
        }
    }
}
