using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipPanelSquadBuilder : MonoBehaviour {

    public string ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Rebel%20Alliance/X-wing/wedge-antilles.png";
    public string ShipName = "X-Wing";

    // Use this for initialization
    void Start ()
    {
        LoadImage();
        SetName();
    }

    private void LoadImage()
    {
        if (ImageUrl != null) StartCoroutine(LoadTooltipImage(this.gameObject, ImageUrl));
    }

    private void SetName()
    {
        transform.Find("ShipName").GetComponent<Text>().text = ShipName;
    }

    private static IEnumerator LoadTooltipImage(GameObject thisGameObject, string url)
    {
        WWW www = new WWW(url);
        yield return www;

        if (www.texture != null)
        {
            SetImageFromWeb(thisGameObject.transform.Find("ShipImage").gameObject, www);
        }
    }

    private static void SetImageFromWeb(GameObject targetObject, WWW www)
    {
        Texture2D newTexture = new Texture2D(www.texture.height, www.texture.width);
        www.LoadImageIntoTexture(newTexture);
        Sprite newSprite = Sprite.Create(newTexture, new Rect(0, newTexture.height-124, newTexture.width, 124), Vector2.zero);
        targetObject.transform.GetComponent<Image>().sprite = newSprite;
    }
}
