using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;
using UnityEngine;
using Ship;
using SubPhases;
using BoardTools;

namespace Bombs
{

    public class BombDetonationEventArgs : EventArgs
    {
        public GenericShip DetonatedShip;
        public GenericDeviceGameObject BombObject;
    }

    public enum BombDropTemplates
    {
        Straight_1,
        Bank_1_Left,
        Bank_1_Right,
        Straight_2,
        Straight_3,
        Turn_1_Left,
        Turn_1_Right,
        Turn_3_Left,
        Turn_3_Right
    }

    public static class BombsManager
    {
        public static GenericBomb CurrentBomb { get; set; }
        public static GenericDeviceGameObject CurrentBombObject { get; set; }
        public static GenericShip DetonatedShip { get; set; }
        public static bool DetonationIsAllowed { get; set; }

        private static List<Vector3> generatedBombPoints = new List<Vector3>();
        private static Dictionary<GenericDeviceGameObject, GenericBomb> bombsList;

        public delegate void EventHandlerBomb(GenericBomb bomb, GenericDeviceGameObject model);
        public delegate void EventHandlerBombShip(GenericBomb bomb, GenericShip detonatedShip);
        public static event EventHandlerBomb OnBombIsRemoved;
        public static event EventHandlerBombShip OnCheckPermissionToDetonate;

        public static bool IsOverriden = false;

        public static void Initialize()
        {
            bombsList = new Dictionary<GenericDeviceGameObject, GenericBomb>();
            CurrentBomb = null;
        }

        private static List<Vector3> GetBombPointsRelative()
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

        public static void RegisterBombs(List<GenericDeviceGameObject> bombObjects, GenericBomb bombUpgrade)
        {
            foreach (var bombObject in bombObjects)
            {
                if (!bombsList.ContainsKey(bombObject)) bombsList.Add(bombObject, bombUpgrade);

                MeshCollider collider = bombObject.transform.Find("Model").GetComponent<MeshCollider>();
                if (collider != null) Board.Objects.Add(collider);
            }

            BombsManager.CurrentBombObject = bombObjects.FirstOrDefault();
            BombsManager.CurrentBomb = bombUpgrade;
        }

        public static void UnregisterBomb(GenericDeviceGameObject bombObject)
        {
            bombsList.Remove(bombObject);

            MeshCollider collider = bombObject.transform.Find("Model").GetComponent<MeshCollider>();
            if (collider != null) Board.Objects.Remove(collider);
        }

        public static GenericBomb GetBombByObject(GenericDeviceGameObject bombObject)
        {
            return bombsList[bombObject];
        }

        public static List<GenericShip> GetShipsInRange(GenericDeviceGameObject bombObject)
        {
            List<GenericShip> result = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Select(n => n.Value))
            {
                if (!ship.IsDestroyed)
                {
                    if (IsShipInRange(ship, bombObject, bombObject.ParentUpgrade.detonationRange))
                    {
                        result.Add(ship);
                    }
                }
            }

            return result;
        }

        public static List<GenericBomb> GetBombsInRange(GenericShip ship)
        {
            List<GenericBomb> result = new List<GenericBomb>();

            foreach (var bombHolder in bombsList)
            {
                if (IsShipInRange(ship, bombHolder.Key, bombHolder.Key.ParentUpgrade.detonationRange))
                {
                    result.Add(bombHolder.Value);
                }
            }

            return result;
        }

