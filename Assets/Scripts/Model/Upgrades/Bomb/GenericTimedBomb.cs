using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;
using System.Linq;

namespace Upgrade
{

    abstract public class GenericTimedBomb : GenericBomb
    {
        public GenericTimedBomb() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnManeuverIsRevealed += RegisterAskDropBomb;
        }

        private void RegisterAskDropBomb(GenericShip host)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = Name + " : Ask to drop",
                TriggerOwner = host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnManeuverIsRevealed,
                EventHandler = AskDropBomb
            });
        }

        private void AskDropBomb(object sender, System.EventArgs e)
        {
            BombsManager.CurrentBomb = this;
            Phases.StartTemporarySubPhase(
                Name,
                typeof(SubPhases.DropBombDecisionSubPhase),
                delegate { Triggers.FinishTrigger(); }
            );
        }

        public override void ActivateBomb(GameObject bombObject, Action callBack)
        {
            Phases.OnActivationPhaseEnd += PlanTimedDetonation;

            base.ActivateBomb(bombObject, callBack);
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

        public override void Detonate(object sender, EventArgs e)
        {
            Phases.OnActivationPhaseEnd -= PlanTimedDetonation;
            foreach (var ship in GetShipsInRange())
            {
                RegisterDetonationTriggerForShip(ship);
            }
        
            base.Detonate(sender, e);
        }

        private List<GenericShip> GetShipsInRange()
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

        public override void Discard(Action callBack)
        {
            Host.OnManeuverIsRevealed -= RegisterAskDropBomb;

            base.Discard(callBack);
        }

    }

}

namespace SubPhases
{

    public class DropBombDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            if (!Selection.ThisShip.IsBombAlreadyDropped)
            {
                infoText = "Drop " + Phases.CurrentSubPhase.Name + "?";

                AddDecision("Yes", DropBomb);
                AddDecision("No", SkipDropBomb);

                defaultDecision = "No";

                callBack();
            }
            else
            {
                SkipDropBomb(null, null);
            }
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
