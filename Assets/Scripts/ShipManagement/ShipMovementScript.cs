using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShipMovementScript : MonoBehaviour {

    //TODO: Refactor old
    public Collider CollidedWith;
    public Collider ObstacleEnter;
    public Collider ObstacleExit;
    public Collider ObstacleHitEnter;
    public Collider ObstacleHitExit;

    public List<System.Func<bool>> FuncsToUpdate = new List<System.Func<bool>>();

    public bool isMoving;

    void Update ()
    {
        Selection.UpdateSelection();
        UpdateMovement();
        UpdateSubscribedFuncs();

        ClearCollision();
    }

    private void UpdateMovement()
    {
        if (isMoving)
        {
            Selection.ThisShip.AssignedManeuver.UpdateMovementExecution();
        }
    }

    private void UpdateSubscribedFuncs()
    {
        List<System.Func<bool>> subscribedFuncs = new List<System.Func<bool>>();
        subscribedFuncs.AddRange(FuncsToUpdate);

        foreach (var func in subscribedFuncs)
        {
            bool isFinished = func();
            if (isFinished) FuncsToUpdate.Remove(func);
        }        
    }

    private void ClearCollision()
    {
        CollidedWith = null;
    }

    //Assignment and launch of execution of meneuver

    public void AssignManeuver()
    {
        string parameters = EventSystem.current.currentSelectedGameObject.name;

        Selection.ThisShip.AssignedManeuver = MovementFromString(parameters);

        UI.HideDirectionMenu();

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase))
        {
            Selection.ThisShip.InfoPanel.transform.Find("DialAssigned" + Selection.ThisShip.Owner.Id).gameObject.SetActive(true);
            Roster.HighlightShipOff(Selection.ThisShip);

            if (Roster.AllManuersAreAssigned(Phases.CurrentPhasePlayer))
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }
        }
        else
        {
            Messages.ShowInfo("Finish trigger");
            Triggers.FinishTrigger();
        }
    }

    public Movement.GenericMovement MovementFromStruct(Movement.MovementStruct movementStruct)
    {
        Movement.GenericMovement result = null;

        if (movementStruct.Bearing == Movement.ManeuverBearing.Straight)
        {
            result = new Movement.StraightMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.KoiogranTurn)
        {
            result = new Movement.KoiogranTurnMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Turn)
        {
            result = new Movement.TurnMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Bank)
        {
            result = new Movement.BankMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }

        return result;
    }

    public Movement.GenericMovement MovementFromString(string parameters)
    {
        Movement.MovementStruct movementStruct = new Movement.MovementStruct(parameters);

        return MovementFromStruct(movementStruct);
    }

    public void PerformStoredManeuver()
    {
        Phases.StartTemporarySubPhase("Movement", typeof(SubPhases.MovementExecutionSubPhase));
    }

}
