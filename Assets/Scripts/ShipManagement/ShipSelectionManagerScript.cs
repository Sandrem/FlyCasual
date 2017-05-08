using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShipSelectionManagerScript : MonoBehaviour {

    private GameManagerScript Game;

    public Ship.GenericShip ThisShip;
    public Ship.GenericShip AnotherShip;
    public Ship.GenericShip ActiveShip;

    public bool isUIlocked = false;
    public bool isInTemporaryState = false;

    // Use this for initialization
    void Start () {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateSelection();
    }

    //TODO: BUG - enemy ship can be selected
    private void UpdateSelection()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) == true)
            {
                bool isShipHit = false;
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    if (hitInfo.transform.tag != "Untagged")
                    {
                        isShipHit = TryToChangeShip(hitInfo.transform.tag);
                    }
                }
                if (!isShipHit)
                {
                    ProcessClick();
                    Game.UI.HideTemporaryMenus();
                }
            }
        }
    }

    public bool TryToChangeShip(string shipId)
    {
        bool result = false;

        Ship.GenericShip ship = Game.Roster.GetShipById(shipId);
        if (ship.PlayerNo == Game.PhaseManager.CurrentPhase.RequiredPlayer)
        {
            result = TryToChangeThisShip(shipId);
        }
        else
        {
            result = TryToChangeAnotherShip(shipId);
        }
        return result;
    }

    public bool TryToChangeThisShip(string shipId)
    {
        bool result = false;

        if ((!isUIlocked) && (!isInTemporaryState))
        {
            if (!Game.Position.inReposition)
            {
                Ship.GenericShip ship = Game.Roster.GetShipById(shipId);

                result = Game.PhaseManager.CurrentPhase.ThisShipCanBeSelected(ship);

                if (result == true)
                {
                    ChangeActiveShip(shipId);
                }
            }

        }
        return result;
    }

    private void ProcessClick()
    {
        if (Game.Position.inReposition)
        {
            Game.Position.TryConfirmPosition(Game.Selection.ThisShip);
        }
    }

    //TODO: call from roster info panel click too
    public bool TryToChangeAnotherShip(string shipId)
    {
        bool result = false;
        if ((!isUIlocked) && (!isInTemporaryState))
        {
            Ship.GenericShip targetShip = Game.Roster.GetShipById(shipId);

            if (Game.PhaseManager.CurrentPhase.Phase == Phases.Combat)
            {
                if (ThisShip != null)
                {
                    if (targetShip.PlayerNo != Game.PhaseManager.CurrentPhase.RequiredPlayer)
                    {
                        result = true;
                    }
                    else
                    {
                        Game.UI.ShowError("Ship cannot be selected as attack target: Friendly ship");
                    }
                }
                else
                {
                    Game.UI.ShowError("Ship cannot be selected as attack target:\nFirst select attacker");
                }
            } else
            {
                Game.UI.ShowError("Ship of another player");
            }

            if (result == true)
            {
                ChangeAnotherShip(shipId);
            }

        }
        return result;
    }

    private void ChangeActiveShip(string shipId)
    {
        DeselectThisShip();
        ThisShip = Game.Roster.GetShipById(shipId);
        ThisShip.Model.GetShipStandScript().checkCollisions = true;
        ThisShip.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.yellow;
        ThisShip.Model.ApplyShader("selectedYellow");
        Game.UI.CallContextMenu(ThisShip);

    }

    public void DeselectThisShip()
    {
        if (ThisShip != null)
        {
            DeselectShip(ThisShip);
            ThisShip = null;
        }
    }

    private bool ChangeAnotherShip(string shipId)
    {
        //Should I can target my own ships???
        if (AnotherShip != null)
        {
            AnotherShip.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.white;
            AnotherShip.Model.ApplyShader("default");
        }
        AnotherShip = Game.Roster.GetShipById(shipId);
        AnotherShip.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.red;
        AnotherShip.Model.ApplyShader("selectedRed");
        Game.UI.CallContextMenu(AnotherShip);
        return true;
    }

    public void DeselectAnotherShip()
    {
        if (AnotherShip != null)
        {
            DeselectShip(AnotherShip);
            AnotherShip = null;
        }
    }

    private void DeselectShip(Ship.GenericShip ship)
    {
        ship.Model.GetShipStandScript().checkCollisions = false;
        ship.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.white;
        ship.Model.ApplyShader("default");
    }

    public void DeselectAllShips()
    {
        DeselectThisShip();
        DeselectAnotherShip();
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
