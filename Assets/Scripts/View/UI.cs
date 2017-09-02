using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public class UI : MonoBehaviour {

    private static float lastLogTextPosition = -5;
    private static float lastLogTextStep = -20;

    private int minimapSize = 256;

    public static bool ShowShipIds;

    public void Update()
    {
        UpdateShipIds();
    }

    private void UpdateShipIds()
    {
        ShowShipIds = Input.GetKey(KeyCode.LeftAlt);
    }

    //Move to context menu
    public static void CallContextMenu(Ship.GenericShip ship)
    {
        ShowContextMenu(ship, Input.mousePosition + new Vector3(0f, 0f, 0f));
    }

    private static void ShowContextMenu(Ship.GenericShip ship, Vector3 position)
    {
        HideDirectionMenu();
        HideContextMenuButtons();
        if (Phases.CurrentSubPhase.CountActiveButtons(ship) > 0)
        {
            GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(true);
            position = FixMenuPosition(GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject, position);
            GameObject.Find("UI").transform.Find("ContextMenuPanel").position = position;
        }
        else
        {
            GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        }

    }

    private static void HideContextMenuButtons()
    {
        foreach (Transform button in GameObject.Find("UI").transform.Find("ContextMenuPanel"))
        {
            button.gameObject.SetActive(false);
        }
    }

    public static void HideContextMenu()
    {
        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
    }

    public void ShowDirectionMenu()
    {
        GameObject.Find("UI").transform.Find("ContextMenuPanel").gameObject.SetActive(false);
        SetAvailableManeurs();
        GameObject.Find("UI").transform.Find("DirectionsPanel").position = FixMenuPosition(GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject, GameObject.Find("UI").transform.Find("ContextMenuPanel").position);
        GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject.SetActive(true);
    }

    //Add icons
    private void SetAvailableManeurs()
    {
        foreach (KeyValuePair<string, Movement.ManeuverColor> maneuverData in Selection.ThisShip.GetManeuvers())
        {
            string[] parameters = maneuverData.Key.Split('.');
            string maneuverSpeed = parameters[0];

            if (maneuverData.Value == Movement.ManeuverColor.None)
            {
                GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Speed" + maneuverSpeed + "/" + maneuverData.Key).gameObject.SetActive(false);
            }
            else
            {
                GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Speed" + maneuverSpeed + "/" + maneuverData.Key).gameObject.SetActive(true);

                GameObject button = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
                SetManeuverIcon(button, maneuverData);
            }

        }
    }

    private void SetManeuverIcon(GameObject button, KeyValuePair<string, Movement.ManeuverColor> maneuverData)
    {
        Movement.MovementStruct movement = new Movement.MovementStruct(maneuverData.Key);

        string imageName = "";

        if ((movement.Direction == Movement.ManeuverDirection.Forward) && (movement.Bearing == Movement.ManeuverBearing.Straight)) imageName += "Straight";
        if ((movement.Direction == Movement.ManeuverDirection.Forward) && (movement.Bearing == Movement.ManeuverBearing.KoiogranTurn)) imageName += "Koiogran";
        if (movement.Bearing == Movement.ManeuverBearing.Bank) imageName += "Bank";
        if (movement.Bearing == Movement.ManeuverBearing.Turn) imageName += "Turn";

        if (movement.Direction == Movement.ManeuverDirection.Left) imageName += "Left";
        if (movement.Direction == Movement.ManeuverDirection.Right) imageName += "Right";

        if (maneuverData.Value == Movement.ManeuverColor.Green) imageName += "Green";
        if (maneuverData.Value == Movement.ManeuverColor.White) imageName += "White";
        if (maneuverData.Value == Movement.ManeuverColor.Red) imageName += "Red";

        Sprite image = GameObject.Find("UI").transform.Find("DirectionsPanel").Find("ImageStorageDirections").Find(imageName).GetComponent<Image>().sprite;
        button.GetComponent<Image>().sprite = image;
    }

    public static void HideDirectionMenu()
    {
        GameObject.Find("UI").transform.Find("DirectionsPanel").gameObject.SetActive(false);
    }

    public static void HideTemporaryMenus()
    {
        HideContextMenu();
        HideDirectionMenu();
    }

    private static Vector3 FixMenuPosition(GameObject menuPanel, Vector3 position) {
        if (position.x + menuPanel.GetComponent<RectTransform>().rect.width > Screen.width) {
            position = new Vector3(Screen.width - menuPanel.GetComponent<RectTransform>().rect.width - 5, position.y, 0);
        }
        if (position.y - menuPanel.GetComponent<RectTransform>().rect.height < 0)
        {
            position = new Vector3(position.x, menuPanel.GetComponent<RectTransform>().rect.height + 5, 0);
        }
        return position;
    }

    public static void ShowGameResults(string results)
    {
        GameObject gameResultsPanel = GameObject.Find("UI/GameResultsPanel").gameObject;
        gameResultsPanel.transform.Find("Panel").transform.Find("Congratulations").GetComponent<Text>().text = results;
        gameResultsPanel.SetActive(true);
    }

    public void CallChangeMiniMapSize()
    {
        switch (minimapSize)
        {
            case 128: minimapSize = 256;
                break;
            case 256: minimapSize = 512;
                break;
            case 512: minimapSize = 128;
                break;
            default:
                break;
        }
        GameObject.Find("UI").transform.Find("MiniMapHolder").GetComponent<RectTransform>().sizeDelta = new Vector2(minimapSize, minimapSize);
    }

    public void ToggleMinimap()
    {
        GameObject.Find("UI").transform.Find("GameLogHolder").gameObject.SetActive(false);
        GameObject.Find("UI").transform.Find("MiniMapHolder").gameObject.SetActive(!GameObject.Find("UI").transform.Find("MiniMapHolder").gameObject.activeSelf);
    }

    public void ToggleGameLog()
    {
        GameObject.Find("UI").transform.Find("MiniMapHolder").gameObject.SetActive(false);
        GameObject.Find("UI").transform.Find("GameLogHolder").gameObject.SetActive(!GameObject.Find("UI").transform.Find("GameLogHolder").gameObject.activeSelf);
    }

    public static void AddTestLogEntry(string text)
    {
        GameObject area = GameObject.Find("UI").transform.Find("GameLogHolder").Find("Scroll").Find("Viewport").Find("Content").gameObject;
        GameObject logText = (GameObject)Resources.Load("Prefabs/LogText", typeof(GameObject));
        GameObject newLogEntry = Instantiate(logText, area.transform);
        newLogEntry.transform.localPosition = new Vector3(5, lastLogTextPosition, 0);
        lastLogTextPosition += lastLogTextStep;
        if (area.GetComponent<RectTransform>().sizeDelta.y < Mathf.Abs(lastLogTextPosition)) area.GetComponent<RectTransform>().sizeDelta = new Vector2(area.GetComponent<RectTransform>().sizeDelta.x, Mathf.Abs(lastLogTextPosition));
        GameObject.Find("UI").transform.Find("GameLogHolder").Find ("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        newLogEntry.GetComponent<Text>().text = text;
    }

    public void ShowDecisionsPanel()
    {
        //start subphase

        GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(true);
    }

    public void HideDecisionsPanel()
    {
        GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(false);
        //end subphase
    }

    public void ClickNextPhase()
    {
        HideNextButton();
        Roster.AllShipsHighlightOff();

        Phases.CurrentSubPhase.NextButton();
    }

    public void ClickSkipPhase()
    {
        HideNextButton();
        Roster.AllShipsHighlightOff();

        Phases.CurrentSubPhase.SkipButton();
    }

    public void ClickDeclareTarget()
    {
        Combat.DeclareTarget();
    }

    public static void ShowNextButton()
    {
        GameObject.Find("UI").transform.Find("NextPanel").gameObject.SetActive(true);
        GameObject.Find("UI/NextPanel").transform.Find("NextButton").GetComponent<Animator>().enabled = false;
    }

    public static void HideNextButton()
    {
        GameObject.Find("UI/NextPanel").gameObject.SetActive(false);
        GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Animator>().enabled = false;

        ColorBlock colors = GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Button>().colors;
        colors.normalColor = new Color32(0, 0, 0, 200);
        GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Button>().colors = colors;
    }

    public static void ShowSkipButton()
    {
        GameObject.Find("UI").transform.Find("SkipPanel").gameObject.SetActive(true);
    }

    public static void HideSkipButton()
    {
        GameObject.Find("UI").transform.Find("SkipPanel").gameObject.SetActive(false);
    }

    public static void HighlightNextButton()
    {
        GameObject.Find("UI").transform.Find("NextPanel").Find("NextButton").GetComponent<Animator>().enabled = true;
    }

    public static void CallHideTooltip()
    {
        Tooltips.EndTooltip();
    }

    public static void HideInformCritPanel()
    {
        InformCrit.HidePanel();
    }

    public static void QuitGame()
    {
        Application.Quit();
    }

}
