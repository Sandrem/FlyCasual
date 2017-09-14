using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;
using System.Linq;

namespace Upgrade
{

    public class GenericBomb : GenericUpgrade
    {
        public string bombPrefabPath;
        public bool IsDiscardedAfterDropped;
        public bool IsDropAfterManeuverRevealed;
        public bool IsDropAsAction;
        public bool IsDetonationPlanned;
        public bool IsDetonationByContact;

        public GameObject BombObject { get; private set; }

        public GenericBomb() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            if (IsDropAfterManeuverRevealed) host.OnManeuverIsRevealed += RegisterAskDropBomb;
            if (IsDropAsAction) host.AfterGenerateAvailableActionsList += PerformDropBombAction;
        }

        public virtual void PayDropCost(Action callBack)
        {
            if (IsDiscardedAfterDropped) Discard();

            callBack();
        }

        public virtual void RegisterAskDropBomb(GenericShip host)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = Name + " : Ask to drop",
                TriggerOwner = host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnManeuverIsRevealed,
                EventHandler = AskDropBomb
            });
        }

        public virtual void AskDropBomb(object sender, System.EventArgs e)
        {
            BombsManager.CurrentBombToDrop = this;
            Phases.StartTemporarySubPhase(
                Name,
                typeof(SubPhases.DropBombDecisionSubPhase),
                delegate { Triggers.FinishTrigger(); }
            );
        }

        public virtual void PerformDropBombAction(GenericShip ship)
        {
            ActionsList.GenericAction action = new ActionsList.BombDropAction()
            {
                Name = "Drop " + Name,
                Source = this
            };
            Host.AddAvailableAction(action);
        }

        public void ActivateBomb(GameObject bombObject)
        {
            BombObject = bombObject;
            Host.IsBombAlreadyDropped = true;
            Discard();

            if (IsDetonationPlanned)
            {
                Phases.OnActivationPhaseEnd += PlanTimedDetonation;
            }
            else if (IsDetonationByContact)
            {
                BombsManager.RegisterMine(bombObject, this);
            }
        }

        private void PlanTimedDetonation()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Detonation of " + Name,
                TriggerType = TriggerTypes.OnActivationPhaseEnd,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = Detonate,
                Sender = BombObject
            });
        }

        public virtual void Detonate(object sender, EventArgs e)
        {
            Messages.ShowError("BOOM!!!");

            if (IsDetonationPlanned)
            {
                Phases.OnActivationPhaseEnd -= PlanTimedDetonation;
                foreach (var ship in GetShipsInRange())
                {
                    RegisterDetonationTriggerForShip(ship);
                }
            }
            else if (IsDetonationByContact)
            {
                BombsManager.UnregisterMine(BombObject);
                RegisterDetonationTriggerForShip(sender as GenericShip);
            }
            GameObject.Destroy(BombObject);

            Triggers.ResolveTriggers(TriggerTypes.OnBombDetonated, Triggers.FinishTrigger);
        }

        private void RegisterDetonationTriggerForShip(GenericShip ship)
        {
            Triggers.RegisterTrigger(new Trigger
            {
                Name = ship.ShipId + " : Detonation of " + Name,
                TriggerType = TriggerTypes.OnBombDetonated,
                TriggerOwner = ship.Owner.PlayerNo,
                EventHandler = delegate
                {
                    ExplosionEffect(ship, Triggers.FinishTrigger);
                }
            });
        }

        public virtual List<GenericShip> GetShipsInRange()
        {
            List<GenericShip> result = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Select(n => n.Value))
            {
                if (!ship.IsDestroyed)
                {
                    if (IsShipInDetonationRange(ship))
                    {
                        result.Add(ship);
                    }
                }
            }

            return result;
        }

        private bool IsShipInDetonationRange(GenericShip ship)
        {
            List<Vector3> bombPoints = BombsManager.GetBombPoints();

            foreach (var localBombPoint in bombPoints)
            {
                Vector3 globalBombPoint = BombObject.transform.TransformPoint(localBombPoint);
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


        public virtual void ExplosionEffect(GenericShip ship, Action callBack)
        {
            callBack();
        }

    }

}

namespace SubPhases
{

    public class DropBombDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Drop " + Phases.CurrentSubPhase.Name + "?";

            AddDecision("Yes", DropBomb);
            AddDecision("No", SkipDropBomb);

            defaultDecision = "No";
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhase(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                ConfirmDecision
            );
        }

        private void SkipDropBomb(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
