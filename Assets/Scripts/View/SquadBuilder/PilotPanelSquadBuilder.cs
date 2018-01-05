using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PilotPanelSquadBuilder : MonoBehaviour {

    public string ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/X-wing/wedge-antilles.png";
    public string ShipName = "X-Wing";
    public string PilotName = "Wedge Antilles";
    public Action<string, string> OnClick;

    // Use this for initialization
    void Start ()
    {
        LoadImage();
        SetOnClickHandler();
    }

    private void LoadImage()
    {
        if (ImageUrl != null) StartCoroutine(LoadTooltipImage(this.gameObject, ImageUrl));
    }

    private IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.texture != null)
        {
            SetImageFromWeb(thisGameObject.transform.Find("PilotImage").gameObject, www);
        }
    }

    private void SetImageFromWeb(GameObject targetObject, WWW www)
    {
        Texture2D newTexture = new Texture2D(www.texture.height, www.texture.width);
        www.LoadImageIntoTexture(newTexture);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
    }

    private void SetOnClickHandler()
    {
        EventTrigger trigger = this.gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener(delegate { OnClick(PilotName, ShipName); });
        trigger.triggers.Add(entry);
    }

    private void SelectShip(string shipName)
    {
        Messages.ShowInfo(shipName);
    }
}
