using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DamageDeckCard;

public class DamageCardPanel : MonoBehaviour {

    private string DamageCardName;
    private string DamageCardTooltip;
    private int Count;
    private Action<string> OnClick;

    public void Initialize(string damageCardName, string damageCardTooltip, Action<string> onClick = null, int count = -1)
    {
        DamageCardName = damageCardName;
        DamageCardTooltip = damageCardTooltip;
        OnClick = onClick;
        Count = count;
    }

    void Start()
    {
        this.gameObject.SetActive(false);

        if (!String.IsNullOrEmpty(DamageCardTooltip))
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
        Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, DamageCardTooltip));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (www.error == null)
        {
            if (thisGameObject != null)
            {
                SetImageFromWeb(thisGameObject.transform.Find("DamageCardImage").gameObject, www);
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

        ShowCounter();

        this.gameObject.SetActive(true);
    }

    private void ShowTextVersionOfCard()
    {
        this.transform.Find("DamageCardInfo").GetComponent<Text>().text = DamageCardName;

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
            entry.callback.AddListener(delegate { OnClick(DamageCardName); });
            trigger.triggers.Add(entry);
        }
    }
}
