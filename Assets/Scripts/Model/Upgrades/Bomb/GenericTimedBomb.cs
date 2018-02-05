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
            Phases.StartTemporarySubPhaseOld(
                Name,
                typeof(SubPhases.DropBombDecisionSubPhase),
                delegate { Triggers.FinishTrigger(); }
            );
        }

        public override void ActivateBombs(List<GameObject> bombObjects, Action callBack)
        {
            Phases.OnActivationPhaseEnd += PlanTimedDetonation;

            base.ActivateBombs(bombObjects, callBack);
        }

        private void PlanTimedDetonation()
        {
            foreach (var bombObject in BombObjects)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Detonation of " + Name,
                    TriggerType = TriggerTypes.OnActivationPhaseEnd,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = Detonate,
                    EventArgs = new BombDetonationEventArgs()
                    {
                        BombObject = bombObject
                    }
                });
            }
        }

        public override void Detonate(object sender, EventArgs e)
        {
            GameObject bombObject = (e as BombDetonationEventArgs).BombObject;
            Phases.OnActivationPhaseEnd -= PlanTimedDetonation;
            foreach (var ship in BombsManager.GetShipsInRange(bombObject))
            {
                RegisterDetonationTriggerForShip(ship);
            }
        
            base.Detonate(sender, e);
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
                InfoText = "Drop " + Phases.CurrentSubPhase.Name + "?";

                if (Selection.ThisShip.CanLaunchBombs)
                {
                    AddDecision("Drop", DropBomb);
                    AddDecision("Launch", LaunchBomb);
                }
                else
                {
                    AddDecision("Yes", DropBomb);
                }

                AddDecision("No", SkipDropBomb);
                DefaultDecision = "No";

                callBack();
            }
            else
            {
                SkipDropBomb(null, null);
            }
        }

        private void DropBomb(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb drop planning",
                typeof(BombDropPlanningSubPhase),
                ConfirmDecision
            );
        }

        private void LaunchBomb(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Bomb launch planning",
                typeof(BombLaunchPlanningSubPhase),
                ConfirmDecision
            );
        }

        private void SkipDropBomb(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

    }

}
