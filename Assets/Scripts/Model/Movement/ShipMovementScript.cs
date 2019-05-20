using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameModes;
using Ship;
using System.Linq;
using SubPhases;
using System;
using GameCommands;

public class ShipMovementScript : MonoBehaviour {

    //TODO: Refactor old
    public Collider CollidedWith;
    public Collider ObstacleEnter;
    public Collider ObstacleExit;
    public Collider ObstacleHitEnter;
    public Collider ObstacleHitExit;

    public static Action ExtraMovementCallback;

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

    public static void SendAssignManeuverCommand(int shipId, string maneuverCode)
    {
        JSONObject parameters = new JSONObject();
        parameters.AddField("id", shipId.ToString());
        parameters.AddField("maneuver", maneuverCode);
        GameController.SendCommand(
            GameCommandTypes.AssignManeuver,
            Phases.CurrentSubPhase.GetType(),
            parameters.ToString()
        );
    }

    public static void AssignManeuver(int shipId, string maneuverCode)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        UI.HideContextMenu();

        Selection.ThisShip.SetAssignedManeuver(MovementFromString(maneuverCode));

        if (Phases.CurrentSubPhase.GetType() == typeof(PlanningSubPhase))
        {
            Roster.HighlightShipOff(Selection.ThisShip);

            if (Roster.AllManuversAreAssigned(Phases.CurrentPhasePlayer))
            {
                UI.ShowNextButton();
                UI.HighlightNextButton();
            }
        }
        else
        {
            Triggers.FinishTrigger();
        }
    }

    public static void AssignManeuverSimple(int shipId, string maneuverCode)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        Selection.ThisShip.SetAssignedManeuver(MovementFromString(maneuverCode));
    }

    public static Movement.GenericMovement MovementFromStruct(Movement.ManeuverHolder movementStruct)
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
        else if (movementStruct.Bearing == Movement.ManeuverBearing.SegnorsLoop)
        {
            result = new Movement.SegnorsLoopMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.SegnorsLoopUsingTurnTemplate)
        {
            result = new Movement.SegnorsLoopUsingTurnTemplateMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.TallonRoll)
        {
            result = new Movement.TallonRollMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Stationary)
        {
            result = new Movement.StationaryMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Reverse)
        {
            if (movementStruct.Direction == Movement.ManeuverDirection.Forward)
            {
                result = new Movement.ReverseStraightMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
            }
            else
            {
                result = new Movement.ReverseBankMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
            }
        }

        return result;
    }

    public static Movement.GenericMovement MovementFromString(string parameters, Ship.GenericShip ship = null)
    {
        Movement.ManeuverHolder movementStruct = new Movement.ManeuverHolder(parameters, ship);
        return MovementFromStruct(movementStruct);
    }

    public static GameCommand GenerateActivateAndMoveCommand(int shipId)
    {
        JSONObject parameters = new JSONObject();
        parameters.AddField("id", shipId.ToString());
        return GameController.GenerateGameCommand(
            GameCommandTypes.ActivateAndMove,
            Phases.CurrentSubPhase.GetType(),
            parameters.ToString()
        );
    }

    public static void ActivateAndMove(int shipId)
    {
        Phases.CurrentSubPhase.IsReadyForCommands = false;

        Selection.ChangeActiveShip("ShipId:" + shipId);

        UI.HideContextMenu();

        Selection.ThisShip.CallMovementActivationStart(ReadyRoRevealManeuver);
    }

    private static void ReadyRoRevealManeuver()
    {
        Selection.ThisShip.CallManeuverIsReadyToBeRevealed(RevealManeuver);
    }

    private static void RevealManeuver()
    {
        Selection.ThisShip.CallManeuverIsRevealed(
            delegate { ShipMovementScript.LaunchMovement(FinishMovementAndStartActionDecision); }
        );
    }

    private static void FinishMovementAndStartActionDecision()
    {
        GenericSubPhase actionSubPhase = new ActionSubPhase();
        actionSubPhase.PreviousSubPhase = Phases.CurrentSubPhase;

        Phases.CurrentSubPhase = actionSubPhase;
        Phases.CurrentSubPhase.Start();
        Phases.CurrentSubPhase.Initialize();
    }

    public static void LaunchMovement(Action callback)
    {
        if (callback == null) callback = ExtraMovementCallback;

        LaunchMovementPrepared(delegate {
            Phases.FinishSubPhase(typeof(MovementExecutionSubPhase));
            callback();
        });
    }

    private static void LaunchMovementPrepared(Action callback)
    {
        Triggers.RegisterTrigger(new Trigger()
        {
            Name = "Maneuver",
            TriggerType = TriggerTypes.OnManeuver,
            TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
            EventHandler = delegate { Phases.StartTemporarySubPhaseOld("Movement", typeof(MovementExecutionSubPhase)); }
        });

        Triggers.ResolveTriggers(
            TriggerTypes.OnManeuver,
            delegate{
                ExtraMovementCallback = null;
                callback();
            }
        );
    }

}
