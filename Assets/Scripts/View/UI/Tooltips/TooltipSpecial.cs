using SquadBuilderNS;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class TooltipSpecial
{

    private static bool TooltipIsCalled;
    private static bool TooltipIsVisible;
    private static readonly float ACTIVATION_DELAY = 1f;

    private static MonoBehaviour Behavior;
    private static Transform TooltipsPanel;
    private static Transform SpecialTooltipPanel;

    static TooltipSpecial()
    {
        //TEMPORARY
        if (GameObject.Find("Global") == null)
        {
            Behavior = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        }
        else
        {
            Behavior = GameObject.Find("Global").GetComponent<Global>();
        }
    }

    public static void StartTooltip(GameObject sender, Action<Transform> createElementDelegate)
    {
        if (!TooltipIsVisible)
        {
            TooltipsPanel = GameObject.Find("UI").transform.Find("TooltipPanel").transform;

            if (createElementDelegate != null)
            {
                TooltipIsCalled = true;

                LoadTooltip(createElementDelegate);
            }
        }
    }

    private static void LoadTooltip(Action<Transform> createElementDelegate)
    {
        Behavior.StartCoroutine(SetTooltip(createElementDelegate));
    }

    private static IEnumerator SetTooltip(Action<Transform> createElementDelegate)
    {
        SpecialTooltipPanel = TooltipsPanel.Find("SpecialTooltipPanel");

        foreach (Transform transform in SpecialTooltipPanel.transform)
        {
            GameObject.Destroy(transform.gameObject);
        }

        createElementDelegate(SpecialTooltipPanel);

        GameObject createdPanel = SpecialTooltipPanel.transform.Find("SpecialTooltip").gameObject;
        RectTransform createdRect = createdPanel.GetComponent<RectTransform>();
        TooltipsPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(createdRect.sizeDelta.x / 2f + 10f, createdRect.sizeDelta.y / 2f + 10f);
        SpecialTooltipPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(createdRect.sizeDelta.x, createdRect.sizeDelta.y);

        PrepareTooltipPanel();
        yield return new WaitForSeconds(ACTIVATION_DELAY);

        if (TooltipIsCalled) ShowTooltip();
    }

    private static void PrepareTooltipPanel()
    {
        foreach (Transform transform in TooltipsPanel)
        {
            transform.gameObject.SetActive(false);
        }
        SpecialTooltipPanel.gameObject.SetActive(true);
    }

    private static void SetTooltipPosition()
    {
        float uiScale = GameObject.Find("UI").transform.localScale.x;

        RectTransform imageRect = SpecialTooltipPanel.GetComponent<RectTransform>();
        float imageWidth = imageRect.sizeDelta.x * uiScale;
        float imageHeight = imageRect.sizeDelta.y * uiScale;

        float mousePositionX = Input.mousePosition.x;
        float mousePositionY = Input.mousePosition.y;

        float windowPositionX = 0;
        float windowPositionY = 0;

        if (mousePositionX + imageWidth / 2f + 25f > Screen.width)
        {
            windowPositionX = mousePositionX - imageWidth / 2f - 25f;
        }
        else
        {
            windowPositionX = mousePositionX + 25f;
        }

        if ((Screen.height - mousePositionY) + imageHeight / 2f > Screen.height)
        {
            windowPositionY = imageHeight / 2f;
        }
        else
        {
            windowPositionY = mousePositionY;
        }

        TooltipsPanel.transform.position = new Vector3(windowPositionX, windowPositionY);
    }

    private static void ShowTooltip()
    {
        SetTooltipPosition();
        TooltipsPanel.gameObject.SetActive(true);
        TooltipIsVisible = true;
    }

    public static void EndTooltip()
    {
        TooltipIsCalled = false;
        TooltipIsVisible = false;
        if (TooltipsPanel != null) TooltipsPanel.gameObject.SetActive(false);
    }

    public static void AddTooltip(GameObject sender, Action<Transform> createElementDelegate)
    {
        EventTrigger trigger = sender.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            sender.AddComponent<EventTrigger>();
            trigger = sender.GetComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { StartTooltip(sender, createElementDelegate); });
        trigger.triggers.Add(entry);

        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback.AddListener((data) => { EndTooltip(); });
        trigger.triggers.Add(entry);
    }

}
