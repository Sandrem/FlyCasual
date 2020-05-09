﻿using Movement;
using Editions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SubPhases;
using Ship;

public static class DirectionsMenu
{
    private static readonly float WarningPanelHeight = 55f;
    private static bool HasAnyAvailableManeuver = true;

    public static bool IsVisible
    {
        get { return DirectionsWindow != null && DirectionsWindow.activeSelf; }
    }

    public static Action<string> Callback;

    private static GameObject DirectionsWindow;

    public static void Show(Action<string> doWithSelectedManeuver, Action callback, Func<string, bool> filter = null, bool isRegularPlanning = false)
    {
        PrepareSubphase(doWithSelectedManeuver, callback);

        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/DirectionsWindow", typeof(GameObject));
        DirectionsWindow = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/DirectionsPanel").transform);

        if (Selection.ThisShip.Owner is Players.HumanPlayer)
        {
            GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
            CustomizeDirectionsMenu(filter);
            CustomizeForStressed();
            DirectionsWindow.transform.localPosition = FixMenuPosition(
                DirectionsWindow.transform.gameObject,
                Input.mousePosition
            );
        }
        else
        {
            DirectionsMenu.Hide();
        }

        Phases.CurrentSubPhase.IsReadyForCommands = true;

        if (isRegularPlanning) Selection.ThisShip.Owner.AskAssignManeuver();

        if (!HasAnyAvailableManeuver)
        {
            Messages.ShowError("No available maneuvers!");

            DirectionsMenu.Hide();
            DirectionsMenu.FinishManeuverSelections();
        }
    }

    public static void ShowForAll(Action<string> doWithSelectedManeuver, Action callback, Func<string, bool> filter = null)
    {
        /*PrepareSubphase(doWithSelectedManeuver, callback);

        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/DirectionsWindow", typeof(GameObject));
        DirectionsWindow = MonoBehaviour.Instantiate(prefab, GameObject.Find("UI/DirectionsPanel").transform);

        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        CustomizeDirectionsMenuAll(filter);
        DirectionsWindow.transform.localPosition = FixMenuPosition(
            DirectionsWindow.transform.gameObject,
            Input.mousePosition
        );*/
    }

    public static void FinishManeuverSelections()
    {
        Phases.FinishSubPhase(typeof(ManeuverSelectionSubphase));
    }

