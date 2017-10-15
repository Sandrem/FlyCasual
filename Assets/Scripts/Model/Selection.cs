using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class Selection {

    public static Ship.GenericShip ThisShip;
    public static Ship.GenericShip AnotherShip;
    public static Ship.GenericShip ActiveShip;
    public static Ship.GenericShip HoveredShip;
	
    //TODO: BUG - enemy ship can be selected
    public static void UpdateSelection()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            TryMarkShipByModel();
            if (Input.GetKeyUp(KeyCode.Mouse0) == true)
            {
                bool isShipHit = false;
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    if (hitInfo.transform.tag.StartsWith("ShipId:"))
                    {
                        isShipHit = TryToChangeShip(hitInfo.transform.tag);
                    }
                }
                if (!isShipHit)
                {
                    ProcessClick();
                    UI.HideTemporaryMenus();
                }
            }
        }
    }

    private static void TryMarkShipByModel()
    {
        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
        {
            TryMarkShip(hitInfo.transform.tag);
        }
    }

    public static void TryMarkShip(string tag)
    {
        if (tag.StartsWith("ShipId:"))
        {
            TryUnmarkPreviousHoveredShip();
            HoveredShip = Roster.AllShips[tag];
            if ((HoveredShip != ThisShip) && (HoveredShip != AnotherShip))
            {
                HoveredShip.HighlightAnyHovered();
                Roster.MarkShip(HoveredShip, Color.yellow);
            }
        }
        else
        {
            TryUnmarkPreviousHoveredShip();
        }
    }

    public static void TryUnmarkPreviousHoveredShip()
    {
        if (HoveredShip != null)
        {
            if ((HoveredShip != ThisShip) && (HoveredShip != AnotherShip))
            {
                HoveredShip.HighlightSelectedOff();
                Roster.UnMarkShip(HoveredShip);
                HoveredShip = null;
            }
        }
    }

    public static bool TryToChangeShip(string shipId)
    {
        bool result = false;

        Ship.GenericShip ship = Roster.GetShipById(shipId);
        if (ship.Owner.PlayerNo == Phases.CurrentSubPhase.RequiredPlayer)
        {
            result = TryToChangeThisShip(shipId);
        }
        else
        {
            result = TryToChangeAnotherShip(shipId);
        }
        return result;
    }

    private static void ProcessClick()
    {
        Phases.CurrentSubPhase.ProcessClick();
        GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
            ChangeActiveShip(shipId);
        }

        return result;
    }

    public static void ChangeActiveShip(string shipId)
    {
        DeselectThisShip();
        ThisShip = Roster.GetShipById(shipId);
        ThisShip.ToggleCollisionDetection(true);
        Roster.MarkShip(ThisShip, Color.green);
        ThisShip.HighlightThisSelected();
        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.CombatSubPhase)) Roster.HighlightShipsFiltered(Roster.AnotherPlayer(Phases.CurrentPhasePlayer));
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer)) UI.CallContextMenu(ThisShip);
    }

    public static void DeselectThisShip()
    {
        if (ThisShip != null)
        {
            DeselectShip(ThisShip);
            ThisShip = null;
        }
    }

    public static bool ChangeAnotherShip(string shipId)
    {
        //Should I can target my own ships???
        if (AnotherShip != null)
        {
            Roster.UnMarkShip(AnotherShip);
            AnotherShip.HighlightSelectedOff();
        }
        AnotherShip = Roster.GetShipById(shipId);
        Roster.MarkShip(AnotherShip, Color.red);
        AnotherShip.HighlightEnemySelected();
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer)) UI.CallContextMenu(AnotherShip);
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
        ship.ToggleCollisionDetection(false);
        Roster.UnMarkShip(ship);
        ship.HighlightSelectedOff();
    }

    public static void DeselectAllShips()
    {
        DeselectThisShip();
        DeselectAnotherShip();
    }

}
