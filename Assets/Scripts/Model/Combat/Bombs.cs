using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;
using UnityEngine;
using Ship;
using SubPhases;

namespace Bombs
{

    public class BombDetonationEventArgs : EventArgs
    {
        public GenericShip DetonatedShip;
        public GameObject BombObject;
    }

    public enum BombDropTemplates
    {
        Straight1,
        Straight2,
        Straight3,
        Turn1Left,
        Turn1Right,
        Turn3Left,
        Turn3Right
    }

    public static class BombsManager
    {
        public static GenericBomb CurrentBomb { get; set; }
        public static GameObject CurrentBombObject { get; set; }
        public static GenericShip DetonatedShip { get; set; }
        public static bool DetonationIsAllowed { get; set; }

        private static List<Vector3> generatedBombPoints = new List<Vector3>();
        private static Dictionary<GameObject, GenericBomb> bombsList;

        public delegate void EventHandlerBomb(GenericBomb bomb, GameObject model);
        public delegate void EventHandlerBombShip(GenericBomb bomb, GenericShip detonatedShip);
        public static event EventHandlerBomb OnBombIsRemoved;
        public static event EventHandlerBombShip OnCheckPermissionToDetonate;

        public static void Initialize()
        {
            bombsList = new Dictionary<GameObject, GenericBomb>();
            CurrentBomb = null;
        }

        public static List<Vector3> GetBombPoints()
        {
            if (generatedBombPoints.Count == 0)
            {
                int precision = 10;
                for (int i = 0; i <= precision; i++)
                {
                    generatedBombPoints.Add(new Vector3(-1.6f + (3.2f / precision) * precision, 0, 0.05f));
                    generatedBombPoints.Add(new Vector3(1.6f, 0, 0.05f + (3 / precision) * precision));
                    generatedBombPoints.Add(new Vector3(-1.6f, 0, 0.05f + (3 / precision) * precision));
                    generatedBombPoints.Add(new Vector3(-1.6f + (3.2f / precision) * precision, 0, 3.05f));
                }
            }

            return generatedBombPoints;
        }

        public static void RegisterBombs(List<GameObject> bombObjects, GenericBomb bombUpgrade)
        {
            foreach (var bombObject in bombObjects)
            {
                if (!bombsList.ContainsKey(bombObject)) bombsList.Add(bombObject, bombUpgrade);
            }
        }

        public static void UnregisterBomb(GameObject bombObject)
        {
            bombsList.Remove(bombObject);
        }

        public static GenericBomb GetBombByObject(GameObject bombObject)
        {
            return bombsList[bombObject];
        }

        public static List<GenericShip> GetShipsInRange(GameObject bombObject)
        {
            List<GenericShip> result = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Select(n => n.Value))
            {
                if (!ship.IsDestroyed)
                {
                    if (IsShipInDetonationRange(ship, bombObject))
                    {
                        result.Add(ship);
                    }
                }
            }

            return result;
        }

