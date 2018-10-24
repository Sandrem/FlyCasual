using Ship;
using Ship.TIEDefender;
using Upgrade;
using System.Collections.Generic;
using System;
using UpgradesList;
using SubPhases;
using ActionsList;
using Abilities;

namespace UpgradesList
{
    public class TIEx7 : GenericUpgradeSlotUpgrade
    {
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

            UpgradeAbilities.Add(new TIEx7Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEDefender;
        }
    }
}

namespace Abilities
{
    public class TIEx7Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckTIEx7Ability;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckTIEx7Ability;
        }

        private void CheckTIEx7Ability(GenericShip ship)
        {
            string abilityName = "";
            if (RuleSets.RuleSet.Instance is RuleSets.FirstEdition)
            {
                if (ship.IsHitObstacles) return;
                abilityName = "TIE/x7";
            }
            else
            {
                abilityName = "Full Throttle";
            }

            if (ship.AssignedManeuver.Speed > 2)
            {
                Triggers.RegisterTrigger(new Trigger() {
                    Name = abilityName,
                    TriggerType = TriggerTypes.OnMovementFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = AskTIEx7Ability,
                    Sender = HostUpgrade,
                });
            }
        }

        private void AskTIEx7Ability(object sender, System.EventArgs e)
        {
            if (Selection.ThisShip.CanPerformAction(new EvadeAction()))
            {
                TIEx7DecisionSubPhase newSubPhase = (TIEx7DecisionSubPhase)Phases.StartTemporarySubPhaseNew("TIE/x7 decision", typeof(TIEx7DecisionSubPhase), Triggers.FinishTrigger);
                newSubPhase.TIEx7AbilityInstance = this;
                newSubPhase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public bool IsAlwaysUseAbility()
        {
            return alwaysUseAbility;
        }

        public void SetIsAlwaysUseAbility()
        {
            alwaysUseAbility = true;
        }
    }
}

namespace SubPhases
{

    public class TIEx7DecisionSubPhase : DecisionSubPhase
    {
        public TIEx7Ability TIEx7AbilityInstance;

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Perform free evade action?";

            AddDecision("Yes", PerformFreeEvadeAction);
            AddDecision("No", DontPerformFreeEvadeAction);
            AddDecision("Always", AlwaysPerformFreeEvadeAction);

            DefaultDecisionName = "Yes";

            if (!TIEx7AbilityInstance.IsAlwaysUseAbility())
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
            TIEx7AbilityInstance.SetIsAlwaysUseAbility();

            PerformFreeEvadeAction(sender, e);
        }

    }

}