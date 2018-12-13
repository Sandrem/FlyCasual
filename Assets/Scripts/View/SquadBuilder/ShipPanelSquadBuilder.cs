using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using SquadBuilderNS;

public class ShipPanelSquadBuilder : MonoBehaviour {

    public string ImageUrl;
    public string ShipName;
    public string FullType;

    // Use this for initialization
    void Start ()
    {
        this.gameObject.SetActive(false);

        LoadImage();
        SetName();
        SetOnClickHandler();
    }

    private void LoadImage()
    {
        if (ImageUrl != null) Global.Instance.StartCoroutine(LoadTooltipImage(this.gameObject, ImageUrl));
    }

    private void SetName()
    {
        transform.Find("ShipName").GetComponent<Text>().text = FullType;
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = ImageManager.GetImage(url);
        yield return www;

        if (www.error == null)
        {
            if (thisGameObject != null) SetImageFromWeb(thisGameObject.transform.Find("ShipImage").gameObject, www);
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

        TextureScale.Bilinear(newTexture, 600, 836);

        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, newTexture.height-248, newTexture.width, 248), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;

        FinallyShow();
    }

    private void ShowTextVersionOfCard()
    {
        FinallyShow();
    }

    private void SetOnClickHandler()
    {
        EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(delegate { SelectShip(ShipName); });
        trigger.triggers.Add(entry);
    }

    private void SelectShip(string shipName)
    {
        SquadBuilder.CurrentShip = shipName;
        MainMenu.CurrentMainMenu.ChangePanel("SelectPilotPanel");
    }
    
    private void FinallyShow()
    {
        GameObject loadingText = GameObject.Find("UI/Panels/SelectShipPanel/LoadingText");
        if (loadingText != null) loadingText.SetActive(false);

        this.gameObject.SetActive(true);
    }
}
