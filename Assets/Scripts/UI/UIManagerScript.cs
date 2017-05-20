using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public class UIManagerScript: MonoBehaviour {

    private GameManagerScript Game;

    public MessageManagerScript ErrorManager;
    public ActionsPanelScript ActionsPanel;

    public GameObject panelDirectionMenu;
    public GameObject panelContextMenu;
    public GameObject panelGameResultsMessage;
    public GameObject panelMinimap;
    public GameObject panelGameLog;
    public GameObject ImageStorageDirections;

    public GameObject prefabLogText;
    private float lastLogTextPosition = -5;
    private float lastLogTextStep = -20;

    private int minimapSize = 256;

    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    //Move to context menu
    public void CallContextMenu(Ship.GenericShip ship)
    {
        ShowContextMenu(ship, Input.mousePosition + new Vector3(0f, 0f, 0f));
    }

    private void ShowContextMenu(Ship.GenericShip ship, Vector3 position)
    {
        HideDirectionMenu();
        HideContextMenuButtons();
        if (Phases.CurrentSubPhase.CountActiveButtons(ship) > 0)
        {
            panelContextMenu.SetActive(true);
            position = FixMenuPosition(panelContextMenu, position);
            panelContextMenu.transform.position = position;
        }
        else
        {
            panelContextMenu.SetActive(false);
        }

    }

    private void HideContextMenuButtons()
    {
        foreach (Transform button in panelContextMenu.transform)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void ShowError(string text)
    {
        ErrorManager.ShowError(text);
    }

    public void ShowInfo(string text)
    {
        ErrorManager.ShowInfo(text);
    }

    public void HideContextMenu()
    {
        panelContextMenu.SetActive(false);
    }

    public void ShowDirectionMenu()
    {
        panelContextMenu.SetActive(false);
        SetAvailableManeurs();
        panelDirectionMenu.transform.position = FixMenuPosition(panelDirectionMenu, panelContextMenu.transform.position);
        panelDirectionMenu.SetActive(true);
    }

    //Add icons
    private void SetAvailableManeurs()
    {
        foreach (KeyValuePair<string, Ship.ManeuverColor> maneuverData in Selection.ThisShip.GetManeuvers())
        {
            string[] parameters = maneuverData.Key.Split('.');
            string maneuverSpeed = parameters[0];

            if (maneuverData.Value == Ship.ManeuverColor.None)
            {
                panelDirectionMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject.SetActive(false);
            }
            else
            {
                panelDirectionMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject.SetActive(true);

                GameObject button = panelDirectionMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
                SetManeuverIcon(button, maneuverData);
            }

        }
    }

    private void SetManeuverIcon(GameObject button, KeyValuePair<string, Ship.ManeuverColor> maneuverData)
    {
        Movement movement = Game.Movement.ManeuverFromString(maneuverData.Key);

        string imageName = "";

        if ((movement.Direction == ManeuverDirection.Forward) && (movement.Bearing == ManeuverBearing.Straight)) imageName += "Straight";
        if ((movement.Direction == ManeuverDirection.Forward) && (movement.Bearing == ManeuverBearing.KoiogranTurn)) imageName += "Koiogran";
        if (movement.Bearing == ManeuverBearing.Bank) imageName += "Bank";
        if (movement.Bearing == ManeuverBearing.Turn) imageName += "Turn";

        if (movement.Direction == ManeuverDirection.Left) imageName += "Left";
        if (movement.Direction == ManeuverDirection.Right) imageName += "Right";

        if (maneuverData.Value == Ship.ManeuverColor.Green) imageName += "Green";
        if (maneuverData.Value == Ship.ManeuverColor.White) imageName += "White";
        if (maneuverData.Value == Ship.ManeuverColor.Red) imageName += "Red";

        Sprite image = ImageStorageDirections.transform.Find(imageName).GetComponent<Image>().sprite;
        button.GetComponent<Image>().sprite = image;
    }

     public void HideDirectionMenu()
    {
        panelDirectionMenu.SetActive(false);
    }

    public void HideTemporaryMenus()
    {
        HideContextMenu();
        HideDirectionMenu();
    }

    private Vector3 FixMenuPosition(GameObject menuPanel, Vector3 position) {
        if (position.x + menuPanel.GetComponent<RectTransform>().rect.width > Screen.width) {
            position = new Vector3(Screen.width - menuPanel.GetComponent<RectTransform>().rect.width - 5, position.y, 0);
        }
        if (position.y - menuPanel.GetComponent<RectTransform>().rect.height < 0)
        {
            position = new Vector3(position.x, menuPanel.GetComponent<RectTransform>().rect.height + 5, 0);
        }
        return position;
    }

    public void ShowGameResults(string results)
    {
        panelGameResultsMessage.transform.GetComponentInChildren<Text>().text = results;
        panelGameResultsMessage.SetActive(true);
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
        panelMinimap.GetComponent<RectTransform>().sizeDelta = new Vector2(minimapSize, minimapSize);
    }

    public void ToggleMinimap()
    {
        panelGameLog.SetActive(false);
        panelMinimap.SetActive(!panelMinimap.activeSelf);
    }

    public void ToggleGameLog()
    {
        panelMinimap.SetActive(false);
        panelGameLog.SetActive(!panelGameLog.activeSelf);
    }

    public void AddTestLogEntry(string text)
    {
        Transform area = panelGameLog.transform.Find("Scroll").Find("Viewport").Find("Content");
        GameObject newLogEntry = Instantiate(prefabLogText, area);
        newLogEntry.transform.localPosition = new Vector3(5, lastLogTextPosition, 0);
        lastLogTextPosition += lastLogTextStep;
        if (area.GetComponent<RectTransform>().sizeDelta.y < Mathf.Abs(lastLogTextPosition)) area.GetComponent<RectTransform>().sizeDelta = new Vector2(area.GetComponent<RectTransform>().sizeDelta.x, Mathf.Abs(lastLogTextPosition));
        panelGameLog.transform.Find("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        newLogEntry.GetComponent<Text>().text = text;
    }

    public void ClickNextPhase()
    {
        Phases.CallNextSubPhase();
    }

    public void ClickPerformAttack()
    {
        Actions.PerformAttack();
    }

    public void ConfirmDiceResults()
    {
        Combat.ConfirmDiceResults();
    }

}