        public static bool IsShipInRange(GenericShip ship, GenericDeviceGameObject bombObject, int range = 1)
        {
            List<Vector3> bombPoints = GetBombPointsRelative();

            foreach (var localBombPoint in bombPoints)
            {
                Vector3 globalBombPoint = bombObject.transform.TransformPoint(localBombPoint);
                foreach (var globalShipBasePoint in ship.ShipBase.GetStandPoints().Select(n => n.Value))
                {
                    if (Board.GetRangeBetweenPoints(globalBombPoint, globalShipBasePoint) <= range)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static List<Vector3> GetBombPoints(GenericBomb bomb)
        {
            List<Vector3> globalPoints = new List<Vector3>();
            foreach (Vector3 relativePoint in GetBombPointsRelative())
            {
                Vector3 globalBombPoint = bomb.CurrentBombObjects.First().transform.TransformPoint(relativePoint);
                globalPoints.Add(globalBombPoint);
            }
            return globalPoints;
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
            GameObject.Destroy(CurrentBombObject.Model);
            Triggers.FinishTrigger();
        }

        public static void CallGetPermissionToDetonateTrigger(Action callback)
        {            
            DetonationIsAllowed = true;
            ToggleReadyToDetonateHighLight(true);

            Rules.Fuse.CheckForRemoveFuseInsteadOfDetonating(CurrentBombObject);

            if (OnCheckPermissionToDetonate != null) OnCheckPermissionToDetonate(CurrentBomb, DetonatedShip);

            Triggers.ResolveTriggers(TriggerTypes.OnCheckPermissionToDetonate, delegate { CheckPermissionToDetonate(callback); });
        }

        private static void CheckPermissionToDetonate(Action callback)
        {
            ToggleReadyToDetonateHighLight(false);

            if (DetonationIsAllowed)
            {
                callback();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public static void ToggleReadyToDetonateHighLight(bool isActive)
        {
            CurrentBombObject.transform.Find("Light").gameObject.SetActive(isActive);
        }

        public static void CheckBombDropAvailability(GenericShip ship)
        {
            CheckBombDropAvailability(ship, TriggerTypes.OnMovementActivationStart);
        }

        public static void CheckBombDropAvailability(GenericShip ship, TriggerTypes triggerType, UpgradeSubType subType = UpgradeSubType.None, bool onlyDrop = false, bool isRealDrop = true)
        {
            if ((!isRealDrop || !ship.IsBombAlreadyDropped) && HasBombsToDrop(ship, subType))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ask which bomb to drop",
                    TriggerType = TriggerTypes.OnMovementActivationStart,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = (object sender, EventArgs e) => CreateAskBombDropSubPhase((sender as GenericShip), subType, onlyDrop),
                    Sender = ship
                });
            }
        }

        public static void CreateAskBombDropSubPhase(GenericShip ship, UpgradeSubType subType = UpgradeSubType.None, bool onlyDrop = false)
        {
            Selection.ChangeActiveShip("ShipId:" + ship.ShipId);

            BombDecisionSubPhase selectBombToDrop = (BombDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select a device to drop",
                typeof(BombDecisionSubPhase),
                delegate { CheckSelectedBomb(onlyDrop); }
            );

            foreach (var deviceInstalled in GetBombsToDrop(Selection.ThisShip, subType))
            {
                selectBombToDrop.AddDecision(
                    deviceInstalled.UpgradeInfo.Name,
                    delegate { SelectBomb(deviceInstalled); }
                );
            }

            selectBombToDrop.AddDecision(
                "None",
                delegate { SelectBomb(null); }
            );

            selectBombToDrop.DescriptionShort = "Select a bomb to drop";

            selectBombToDrop.DefaultDecisionName = "None";

            selectBombToDrop.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private static void SelectBomb(GenericUpgrade timedBombUpgrade)
        {
            CurrentBomb = timedBombUpgrade as GenericBomb;
            DecisionSubPhase.ConfirmDecision();
        }

        private class BombDecisionSubPhase : DecisionSubPhase { }

        private static void CheckSelectedBomb(bool onlyDrop)
        {
            if (CurrentBomb != null)
            {
                if (onlyDrop || Selection.ThisShip.GetAvailableBombLaunchTemplates(CurrentBomb).Count == 0)
                {
                    DropBomb(); 
                }
                else
                {
                    AskWayToDropBomb();
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
                "Select the direction to drop the bomb",
                typeof(WayToDropDecisionSubPhase),
                Triggers.FinishTrigger
            );

            selectBombToDrop.AddDecision("Drop", (o, e) => { DecisionSubPhase.ConfirmDecisionNoCallback(); DropBomb(); });
            selectBombToDrop.AddDecision("Launch", LaunchBomb);

            selectBombToDrop.DescriptionShort = "Select a way how to use the device";

            selectBombToDrop.DefaultDecisionName = "Drop";

            selectBombToDrop.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private static void DropBomb()
        {
            Selection.ThisShip.CallBombWillBeDropped(StartDropBombSubphase);
        }

        private static void StartDropBombSubphase()
        {
            if (!IsOverriden)
            {
                Phases.StartTemporarySubPhaseOld(
                    "Bomb drop planning",
                    typeof(BombDropPlanningSubPhase),
                    delegate { Selection.ThisShip.CallBombWasDropped(Triggers.FinishTrigger); }
                );
            }
            else
            {
                IsOverriden = false;
                Selection.ThisShip.CallBombWasDropped(Triggers.FinishTrigger);
            }
        }

        private static void LaunchBomb(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            Phases.StartTemporarySubPhaseOld(
                "Bomb launch planning",
                typeof(BombLaunchPlanningSubPhase),
                delegate { Selection.ThisShip.CallBombWasLaunched(Triggers.FinishTrigger); }
            );
        }

        private class WayToDropDecisionSubPhase : DecisionSubPhase { }

        public static List<GenericUpgrade> GetBombsToDrop(GenericShip ship, UpgradeSubType subType = UpgradeSubType.None)
        {
            return ship.UpgradeBar.GetUpgradesOnlyFaceup()
                .Where(n => n.GetType().BaseType == typeof(GenericTimedBomb) || 
                    n.GetType().BaseType == typeof(GenericTimedBombSE) || n.GetType().BaseType == typeof(GenericContactMineSE))
                .Where(n => n.State.UsesCharges == false || (n.State.UsesCharges == true && n.State.Charges > 0))
                .Where(n => subType == UpgradeSubType.None || n.UpgradeInfo.SubType == subType)
                .ToList();
        }

        public static bool HasBombsToDrop(GenericShip ship, UpgradeSubType subType = UpgradeSubType.None)
        {
            return GetBombsToDrop(ship, subType).Any();
        }

        public static Dictionary<GenericDeviceGameObject, GenericBomb> GetBombsOnBoard()
        {
            return bombsList;
        }

    }
}