    private static void PrepareSubphase(Action<string> doWithSelectedManeuver, Action callback)
    {
        Triggers.RegisterTrigger(
            new Trigger()
            {
                Name = "Assign Maneuver",
                TriggerType = TriggerTypes.OnAbilityDirect,
                TriggerOwner = Phases.CurrentSubPhase.RequiredPlayer,
                EventHandler = delegate { StartAssignManeuverSubphase(doWithSelectedManeuver); }
            }
        );

        Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callback);
    }

    private static void StartAssignManeuverSubphase(Action<string> doWithSelectedManeuver)
    {
        ManeuverSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<ManeuverSelectionSubphase>(
            "Select a maneuver",
            Triggers.FinishTrigger
        );
        subphase.RequiredPlayer = Phases.CurrentSubPhase.RequiredPlayer;
        subphase.Start();

        DeleteOldDirectionsMenu();

        Callback = doWithSelectedManeuver;
    }

    private static void DeleteOldDirectionsMenu()
    {
        foreach (Transform transform in GameObject.Find("UI/DirectionsPanel").transform)
        {
            GameObject.Destroy(transform.gameObject);
        }
    }

    private static void CustomizeDirectionsMenu(Func<string, bool> filter = null)
    {
        List<char> linesExist = new List<char>();
        HasAnyAvailableManeuver = false;

        foreach (KeyValuePair<string, MovementComplexity> maneuverData in Selection.ThisShip.GetManeuvers())
        {
            string[] parameters = maneuverData.Key.Split('.');
            char maneuverSpeed = parameters[0].ToCharArray()[0];
            if (parameters[2] == "V")
            {
                switch (maneuverSpeed)
                {
                    case '1':
                        maneuverSpeed = '-';
                        break;
                    case '2':
                        maneuverSpeed = '=';
                        break;
                    default:
                        break;
                }
            }

            GameObject button = DirectionsWindow.transform.Find("Directions").Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
            if (maneuverData.Value != MovementComplexity.None)
            {
                if (filter == null || filter(maneuverData.Key))
                {
                    HasAnyAvailableManeuver = true;

                    if (!linesExist.Contains(maneuverSpeed)) linesExist.Add(maneuverSpeed);

                    SetManeuverColor(button, maneuverData);
                    button.SetActive(true);
                    button.GetComponent<Button>().onClick.AddListener(UI.AssignManeuverButtonPressed);

                    GameObject number = DirectionsWindow.transform.Find("Numbers").Find("Speed" + maneuverSpeed).Find("Number").gameObject;
                    number.SetActive(true);
                }
            }
        }

        HideExtraElements(linesExist);
    }

    private static void CustomizeDirectionsMenuAll(Func<string, bool> filter = null)
    {
        List<char> linesExist = new List<char>();

        foreach (string maneuverCode in GenericMovement.GetAllManeuvers())
        {
            string[] parameters = maneuverCode.Split('.');
            char maneuverSpeed = parameters[0].ToCharArray()[0];

            if (parameters[2] == "V")
            {
                switch (maneuverSpeed)
                {
                    case '1':
                        maneuverSpeed = '-';
                        break;
                    case '2':
                        maneuverSpeed = '=';
                        break;
                    default:
                        break;
                }
            }

            GameObject button = DirectionsWindow.transform.Find("Directions").Find("Speed" + maneuverSpeed).Find(maneuverCode).gameObject;

            if (filter == null || filter(maneuverCode))
            {
                if (!linesExist.Contains(maneuverSpeed)) linesExist.Add(maneuverSpeed);

                SetManeuverColor(button, new KeyValuePair<string, MovementComplexity>(maneuverCode, MovementComplexity.Normal));
                button.SetActive(true);
                button.GetComponent<Button>().onClick.AddListener(UI.AssignManeuverButtonPressed);

                GameObject number = DirectionsWindow.transform.Find("Numbers").Find("Speed" + maneuverSpeed).Find("Number").gameObject;
                number.SetActive(true);
            }

        }

        HideExtraElements(linesExist);
    }

    private static void HideExtraElements(List<char> linesExist)
    {
        float freeSpace = 40;

        // COLUMNS

        float totalWidth = 480f;

        List<string> columns = new List<string>() { ".L.E", ".L.R", ".F.R", ".R.R", ".R.E" };
        GameObject directionsPanel = DirectionsWindow.transform.Find("Directions").gameObject;
        int columnCounter = 0;

        int missingColumnsCounter = 0;

        foreach (var column in columns)
        {
            bool columnExists = false;
            for (int i = 1; i < 6; i++)
            {
                Transform directionIcon = directionsPanel.transform.Find("Speed" + i).Find(i + column);
                if (directionIcon != null && directionIcon.gameObject.activeSelf)
                {
                    columnExists = true;
                    //break;
                }
            }
            if (columnExists)
            {
                for (int i = 1; i < 6; i++)
                {
                    Transform directionIcon = directionsPanel.transform.Find("Speed" + i).Find(i + column);
                    if (directionIcon != null && directionIcon.gameObject.activeSelf)
                    {
                        directionIcon.localPosition = new Vector2(205 + columnCounter * 40, directionIcon.localPosition.y);
                    }
                }
                columnCounter++;
            }
            else
            {
                missingColumnsCounter++;
                totalWidth -= 40;
            }
        }

        // LINES

        float totalHeight = 320f;

        for (int i = -2; i < 6; i++)
        {
            char c = (i >= 0) ? i.ToString().ToCharArray()[0] : ((i == -1) ? '-' : '=');
            if (!linesExist.Contains(c))
            {
                totalHeight -= 40;

                GameObject numbersLinePanel = DirectionsWindow.transform.Find("Numbers").gameObject;
                numbersLinePanel.transform.Find("Speed" + c).gameObject.SetActive(false);

                GameObject directionsLinePanel = DirectionsWindow.transform.Find("Directions").gameObject;
                directionsLinePanel.transform.Find("Speed" + c).gameObject.SetActive(false);
            }
        }

        DirectionsWindow.GetComponent<RectTransform>().sizeDelta = new Vector3(DirectionsWindow.GetComponent<RectTransform>().sizeDelta.x, linesExist.Count * 40);
        DirectionsWindow.transform.Find("Numbers").GetComponent<RectTransform>().sizeDelta = new Vector3(DirectionsWindow.transform.Find("Numbers").GetComponent<RectTransform>().sizeDelta.x, linesExist.Count * 40 + 10);
        DirectionsWindow.transform.Find("Directions").GetComponent<RectTransform>().sizeDelta = new Vector3(DirectionsWindow.transform.Find("Directions").GetComponent<RectTransform>().sizeDelta.x - missingColumnsCounter * freeSpace, linesExist.Count * 40 + 10);

        DirectionsWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(DirectionsWindow.transform.Find("Directions").GetComponent<RectTransform>().sizeDelta.x + 70, DirectionsWindow.GetComponent<RectTransform>().sizeDelta.y);

        float offset = 5;
        foreach (Transform transform in DirectionsWindow.transform.Find("Numbers"))
        {
            if (transform.gameObject.activeSelf)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, offset);
                offset -= freeSpace;
            }
        }

        offset = 0;
        foreach (Transform transform in DirectionsWindow.transform.Find("Directions"))
        {
            if (transform.gameObject.activeSelf)
            {
                transform.localPosition = new Vector2(transform.localPosition.x, offset);
                offset -= freeSpace;
            }
        }

        DirectionsWindow.GetComponent<RectTransform>().sizeDelta = new Vector2(totalWidth + 10, totalHeight + 10);
    }

    private static void SetManeuverColor(GameObject button, KeyValuePair<string, MovementComplexity> maneuverData)
    {
        Color maneuverColor = Color.yellow;
        if (maneuverData.Value == MovementComplexity.Easy) maneuverColor = Edition.Current.MovementEasyColor;
        if (maneuverData.Value == MovementComplexity.Normal) maneuverColor = Color.white;
        if (maneuverData.Value == MovementComplexity.Complex)
        {
            maneuverColor = Color.red;
            if (Selection.ThisShip != null && Selection.ThisShip.IsStressed) button.transform.Find("RedBackground").gameObject.SetActive(true);
        }
        button.GetComponentInChildren<Text>().color = maneuverColor;
    }

    private static Vector3 FixMenuPosition(GameObject menuPanel, Vector3 position)
    {
        float globalUiScale = GameObject.Find("UI").transform.localScale.x;

        Vector3 screenPosition = new Vector3(position.x, position.y);
        Vector3 newPosition = new Vector3(position.x / globalUiScale, position.y / globalUiScale - Screen.height / globalUiScale);

        RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
        float windowHeight = menuRect.sizeDelta.y;
        float windowWidth = menuRect.sizeDelta.x;

        if (newPosition.x + windowWidth > Screen.width / globalUiScale)
        {
            newPosition = new Vector3(Screen.width / globalUiScale - windowWidth - 5, newPosition.y, 0);
        }
        if (-newPosition.y + windowHeight > Screen.height / globalUiScale)
        {
            newPosition = new Vector3(newPosition.x, (position.y + windowHeight) / globalUiScale - Screen.height / globalUiScale + 5, 0);
        }

        if (Selection.ThisShip != null
            && Selection.ThisShip.IsStressed
            && -newPosition.y < WarningPanelHeight * menuRect.localScale.y - 5
        )
        {
            newPosition = new Vector3(newPosition.x, - WarningPanelHeight - 5, 0);
        }
        return newPosition;
    }

    public static void Hide()
    {
        GameObject.Destroy(DirectionsWindow);
    }

    private static void CustomizeForStressed()
    {
        if (Selection.ThisShip.IsStressed)
        {
            GameObject warningGO = DirectionsWindow.transform.Find("Warning").gameObject;
            warningGO.SetActive(true);
        }
    }
}

namespace SubPhases
{
    public class ManeuverSelectionSubphase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.AssignManeuver }; } }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            CallBack();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }
    }
}