using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GameModes;
using Ship;
using System.Linq;

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

        Test();
    }

    private void Test()
    {
        GenericShip ship1 = Roster.GetPlayer(Players.PlayerNo.Player1).Ships.FirstOrDefault().Value;
        GenericShip ship2 = Roster.GetPlayer(Players.PlayerNo.Player2).Ships.FirstOrDefault().Value;
        if (ship1 != null && ship2 != null)
        {
            Board.RangeInfo rangeInfo = new Board.RangeInfo(ship1, ship2);

            GameObject distanceChecker = GameObject.Find("SceneHolder/Board/DistanceTester");
            distanceChecker.transform.position = rangeInfo.MinDistance.Point1;
            distanceChecker.transform.LookAt(rangeInfo.MinDistance.Point2);
            distanceChecker.transform.localScale = new Vector3(1, 1, rangeInfo.MinDistance.Distance / Board.BoardManager.DISTANCE_INTO_RANGE);

            /*GameObject distanceCheckerAlt1 = GameObject.Find("SceneHolder/Board/DistanceTesterAlt1");
            distanceCheckerAlt1.transform.position = rangeInfo.AltDistance1.Point1;
            distanceCheckerAlt1.transform.LookAt(rangeInfo.AltDistance1.Point2);
            distanceCheckerAlt1.transform.localScale = new Vector3(1, 1, rangeInfo.AltDistance1.Distance / Board.BoardManager.DISTANCE_INTO_RANGE);

            GameObject distanceCheckerAlt2 = GameObject.Find("SceneHolder/Board/DistanceTesterAlt2");
            distanceCheckerAlt2.transform.position = rangeInfo.AltDistance2.Point1;
            distanceCheckerAlt2.transform.LookAt(rangeInfo.AltDistance2.Point2);
            distanceCheckerAlt2.transform.localScale = new Vector3(1, 1, rangeInfo.AltDistance2.Distance / Board.BoardManager.DISTANCE_INTO_RANGE);*/
        }
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

    public static void AssignManeuver(int shipId, string maneuverCode)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);
        UI.HideContextMenu();

        Selection.ThisShip.SetAssignedManeuver(MovementFromString(maneuverCode));

        if (Phases.CurrentSubPhase.GetType() == typeof(SubPhases.PlanningSubPhase))
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

    public static Movement.GenericMovement MovementFromStruct(Movement.MovementStruct movementStruct)
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
        else if (movementStruct.Bearing == Movement.ManeuverBearing.TallonRoll)
        {
            result = new Movement.TallonRollMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }
        else if (movementStruct.Bearing == Movement.ManeuverBearing.Stationary)
        {
            result = new Movement.StationaryMovement(movementStruct.SpeedInt, movementStruct.Direction, movementStruct.Bearing, movementStruct.ColorComplexity);
        }

        return result;
    }

    public static Movement.GenericMovement MovementFromString(string parameters, Ship.GenericShip ship = null)
    {
        Movement.MovementStruct movementStruct = new Movement.MovementStruct(parameters, ship);
        return MovementFromStruct(movementStruct);
    }

    public void PerformStoredManeuverButtonIsPressed()
    {
        GameMode.CurrentGameMode.PerformStoredManeuver(Selection.ThisShip.ShipId);
    }

    private static void DoMovementTriggerHandler(object sender, System.EventArgs e)
    {
        Phases.StartTemporarySubPhaseOld("Movement", typeof(SubPhases.MovementExecutionSubPhase));
    }

    public static void PerformStoredManeuver(int shipId)
    {
        Selection.ChangeActiveShip("ShipId:" + shipId);

        UI.HideContextMenu();

        Selection.ThisShip.CallMovementActivation(LaunchMovementTrigger);
    }

    private static void LaunchMovementTrigger()
    {
        Triggers.RegisterTrigger(new Trigger()
        {
            Name = "Maneuver",
            TriggerType = TriggerTypes.OnManeuver,
            TriggerOwner = Selection.ThisShip.Owner.PlayerNo,
            EventHandler = DoMovementTriggerHandler
        });

        Triggers.ResolveTriggers(TriggerTypes.OnManeuver, FinishManeuverExecution);
    }

    private static void FinishManeuverExecution()
    {
        GameMode.CurrentGameMode.FinishMovementExecution();
    }

}
