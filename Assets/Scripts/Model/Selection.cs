using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class Selection {

    private static GameManagerScript Game;

    public static Ship.GenericShip ThisShip;
    public static Ship.GenericShip AnotherShip;
    public static Ship.GenericShip ActiveShip;

    // Use this for initialization
    static Selection() {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }
	
    //TODO: BUG - enemy ship can be selected
    public static void UpdateSelection()
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

    public static bool TryToChangeShip(string shipId)
    {
        bool result = false;

        Ship.GenericShip ship = Roster.GetShipById(shipId);
        if (ship.Owner.PlayerNo == Phases.CurrentSubPhase.RequiredPlayer)
        {
            result = Selection.TryToChangeThisShip(shipId);
        }
        else
        {
            result = TryToChangeAnotherShip(shipId);
        }
        return result;
    }

    private static void ProcessClick()
    {
        if (Game.Position.inReposition)
        {
            Game.Position.TryConfirmPosition(Selection.ThisShip);
        }
    }

    //TODO: call from roster info panel click too
    public static bool TryToChangeAnotherShip(string shipId)
    {
        bool result = false;
        Ship.GenericShip targetShip = Roster.GetShipById(shipId);
        result = Phases.CurrentSubPhase.AnotherShipCanBeSelected(targetShip);

        if (result == true)
        {
            ChangeAnotherShip(shipId);
        }
        return result;
    }

    public static bool TryToChangeThisShip(string shipId)
    {
        bool result = false;

        Ship.GenericShip ship = Roster.GetShipById(shipId);

        result = Phases.CurrentSubPhase.ThisShipCanBeSelected(ship);

        if (result == true)
        {
            Selection.ChangeActiveShip(shipId);
        }

        return result;
    }

    public static void ChangeActiveShip(string shipId)
    {
        DeselectThisShip();
        ThisShip = Roster.GetShipById(shipId);
        ThisShip.GetShipStandScript().checkCollisions = true;
        ThisShip.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.yellow;
        ThisShip.ApplyShader("selectedYellow");
        Game.UI.CallContextMenu(ThisShip);

    }

    public static void DeselectThisShip()
    {
        if (ThisShip != null)
        {
            DeselectShip(ThisShip);
            ThisShip = null;
        }
    }

    private static bool ChangeAnotherShip(string shipId)
    {
        //Should I can target my own ships???
        if (AnotherShip != null)
        {
            AnotherShip.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.white;
            AnotherShip.ApplyShader("default");
        }
        AnotherShip = Roster.GetShipById(shipId);
        AnotherShip.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.red;
        AnotherShip.ApplyShader("selectedRed");
        Game.UI.CallContextMenu(AnotherShip);
        return true;
    }

    public static void DeselectAnotherShip()
    {
        if (AnotherShip != null)
        {
            DeselectShip(AnotherShip);
            AnotherShip = null;
        }
    }

    private static void DeselectShip(Ship.GenericShip ship)
    {
        ship.GetShipStandScript().checkCollisions = false;
        ship.InfoPanel.transform.Find("ShipInfo").Find("ShipPilotNameText").GetComponent<Text>().color = Color.white;
        ship.ApplyShader("default");
    }

    public static void DeselectAllShips()
    {
        DeselectThisShip();
        DeselectAnotherShip();
    }

}
