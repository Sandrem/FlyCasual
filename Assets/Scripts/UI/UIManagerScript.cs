using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public class UIManagerScript: MonoBehaviour {

    private GameManagerScript Game;

    public MessageManagerScript ErrorManager;
    public RosterInfoScript Roster;
    public DiceResultsScript DiceResults;
    public ActionsPanelScript ActionsPanel;
    public HelpInfoScript Helper;

    public GameObject panelDirectionMenu;
    public GameObject panelContextMenu;
    public GameObject panelGameResultsMessage;
    public GameObject panelMinimap;
    public GameObject panelGameLog;

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
        if (CountActiveButtons(ship) > 0)
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

    private int CountActiveButtons(Ship.GenericShip ship)
    {
        int result = 0;
        switch (Game.PhaseManager.CurrentPhase.Phase)
        {
            case Phases.Planning:
                panelContextMenu.transform.Find("MoveMenuButton").gameObject.SetActive(true);
                result++;
                break;
            case Phases.Activation:
                if (Game.Selection.ThisShip.AssignedManeuver != null)
                {
                    panelContextMenu.transform.Find("MovePerformButton").gameObject.SetActive(true);
                    result++;
                }
                else
                {
                    ShowError("This ship has already executed his maneuver");
                };
                break;
            case Phases.Combat:
                if (ship.PlayerNo != Game.PhaseManager.CurrentPhase.RequiredPlayer)
                {
                    if (Game.Selection.ThisShip.IsAttackPerformed != true)
                    {
                        panelContextMenu.transform.Find("FireButton").gameObject.SetActive(true);
                        result++;
                    }
                    else
                    {
                        ShowError("Your ship has already attacked");
                    }
                }
                break;
            default:
                break;
        }

        return result;
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
        foreach (KeyValuePair<string, Ship.ManeuverColor> maneuverData in Game.Selection.ThisShip.Maneuvers)
        {
            string[] parameters = maneuverData.Key.Split('.');
            string maneuverSpeed = parameters[0];
            //TODO: Should I show red maneuvers if I have stress?
            if ((maneuverData.Value == Ship.ManeuverColor.None) || ((maneuverData.Value == Ship.ManeuverColor.Red) && (Game.Selection.ThisShip.HasToken(Ship.Token.Stress))))
            {
                panelDirectionMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject.SetActive(false);
            }
            else
            {
                panelDirectionMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject.SetActive(true);
                Color maneuverCompexity = Color.cyan;
                switch (maneuverData.Value)
                {
                    case Ship.ManeuverColor.White:
                        maneuverCompexity = Color.white;
                        break;
                    case Ship.ManeuverColor.Green:
                        maneuverCompexity = Color.green;
                        break;
                    case Ship.ManeuverColor.Red:
                        maneuverCompexity = Color.red;
                        break;
                }
                panelDirectionMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).Find("Text").GetComponent<Text>().color = maneuverCompexity;
            }
            
        }
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

    //TODO: move
    public Player PlayerFromInt(int playerNo)
    {
        Player result = Player.None;
        if (playerNo == 1) result = Player.Player1;
        if (playerNo == 2) result = Player.Player2;
        return result;
    }

}
