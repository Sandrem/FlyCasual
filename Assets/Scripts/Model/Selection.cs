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
    	
    public static void Initialize()
    {
        ThisShip = null;
        AnotherShip = null;
        ActiveShip = null;
        HoveredShip = null;
    }

    //TODO: BUG - enemy ship can be selected
    public static void UpdateSelection()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            TryMarkShipByModel();
            int mouseKeyIsPressed = 0;
            if (Input.GetKeyUp(KeyCode.Mouse0)) mouseKeyIsPressed = 1;
            else if(Input.GetKeyUp(KeyCode.Mouse1)) mouseKeyIsPressed = 2;

            if (mouseKeyIsPressed > 0)
            {
                bool isShipHit = false;
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
                {
                    if (hitInfo.transform.tag.StartsWith("ShipId:"))
                    {
                        isShipHit = TryToChangeShip(hitInfo.transform.tag, mouseKeyIsPressed);
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

    public static bool TryToChangeShip(string shipId, int mouseKeyIsPressed = 1)
    {
        bool result = false;

        Ship.GenericShip ship = Roster.GetShipById(shipId);
        if (ship.Owner.PlayerNo == Phases.CurrentSubPhase.RequiredPlayer)
        {
            result = TryToChangeThisShip(shipId, mouseKeyIsPressed);
        }
        else
        {
            result = TryToChangeAnotherShip(shipId, mouseKeyIsPressed);
        }
        return result;
    }

    private static void ProcessClick()
    {
        if (Phases.CurrentSubPhase != null) Phases.CurrentSubPhase.ProcessClick();
    }

    //TODO: call from roster info panel click too
    public static bool TryToChangeAnotherShip(string shipId, int mouseKeyIsPressed = 1)
    {
        bool result = false;
        Ship.GenericShip targetShip = Roster.GetShipById(shipId);
        result = Phases.CurrentSubPhase.AnotherShipCanBeSelected(targetShip, mouseKeyIsPressed);

        if (result == true)
        {
            ChangeAnotherShip(shipId);
            DoSelectAnotherShip(mouseKeyIsPressed);
        }
        return result;
    }

    public static bool TryToChangeThisShip(string shipId, int mouseKeyIsPressed = 1)
    {
        bool result = false;

        Ship.GenericShip ship = Roster.GetShipById(shipId);

        result = Phases.CurrentSubPhase.ThisShipCanBeSelected(ship, mouseKeyIsPressed);

        if (result == true)
        {
            Selection.ChangeActiveShip(shipId);
            DoSelectThisShip(mouseKeyIsPressed);
        }

        return result;
    }

    public static void ChangeActiveShip(string shipId)
    {
        DeselectThisShip();
        ThisShip = Roster.GetShipById(shipId);
        ChangeActiveShipUsingThisShip ();
    }

    public static void ChangeActiveShip(Ship.GenericShip genShip)
    {
        DeselectThisShip();
        ThisShip = genShip;
        ChangeActiveShipUsingThisShip ();
    }

    private static void ChangeActiveShipUsingThisShip()
    {
        ThisShip.ToggleCollisionDetection(true);
        Roster.MarkShip(ThisShip, Color.green);
        ThisShip.HighlightThisSelected();
    }

    private static void DoSelectThisShip(int mouseKeyIsPressed)
    {
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer)) Phases.CurrentSubPhase.DoSelectThisShip(ThisShip, mouseKeyIsPressed);
    }

    public static void DeselectThisShip()
    {
        if (ThisShip != null)
        {
            DeselectShip(ThisShip);
            ThisShip = null;
        }
    }

    public static void ChangeAnotherShip(string shipId)
    {
        if (AnotherShip != null)
        {
            Roster.UnMarkShip(AnotherShip);
            AnotherShip.HighlightSelectedOff();
        }
        AnotherShip = Roster.GetShipById(shipId);
        Roster.MarkShip(AnotherShip, Color.red);
        AnotherShip.HighlightEnemySelected();
    }

    private static void DoSelectAnotherShip(int mouseKeyIsPressed)
    {
        if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer)) Phases.CurrentSubPhase.DoSelectAnotherShip(AnotherShip, mouseKeyIsPressed);
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
