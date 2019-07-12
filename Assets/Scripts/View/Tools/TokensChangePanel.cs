using System;
using UnityEngine;
using UnityEngine.UI;
using Ship;
using Tokens;

public class TokensChangePanel : MonoBehaviour
{
    private GenericShip Host;
    private Image TokenImage;
    private Image MarkImage1;
    private Image MarkImage2;

    private float DelaySeconds = 2f;
    private float TimeStarted;
    private bool IsAssigned;

    private float movementProgress;

    private const float MOVE_SPEED = 30;
    private static Vector3 ABOVE_SHIP = new Vector3(0, 1, 0);
    private static Vector3 CENTER_PANEL = new Vector3(-31, 0, 0);

    public static void CreateTokensChangePanel(GenericShip host, GenericToken token, bool isAssigned)
    {
        GameObject prefab = (GameObject)Resources.Load("Prefabs/TokensChange", typeof(GameObject));
        GameObject MessagePanelsHolder = GameObject.Find("UI/TokensChangeHolder");
        GameObject Message = MonoBehaviour.Instantiate(prefab, MessagePanelsHolder.transform);
        Message.GetComponent<TokensChangePanel>().Initialize(host, token, isAssigned);
    }

    public void Initialize(GenericShip host, GenericToken token, bool isAssigned)
    {
        Host = host;
        TokenImage = transform.Find("TokenImage").GetComponent<Image>();
        IsAssigned = isAssigned;

        Sprite tokenSprite = Resources.Load<Sprite>("Sprites/Tokens/" + token.ImageName);
        TokenImage.sprite = tokenSprite;

        transform.position = Camera.main.WorldToScreenPoint(Host.GetCenter() + ABOVE_SHIP) + CENTER_PANEL;

        if (!isAssigned)
        {
            transform.Find("RemovedMark").gameObject.SetActive(true);
            MarkImage1 = transform.Find("RemovedMark/Line1").GetComponent<Image>();
            MarkImage2 = transform.Find("RemovedMark/Line2").GetComponent<Image>();
        }

        PlanSelfDestruction();
    }

    void Update()
    {
        movementProgress += Time.deltaTime * MOVE_SPEED;
        transform.position = Camera.main.WorldToScreenPoint(Host.GetCenter() + ABOVE_SHIP) + CENTER_PANEL + new Vector3(0, movementProgress, 0);

        if (IsAssigned)
        {
            TokenImage.color = new Color(1, 1, 1, (Time.time - TimeStarted) / DelaySeconds);
        }
        else
        {
            float alpha = (DelaySeconds - (Time.time - TimeStarted)) / DelaySeconds;
            TokenImage.color = new Color(1, 1, 1, alpha);
            MarkImage1.color = new Color(0, 0, 0, alpha);
            MarkImage2.color = new Color(0, 0, 0, alpha);
        }
    }

    private void PlanSelfDestruction()
    {
        TimeStarted = Time.time;
        MonoBehaviour.Destroy(this.gameObject, DelaySeconds);
    }

}
