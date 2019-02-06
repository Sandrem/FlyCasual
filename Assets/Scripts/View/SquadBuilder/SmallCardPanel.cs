using Editions;
using SquadBuilderNS;
using SubPhases;
using System;
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
    private const string TEXTURENAME = "CARDPANEL_";
    private string textureCacheKey;

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
            textureCacheKey = TEXTURENAME + CardTooltip;
            LoadTooltipImage(this.gameObject, CardTooltip);
        }
        else
        {
            ShowTextVersionOfCard();
        }

        SetOnClickHandler();
    }

    private void LoadTooltipImage(GameObject thisGameObject, string url)
    {
        if (!SquadBuilder.TextureCache.ContainsKey(textureCacheKey))
        {
            Global.Instance.StartCoroutine(ImageManager.GetTexture((texture) =>
            {
                if (thisGameObject != null && texture != null)
                {
                    SetObjectSprite(thisGameObject.transform.Find("CardImage").gameObject, texture, false);
                }
                else
                {
                    ShowTextVersionOfCard();
                }
            }, url));
        }
        else
        {
            SetObjectSprite(thisGameObject.transform.Find("CardImage").gameObject, SquadBuilder.TextureCache[textureCacheKey], true);
        }
    }

    private void SetObjectSprite(GameObject targetObject, Texture2D newTexture, bool textureIsScaled)
    {
        if (!textureIsScaled)
        {
            switch (DecisionViewType)
            {
                case DecisionViewTypes.ImagesUpgrade:
                    TextureScale.Bilinear(
                        newTexture,
                        (int)Edition.Current.UpgradeCardSize.x,
                        (int)Edition.Current.UpgradeCardSize.y
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
        }
        if (!SquadBuilder.TextureCache.ContainsKey(textureCacheKey)) SquadBuilder.TextureCache.Add(textureCacheKey, newTexture);
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
