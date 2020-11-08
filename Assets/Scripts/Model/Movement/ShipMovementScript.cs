using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Ship;
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

    public List<Func<bool>> FuncsToUpdate = new List<System.Func<bool>>();

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
        List<Func<bool>> subscribedFuncs = new List<Func<bool>>();
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

    public static GenericMovement MovementFromStruct(ManeuverHolder movementStruct)
    {
        GenericMovement result = null;

        if (movementStruct.Bearing == ManeuverBearing.Straight)
        {
            result = new StraightMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.KoiogranTurn)
        {
            result = new KoiogranTurnMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.Turn)
        {
            result = new TurnMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.Bank)
        {
            result = new BankMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.SegnorsLoop)
        {
            result = new SegnorsLoopMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.SegnorsLoopUsingTurnTemplate)
        {
            result = new SegnorsLoopUsingTurnTemplateMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.TallonRoll)
        {
            result = new TallonRollMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.Stationary)
        {
            result = new StationaryMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.SideslipBank)
        {
            result = new SideslipBankMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.SideslipTurn)
        {
            result = new SideslipTurnMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == ManeuverBearing.ReverseStraight)
        {
            if (movementStruct.Direction == ManeuverDirection.Forward)
            {
                result = new ReverseStraightMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
            }
            else
            {
                result = new ReverseBankMovement(movementStruct.SpeedIntUnsigned, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
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
        (Phases.CurrentPhase as MainPhases.ActivationPhase).ActivationShip = Selection.ThisShip;

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
