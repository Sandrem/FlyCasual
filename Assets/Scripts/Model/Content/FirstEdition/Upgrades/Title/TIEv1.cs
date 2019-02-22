using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class TIEv1 : GenericUpgrade
    {
        public bool IsAlwaysUse;

        public TIEv1() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TIE/v1",
                UpgradeType.Title,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.TIEAdvPrototype.TIEAdvPrototype)),
                abilityType: typeof(Abilities.FirstEdition.TIEv1Ability)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class TIEv1Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckTargetLockBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckTargetLockBonus;
        }

        private void CheckTargetLockBonus(GenericAction action)
        {
            if (action is TargetLockAction)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "TIE/v1",
                    TriggerType = TriggerTypes.OnActionDecisionSubPhaseEnd,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    Sender = HostUpgrade,
                    EventHandler = AskAssignEvade
                });
            }
        }

        private void AskAssignEvade(object sender, System.EventArgs e)
        {
            if (Selection.ThisShip.CanPerformAction(new EvadeAction()))
            {
                TIEv1DecisionSubPhase newSubPhase = (TIEv1DecisionSubPhase)Phases.StartTemporarySubPhaseNew("TIE/v1 decision", typeof(SubPhases.TIEv1DecisionSubPhase), Triggers.FinishTrigger);
                newSubPhase.TIEv1Upgrade = sender as UpgradesList.FirstEdition.TIEv1;
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

    public class TIEv1DecisionSubPhase : DecisionSubPhase
    {
        public UpgradesList.FirstEdition.TIEv1 TIEv1Upgrade;

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Perform free evade action?";

            AddDecision("Yes", PerformFreeEvadeAction);
            AddDecision("No", DontPerformFreeEvadeAction);
            AddDecision("Always", AlwaysPerformFreeEvadeAction);

            DefaultDecisionName = "Yes";

            if (!TIEv1Upgrade.IsAlwaysUse)
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
            Selection.ThisShip.AskPerformFreeAction(new EvadeAction(), DecisionSubPhase.ConfirmDecision, true);
        }

        private void DontPerformFreeEvadeAction(object sender, EventArgs e)
        {
            ConfirmDecision();
        }

        private void AlwaysPerformFreeEvadeAction(object sender, EventArgs e)
        {
            TIEv1Upgrade.IsAlwaysUse = true;

            PerformFreeEvadeAction(sender, e);
        }

    }

}