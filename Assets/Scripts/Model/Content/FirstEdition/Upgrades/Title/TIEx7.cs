using Ship;
using Upgrade;
using System.Collections.Generic;
using System;
using SubPhases;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class TIEx7 : GenericUpgrade
    {
        public TIEx7() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TIE/x7",
                UpgradeType.Title,
                cost: -2,
                forbidSlots: new List<UpgradeType>()
                {
                    UpgradeType.Cannon,
                    UpgradeType.Missile
                },
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEDefender.TIEDefender)),
                abilityType: typeof(Abilities.FirstEdition.TIEx7Ability)
            );
        }        
    }
}


namespace Abilities.FirstEdition
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
            if (RuleSets.Edition.Current is RuleSets.FirstEdition)
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
                Triggers.RegisterTrigger(new Trigger()
                {
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
        public Abilities.FirstEdition.TIEx7Ability TIEx7AbilityInstance;

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
