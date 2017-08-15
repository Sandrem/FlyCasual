using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Todo: Move to different scripts by menu names

public class UI : MonoBehaviour {

    private GameManagerScript Game;

    private float lastLogTextPosition = -5;
    private float lastLogTextStep = -20;

    private int minimapSize = 256;

    public void Initialize()
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
            Game.PrefabsList.ContextMenuPanel.SetActive(true);
            position = FixMenuPosition(Game.PrefabsList.ContextMenuPanel, position);
            Game.PrefabsList.ContextMenuPanel.transform.position = position;
        }
        else
        {
            Game.PrefabsList.ContextMenuPanel.SetActive(false);
        }

    }

    private void HideContextMenuButtons()
    {
        foreach (Transform button in Game.PrefabsList.ContextMenuPanel.transform)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void HideContextMenu()
    {
        Game.PrefabsList.ContextMenuPanel.SetActive(false);
    }

    public void ShowDirectionMenu()
    {
        Game.PrefabsList.ContextMenuPanel.SetActive(false);
        SetAvailableManeurs();
        Game.PrefabsList.DirectionsMenu.transform.position = FixMenuPosition(Game.PrefabsList.DirectionsMenu, Game.PrefabsList.ContextMenuPanel.transform.position);
        Game.PrefabsList.DirectionsMenu.SetActive(true);
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
                Game.PrefabsList.DirectionsMenu.transform.Find("Speed" + maneuverSpeed + "/" + maneuverData.Key).gameObject.SetActive(false);
            }
            else
            {
                Game.PrefabsList.DirectionsMenu.transform.Find("Speed" + maneuverSpeed + "/" + maneuverData.Key).gameObject.SetActive(true);

                GameObject button = Game.PrefabsList.DirectionsMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
                SetManeuverIcon(button, maneuverData);
            }

        }
    }

    private void SetManeuverIcon(GameObject button, KeyValuePair<string, Movement.ManeuverColor> maneuverData)
    {
        Movement.MovementStruct movement = Game.Movement.ManeuverFromString(maneuverData.Key);

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

        Sprite image = Game.PrefabsList.ImageStorageDirections.transform.Find(imageName).GetComponent<Image>().sprite;
        button.GetComponent<Image>().sprite = image;
    }

    public void HideDirectionMenu()
    {
        Game.PrefabsList.DirectionsMenu.SetActive(false);
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
        Game.PrefabsList.GameResultsPanel.transform.Find("Panel/Congratulations").GetComponent<Text>().text = results;
        Game.PrefabsList.GameResultsPanel.SetActive(true);
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
        Game.PrefabsList.MinimapPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(minimapSize, minimapSize);
    }

    public void ToggleMinimap()
    {
        Game.PrefabsList.GameLogPanel.SetActive(false);
        Game.PrefabsList.MinimapPanel.SetActive(!Game.PrefabsList.MinimapPanel.activeSelf);
    }

    public void ToggleGameLog()
    {
        Game.PrefabsList.MinimapPanel.SetActive(false);
        Game.PrefabsList.GameLogPanel.SetActive(!Game.PrefabsList.GameLogPanel.activeSelf);
    }

    public void AddTestLogEntry(string text)
    {
        Transform area = Game.PrefabsList.GameLogPanel.transform.Find("Scroll/Viewport/Content");
        GameObject newLogEntry = Instantiate(Game.PrefabsList.LogText, area);
        newLogEntry.transform.localPosition = new Vector3(5, lastLogTextPosition, 0);
        lastLogTextPosition += lastLogTextStep;
        if (area.GetComponent<RectTransform>().sizeDelta.y < Mathf.Abs(lastLogTextPosition)) area.GetComponent<RectTransform>().sizeDelta = new Vector2(area.GetComponent<RectTransform>().sizeDelta.x, Mathf.Abs(lastLogTextPosition));
        Game.PrefabsList.GameLogPanel.transform.Find("Scroll").GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
        newLogEntry.GetComponent<Text>().text = text;
    }

    public void ShowDecisionsPanel()
    {
        //start subphase
        Game.PrefabsList.PanelDecisions.SetActive(true);
    }

    public void HideDecisionsPanel()
    {
        Game.PrefabsList.PanelDecisions.SetActive(false);
        //end subphase
    }

    public void ClickNextPhase()
    {
        HideNextButton();
        Roster.AllShipsHighlightOff();
        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.CombatSubPhase))
        {
            foreach (var shipHolder in Roster.GetPlayer(Phases.CurrentPhasePlayer).Ships)
            {
                if (shipHolder.Value.PilotSkill == Phases.CurrentSubPhase.RequiredPilotSkill)
                {
                    shipHolder.Value.IsAttackPerformed = true;
                }
            }
        }
        Phases.CallNextSubPhase();
    }

    public void ClickDeclareTarget()
    {
        Combat.DeclareTarget();
    }

    public void CloseActionsPanel()
    {
        Actions.CloseActionsPanel();
        Phases.CurrentSubPhase.CallBack();
    }

    public void ShowNextButton()
    {
        Game.PrefabsList.NextButtonPanel.SetActive(true);
        Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponent<Animator>().enabled = false;
        Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponentInChildren<Text>().text = "NEXT";
    }

    public void ShowSkipButton()
    {
        Game.PrefabsList.NextButtonPanel.SetActive(true);
        Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponentInChildren<Text>().text = "SKIP";
    }

    public void HideNextButton()
    {
        Game.PrefabsList.NextButtonPanel.SetActive(false);

        Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponent<Animator>().enabled = false;

        ColorBlock colors = Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponent<Button>().colors;
        colors.normalColor = new Color32(0, 0, 0, 200);
        Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponent<Button>().colors = colors;
    }

    public void HighlightNextButton()
    {
        Game.PrefabsList.NextButtonPanel.transform.Find("NextButton").GetComponent<Animator>().enabled = true;
    }

    public void CallHideTooltip()
    {
        Tooltips.EndTooltip();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