        private static bool IsShipInDetonationRange(GenericShip ship, GameObject bombObject)
        {
            List<Vector3> bombPoints = GetBombPoints();

            foreach (var localBombPoint in bombPoints)
            {
                Vector3 globalBombPoint = bombObject.transform.TransformPoint(localBombPoint);
                foreach (var globalShipBasePoint in ship.ShipBase.GetStandPoints().Select(n => n.Value))
                {
                    if (Board.BoardManager.GetRangeBetweenPoints(globalBombPoint, globalShipBasePoint) == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void ResolveDetonationTriggers()
        {
            Triggers.ResolveTriggers(TriggerTypes.OnBombIsDetonated, ResolveRemoveModelTriggers);
        }

        private static void ResolveRemoveModelTriggers()
        {
            if (OnBombIsRemoved != null) OnBombIsRemoved(CurrentBomb, CurrentBombObject);

            Triggers.ResolveTriggers(TriggerTypes.OnBombIsRemoved, RemoveModel);
        }

        private static void RemoveModel()
        {
            GameObject.Destroy(CurrentBombObject);
            Triggers.FinishTrigger();
        }

        public static void CallGetPermissionToDetonateTrigger(Action callback)
        {
            DetonationIsAllowed = true;
            ToggleReadyToDetonateHighLight(CurrentBombObject, true);

            if (OnCheckPermissionToDetonate != null) OnCheckPermissionToDetonate(CurrentBomb, DetonatedShip);

            Triggers.ResolveTriggers(TriggerTypes.OnCheckPermissionToDetonate, delegate { CheckPermissionToDetonate(callback); });
        }

        private static void CheckPermissionToDetonate(Action callback)
        {
            ToggleReadyToDetonateHighLight(CurrentBombObject, false);

            if (DetonationIsAllowed)
            {
                callback();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private static void ToggleReadyToDetonateHighLight(GameObject bombObject, bool isActive)
        {
            bombObject.transform.Find("Light").gameObject.SetActive(isActive);
        }

        public static void CheckBombDropAvailability(GenericShip ship)
        {
            if (!ship.IsBombAlreadyDropped && HasTimedBombs(ship))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ask what bomb to drop",
                    TriggerType = TriggerTypes.OnMovementActivation,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = AskWhatBombToDrop,
                    Sender = ship
                });
            }
        }

        private static void AskWhatBombToDrop(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip("ShipId:" + (sender as GenericShip).ShipId);

            BombDecisionSubPhase selectBombToDrop = (BombDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select bomb to drop",
                typeof(BombDecisionSubPhase),
                CheckSelectedBomb
            );

            foreach (var timedBombInstalled in GetTimedBombsInstalled(Selection.ThisShip))
            {
                selectBombToDrop.AddDecision(
                    timedBombInstalled.Name,
                    delegate { SelectBomb(timedBombInstalled); }
                );
            }

            selectBombToDrop.AddDecision(
                "None",
                delegate { SelectBomb(null); }
            );

            selectBombToDrop.InfoText = "Select bomb to drop";

            selectBombToDrop.DefaultDecision = "None";

            selectBombToDrop.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private static void SelectBomb(GenericUpgrade timedBombUpgrade)
        {
            CurrentBomb = timedBombUpgrade as GenericTimedBomb;
            DecisionSubPhase.ConfirmDecision();
        }

        private class BombDecisionSubPhase : DecisionSubPhase { }

        private static void CheckSelectedBomb()
        {
            if (CurrentBomb != null)
            {
                if (Selection.ThisShip.CanLaunchBombs)
                {
                    AskWayToDropBomb();
                }
                else
                {
                    DropBombWithoudDecision();
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private static void AskWayToDropBomb()
        {
            WayToDropDecisionSubPhase selectBombToDrop = (WayToDropDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select way to drop bomb",
                typeof(WayToDropDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectBombToDrop.AddDecision("Drop", DropBomb);
            selectBombToDrop.AddDecision("Launch", LaunchBomb);

            selectBombToDrop.InfoText = "Select way to drop the bomb";

            selectBombToDrop.DefaultDecision = "Drop";

            selectBombToDrop.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private static void DropBombWithoudDecision()
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                Triggers.FinishTrigger
            );
        }

        private static void DropBomb(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                DecisionSubPhase.ConfirmDecision
            );
        }

        private static void LaunchBomb(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb launch planning",
                typeof(BombLaunchPlanningSubPhase),
                DecisionSubPhase.ConfirmDecision
            );
        }

        private class WayToDropDecisionSubPhase : DecisionSubPhase { }

        public static List<GenericUpgrade> GetTimedBombsInstalled(GenericShip ship)
        {
            return ship.UpgradeBar.GetUpgradesOnlyFaceup().Where(n => n.GetType().BaseType == typeof(GenericTimedBomb)).ToList();
        }

        public static bool HasTimedBombs(GenericShip ship)
        {
            int timedBombsInstalledCount = GetTimedBombsInstalled(ship).Count;
            return timedBombsInstalledCount > 0;
        }

    }
}



