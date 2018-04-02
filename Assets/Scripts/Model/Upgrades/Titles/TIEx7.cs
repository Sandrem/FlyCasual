using Ship;
using Ship.TIEDefender;
using Upgrade;
using System.Collections.Generic;
using System;
using UpgradesList;
using SubPhases;
using ActionsList;

namespace UpgradesList
{
    public class TIEx7 : GenericUpgradeSlotUpgrade
    {
        public bool IsAlwaysUse;

        public TIEx7() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "TIE/x7";
            Cost = -2;

            ForbiddenSlots = new List<UpgradeType>
            {
                UpgradeType.Cannon,
                UpgradeType.Missile
            };
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEDefender;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            Host.OnMovementFinish += CheckTIEx7Ability;
        }

        private void CheckTIEx7Ability(GenericShip ship)
        {
            if (Board.BoardManager.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.Speed > 2 && !ship.IsBumped && !ship.IsHitObstacles)
            {
                Triggers.RegisterTrigger(new Trigger() {
                    Name = "TIE/x7",
                    TriggerType = TriggerTypes.OnShipMovementFinish,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = AskTIEx7Ability,
                    Sender = this,
                });
            }
        }

        private void AskTIEx7Ability(object sender, System.EventArgs e)
        {
            if (Selection.ThisShip.CanPerformAction(new EvadeAction()))
            {
                TIEx7DecisionSubPhase newSubPhase = (TIEx7DecisionSubPhase)Phases.StartTemporarySubPhaseNew("TIE/x7 decision", typeof(TIEx7DecisionSubPhase), Triggers.FinishTrigger);
                newSubPhase.TIEx7Upgrade = sender as TIEx7;
                newSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}

namespace SubPhases
{

    public class TIEx7DecisionSubPhase : DecisionSubPhase
    {
        public TIEx7 TIEx7Upgrade;

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Perform free evade action?";

            AddDecision("Yes", PerformFreeEvadeAction);
            AddDecision("No", DontPerformFreeEvadeAction);
            AddDecision("Always", AlwaysPerformFreeEvadeAction);

            DefaultDecisionName = "Yes";

            if (!TIEx7Upgrade.IsAlwaysUse)
            {
                callBack();
            }
            else
            {
                PerformFreeEvadeAction(null, null);
            }
        }

        private void PerformFreeEvadeAction(object sender, EventArgs e)
        {
            Phases.CurrentSubPhase.CallBack = delegate {
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Triggers.FinishTrigger();
            };
            (new EvadeAction()).ActionTake();
        }

        private void DontPerformFreeEvadeAction(object sender, EventArgs e)
        {
            ConfirmDecision();
        }

        private void AlwaysPerformFreeEvadeAction(object sender, EventArgs e)
        {
            TIEx7Upgrade.IsAlwaysUse = true;

            PerformFreeEvadeAction(sender, e);
        }

    }

}