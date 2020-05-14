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
using Movement;

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

    public static void SendAssignManeuverCommand(string maneuverCode)
    {
        JSONObject parameters = new JSONObject();
        parameters.AddField("id", Selection.ThisShip.ShipId.ToString());
        parameters.AddField("maneuver", maneuverCode);

        GameMode.CurrentGameMode.ExecuteCommand(
            GameController.GenerateGameCommand
            (
                GameCommandTypes.AssignManeuver,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            )
        );
    }

    public static void AssignManeuver(int shipId, string maneuverCode)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        UI.HideContextMenu();

        Selection.ThisShip.SetAssignedManeuver(MovementFromString(maneuverCode));

        DirectionsMenu.FinishManeuverSelections();
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
            result = new Movement.StraightMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.KoiogranTurn)
        {
            result = new Movement.KoiogranTurnMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Turn)
        {
            result = new Movement.TurnMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Bank)
        {
            result = new Movement.BankMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.SegnorsLoop)
        {
            result = new Movement.SegnorsLoopMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.SegnorsLoopUsingTurnTemplate)
        {
            result = new Movement.SegnorsLoopUsingTurnTemplateMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.TallonRoll)
        {
            result = new Movement.TallonRollMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Stationary)
        {
            result = new Movement.StationaryMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.ReverseStraight)
        {
            if (movementStruct.Direction == Movement.ManeuverDirection.Forward)
            {
                result = new Movement.ReverseStraightMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
            }
            else
            {
                result = new Movement.ReverseBankMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
            }
        }

        return result;
    }

    public static GenericMovement CopyMovement(GenericMovement maneuver)
    {
        ManeuverHolder movementStruct = new ManeuverHolder(maneuver.ManeuverSpeed, maneuver.Direction, maneuver.Bearing, maneuver.ColorComplexity);
        return MovementFromStruct(movementStruct);
    }

    public static GenericMovement MovementFromString(string parameters, GenericShip ship = null)
    {
        ManeuverHolder movementStruct = new ManeuverHolder(parameters, ship);
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

        if (!DebugManager.DebugStraightToCombat)
        {
            Selection.ThisShip.CallMovementActivationStart(ReadyRoRevealManeuver);
        }
        else
        {
            FinishMovementAndStartActionDecision();
        }
    }

    private static void ReadyRoRevealManeuver()
    {
        Selection.ThisShip.CallManeuverIsReadyToBeRevealed(RevealManeuver);
    }

    private static void RevealManeuver()
    {
        Selection.ThisShip.CallManeuverIsRevealed(
            delegate { ShipMovementScript.LaunchMovement(FinishMovementAndStartActionDecision); },
            FinishMovementAndStartActionDecision
        );
    }

    public static void FinishMovementAndStartActionDecision()
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
