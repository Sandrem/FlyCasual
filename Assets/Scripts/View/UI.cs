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

    public void ShowError(string text)
    {
        Messages.ShowError(text);
    }

    public void ShowInfo(string text)
    {
        Messages.ShowInfo(text);
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
        foreach (KeyValuePair<string, Ship.ManeuverColor> maneuverData in Selection.ThisShip.GetManeuvers())
        {
            string[] parameters = maneuverData.Key.Split('.');
            string maneuverSpeed = parameters[0];

            if (maneuverData.Value == Ship.ManeuverColor.None)
            {
                Game.PrefabsList.DirectionsMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject.SetActive(false);
            }
            else
            {
                Game.PrefabsList.DirectionsMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject.SetActive(true);

                GameObject button = Game.PrefabsList.DirectionsMenu.transform.Find("Speed" + maneuverSpeed).Find(maneuverData.Key).gameObject;
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
        Game.PrefabsList.GameResultsPanel.transform.GetComponentInChildren<Text>().text = results;
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
        Transform area = Game.PrefabsList.GameLogPanel.transform.Find("Scroll").Find("Viewport").Find("Content");
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
        Phases.CallNextSubPhase();
    }

    public void ClickDeclareTarget()
    {
        Actions.DeclareTarget();
    }

    public void ClickPerformAttack()
    {
        HideDecisionsPanel();

        //TODO: Rework
        Combat.SecondaryWeapon = null;
        Debug.Log("Reset: " + Combat.SecondaryWeapon);

        Actions.PerformAttack();
    }

    public void ClickPerformTorpedoesAttack()
    {
        HideDecisionsPanel();

        //TODO: Get upgrade
        Upgrade.GenericSecondaryWeapon secondaryWeapon = null;
        foreach (var upgrade in Selection.ThisShip.InstalledUpgrades)
        {
            if (upgrade.Key == Upgrade.UpgradeSlot.Torpedoes) secondaryWeapon = upgrade.Value as Upgrade.GenericSecondaryWeapon;
        }
        Combat.SecondaryWeapon = secondaryWeapon;

        Actions.PerformAttack();
    }

    public void ConfirmDiceResults()
    {
        Combat.ConfirmDiceResults();
    }

    public void CloseActionsPanel()
    {
        Actions.CloseActionsPanel();
    }

}
