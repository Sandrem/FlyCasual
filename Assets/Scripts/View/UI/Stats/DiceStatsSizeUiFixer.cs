using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceStatsSizeUiFixer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float sizeX = Mathf.Abs(this.gameObject.GetComponent<RectTransform>().rect.x) * 2f / 3f;
        float sizeY = Mathf.Abs(this.gameObject.GetComponent<RectTransform>().rect.y) * 2;

        float offsetX = -Mathf.Abs(this.gameObject.GetComponent<RectTransform>().rect.x);
        float offsetY = Mathf.Abs(this.gameObject.GetComponent<RectTransform>().rect.y);

        foreach (Transform transform in this.gameObject.transform)
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, sizeY);
            transform.localPosition = new Vector2(offsetX, offsetY);
            offsetX += sizeX;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
