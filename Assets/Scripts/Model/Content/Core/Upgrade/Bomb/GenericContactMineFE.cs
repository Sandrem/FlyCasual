using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;
using System.Linq;

namespace Upgrade
{

    abstract public class GenericContactMineFE : GenericBomb
    {
        private int updatesCount;
        private Action CallBack;

        private int immediateDetonationsCheckedCount;

        public GenericContactMineFE() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnGenerateActions += PerformDropBombAction;
        }

        protected virtual void PerformDropBombAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.BombDropAction()
            {
                Name = "Drop " + UpgradeInfo.Name,
                Source = this
            };
            HostShip.AddAvailableAction(action);
        }

        public override void ActivateBombs(List<GenericDeviceGameObject> bombObjects, Action callBack)
        {
            CallBack = callBack;
            CurrentBombObjects = bombObjects;

            base.ActivateBombs(bombObjects, CheckImmediateDetonation);
        }

        private void CheckImmediateDetonation()
        {
            foreach (var mineObject in CurrentBombObjects)
            {
                ObstaclesStayDetectorForced collisionChecker = mineObject.GetComponentInChildren<ObstaclesStayDetectorForced>();
                collisionChecker.ReCheckCollisionsStart();

                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
                Game.Movement.FuncsToUpdate.Add(delegate { return UpdateColisionDetection(collisionChecker); });
            }
        }

        private bool UpdateColisionDetection(ObstaclesStayDetectorForced collisionChecker)
        {
            bool isFinished = false;

            if (updatesCount > 1)
            {
                GetResults(collisionChecker);
                isFinished = true;
            }
            else
            {
                updatesCount++;
            }

            return isFinished;
        }

        private void GetResults(ObstaclesStayDetectorForced collisionChecker)
        {
            collisionChecker.ReCheckCollisionsFinish();

            if (collisionChecker.OverlapsShipNow)
            {
                List<GenericShip> shipsHitMine = collisionChecker.OverlappedShipsNow;
                CheckNumberOfShipsHit(shipsHitMine, collisionChecker.transform.parent.GetComponent<GenericDeviceGameObject>());
            }

            immediateDetonationsCheckedCount++;
            int minesDropped = string.IsNullOrEmpty(bombSidePrefabPath) ? 1 : 3;
            if (immediateDetonationsCheckedCount == minesDropped)
            {
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

                for (int i = 0; i < minesDropped; i++)
                {
                    Game.Movement.FuncsToUpdate.Remove(delegate { return UpdateColisionDetection(collisionChecker); });
                }

                immediateDetonationsCheckedCount = 0;
                Triggers.ResolveTriggers(TriggerTypes.OnBombIsDetonated, CallBack);
            }
        }

        private void CheckNumberOfShipsHit(List<GenericShip> shipsHitMine, GenericDeviceGameObject bombObject)
        {
            if (shipsHitMine.Count == 1)
            {
                RegisterMineDetonationForShip(shipsHitMine[0], bombObject);
            }
            else
            {
                Triggers.RegisterTrigger(
                    new Trigger()
                    {
                        Name = "Decide what ship triggered this mine",
                        TriggerOwner = HostShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnBombIsDetonated,
                        EventHandler = delegate { DecideWhatShipTriggeredMine(shipsHitMine, bombObject); },
                    }
                );
            }
        }

        private void DecideWhatShipTriggeredMine(List<GenericShip> shipsHitMine, GenericDeviceGameObject bombObject)
        {
            MineDetonationShipSelection subphase = Phases.StartTemporarySubPhaseNew<MineDetonationShipSelection>(
                "Decide what ship triggered this mine",
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = UpgradeInfo.Name;
            subphase.DescriptionLong = "Decide what ship triggered this mine";
            subphase.ImageSource = this;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = false;

            GenericShip enemyShipToDetonate = null;
            foreach (GenericShip ship in shipsHitMine)
            {
                if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
                {
                    if (enemyShipToDetonate == null || ship.State.HullCurrent + ship.State.ShieldsCurrent < enemyShipToDetonate.State.HullCurrent + enemyShipToDetonate.State.ShieldsCurrent)
                    {
                        enemyShipToDetonate = ship;
                    }
                }

                subphase.AddDecision(
                    ship.ShipId + ": " + ship.PilotInfo.PilotName,
                    delegate { ChooseToDetonate(ship, bombObject); },
                    ship.ImageUrl
                );
            }

            subphase.DefaultDecisionName = (enemyShipToDetonate != null) ? enemyShipToDetonate.ShipId + ": " + enemyShipToDetonate.PilotInfo.PilotName : subphase.GetDecisions().First().Name;

            subphase.Start();
        }

        private class MineDetonationShipSelection : SubPhases.DecisionSubPhase { };

        private void ChooseToDetonate(GenericShip detonatedShip, GenericDeviceGameObject bombObject)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            RegisterMineDetonationForShip(detonatedShip, bombObject);

            Triggers.ResolveTriggers(TriggerTypes.OnBombIsDetonated, Triggers.FinishTrigger);
        }

        private void RegisterMineDetonationForShip(GenericShip detonatedShip, GenericDeviceGameObject bombObject)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Damage from mine",
                TriggerOwner = HostShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnBombIsDetonated,
                EventHandler = TryDetonate,
                EventArgs = new BombDetonationEventArgs()
                {
                    DetonatedShip = detonatedShip,
                    BombObject = bombObject
                }
            });
        }

        protected override void Detonate()
        {
            RegisterDetonationTriggerForShip(BombsManager.DetonatedShip);

            base.Detonate();
        }

        public override void Discard(Action callBack)
        {
            HostShip.OnGenerateActions -= PerformDropBombAction;

            base.Discard(callBack);
        }

    }

}