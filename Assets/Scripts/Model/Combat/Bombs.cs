using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;
using UnityEngine;
using Ship;
using SubPhases;
using BoardTools;
using Remote;

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
        public static GenericUpgrade CurrentDevice { get; set; }
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
            CurrentDevice = null;
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
            BombsManager.CurrentDevice = bombUpgrade;
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
            if (OnBombIsRemoved != null) OnBombIsRemoved(CurrentDevice as GenericBomb, CurrentBombObject);

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

            if (OnCheckPermissionToDetonate != null) OnCheckPermissionToDetonate(CurrentDevice as GenericBomb, DetonatedShip);

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

        public static void CheckBombDropAvailabilitySystemPhase(GenericShip ship, ref bool flag)
        {
            if (!ship.IsBombAlreadyDropped && HasBombsToDrop(ship)) flag = true;
        }

        public static void RegisterBombDropAvailabilitySystemPhase(GenericShip ship)
        {
            if (!ship.IsBombAlreadyDropped && HasBombsToDrop(ship))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ask which bomb to drop",
                    TriggerType = TriggerTypes.OnMovementActivationStart,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = (object sender, EventArgs e) => CreateAskBombDropSubPhase((sender as GenericShip)),
                    Sender = ship
                });
            }
        }

        public static void CheckBombDropAvailabilityGeneral(GenericShip ship)
        {
            RegisterBombDropTriggerIfAvailable(ship, TriggerTypes.OnMovementActivationStart);
        }

        public static void RegisterBombDropTriggerIfAvailable(GenericShip ship, TriggerTypes triggerType, UpgradeSubType subType = UpgradeSubType.None, Type type = null, bool onlyDrop = false, bool isRealDrop = true)
        {
            if ((!isRealDrop || !ship.IsBombAlreadyDropped) && HasBombsToDrop(ship, subType, type))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Ask which bomb to drop",
                    TriggerType = TriggerTypes.OnMovementActivationStart,
                    TriggerOwner = ship.Owner.PlayerNo,
                    EventHandler = (object sender, EventArgs e) => CreateAskBombDropSubPhase((sender as GenericShip), subType, type, onlyDrop),
                    Sender = ship
                });
            }
        }

        public static void CreateAskBombDropSubPhase(GenericShip ship, UpgradeSubType subType = UpgradeSubType.None, Type type = null, bool onlyDrop = false)
        {
            Selection.ChangeActiveShip("ShipId:" + ship.ShipId);

            BombDecisionSubPhase selectBombToDrop = (BombDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select a device to drop",
                typeof(BombDecisionSubPhase),
                delegate { CheckSelectedDevice(onlyDrop); }
            );

            selectBombToDrop.DefaultDecisionName = "None";

            foreach (GenericUpgrade deviceInstalled in GetBombsToDrop(Selection.ThisShip, subType, type))
            {
                selectBombToDrop.AddDecision(
                    deviceInstalled.UpgradeInfo.Name,
                    delegate { SelectBomb(deviceInstalled); }
                );

                if (type != null && deviceInstalled.GetType() == type) selectBombToDrop.DefaultDecisionName = deviceInstalled.UpgradeInfo.Name;
            }

            selectBombToDrop.IsForced = selectBombToDrop.DefaultDecisionName != "None";

            selectBombToDrop.AddDecision(
                "None",
                delegate { SelectBomb(null); }
            );

            selectBombToDrop.DescriptionShort = "Select a device to drop";

            selectBombToDrop.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            selectBombToDrop.Start();
        }

        private static void SelectBomb(GenericUpgrade device)
        {
            CurrentDevice = device;
            DecisionSubPhase.ConfirmDecision();
        }

        private class BombDecisionSubPhase : DecisionSubPhase { }

        private static void CheckSelectedDevice(bool onlyDrop)
        {
            if (CurrentDevice != null)
            {
                if (onlyDrop || Selection.ThisShip.GetAvailableDeviceLaunchTemplates(CurrentDevice).Count == 0)
                {
                    DropDevice(); 
                }
                else
                {
                    AskWayToDropDevice();
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private static void AskWayToDropDevice()
        {
            WayToDropDecisionSubPhase subphase = (WayToDropDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                "Select the direction to drop the bomb",
                typeof(WayToDropDecisionSubPhase),
                Triggers.FinishTrigger
            );

            if (Selection.ThisShip.GetAvailableBombDropTemplates(CurrentDevice).Count != 0)
            {
                subphase.AddDecision("Drop", (o, e) => { DecisionSubPhase.ConfirmDecisionNoCallback(); DropDevice(); });
            }
            subphase.AddDecision("Launch", LaunchBomb);

            subphase.DescriptionShort = "Select a way how to use the device";

            subphase.DefaultDecisionName = "Drop";

            subphase.RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            subphase.Start();
        }

        private static void DropDevice()
        {
            Selection.ThisShip.CallDeviceWillBeDropped(StartDropDeviceSubphase);
        }

        private static void StartDropDeviceSubphase()
        {
            if (!IsOverriden)
            {
                Phases.StartTemporarySubPhaseOld(
                    "Device drop planning",
                    typeof(BombDropPlanningSubPhase),
                    delegate { Selection.ThisShip.CallDeviceWasDropped(Triggers.FinishTrigger); }
                );
            }
            else
            {
                IsOverriden = false;
                Selection.ThisShip.CallDeviceWasDropped(Triggers.FinishTrigger);
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

        public static List<GenericUpgrade> GetBombsToDrop(GenericShip ship, UpgradeSubType subType = UpgradeSubType.None, Type type = null)
        {
            return ship.UpgradeBar.GetUpgradesOnlyFaceup()
                .Where(n => n.GetType().BaseType == typeof(GenericTimedBomb) || n.GetType().BaseType == typeof(GenericTimedBombSE) || n.GetType().BaseType == typeof(GenericContactMineSE) || n.UpgradeInfo.SubType == UpgradeSubType.Remote)
                .Where(n => n.State.UsesCharges == false || (n.State.UsesCharges == true && n.State.Charges > 0))
                .Where(n => subType == UpgradeSubType.None || n.UpgradeInfo.SubType == subType)
                .Where(n => type == null || n.GetType() == type)
                .ToList();
        }

        public static bool HasBombsToDrop(GenericShip ship, UpgradeSubType subType = UpgradeSubType.None, Type type = null)
        {
            return GetBombsToDrop(ship, subType, type).Any();
        }

        public static Dictionary<GenericDeviceGameObject, GenericBomb> GetBombsOnBoard()
        {
            return bombsList;
        }

    }
}



