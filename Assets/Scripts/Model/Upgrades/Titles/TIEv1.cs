using Ship;
using Ship.TIEAdvPrototype;
using UnityEngine;
using Upgrade;
using ActionsList;
using System;
using SubPhases;
using UpgradesList;
using Abilities;

namespace UpgradesList
{
    public class TIEv1 : GenericUpgrade
    {
        public bool IsAlwaysUse;

        public TIEv1() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "TIE/v1";
            Cost = 1;

            UpgradeAbilities.Add(new TIEv1Ability());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is TIEAdvPrototype;
        }
    }
}

namespace Abilities
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
                newSubPhase.TIEv1Upgrade = sender as TIEv1;
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
        public TIEv1 TIEv1Upgrade;

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
            Phases.CurrentSubPhase.CallBack = delegate{
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
            TIEv1Upgrade.IsAlwaysUse = true;

            PerformFreeEvadeAction(sender, e);
        }

    }

}

