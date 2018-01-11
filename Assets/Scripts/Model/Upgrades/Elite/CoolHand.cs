using Upgrade;
using UnityEngine;
using Ship;
using System;
using SubPhases;
using System.Linq;

namespace UpgradesList
{
    public class CoolHand : GenericUpgrade
    {
        public CoolHand() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Cool Hand";
            Cost = 1;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnTokenIsAssigned += RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip host, Type type)
        {
            if (type.FullName.Equals(typeof(Tokens.StressToken).ToString()))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = Name,
                    TriggerType = TriggerTypes.OnTokenIsAssigned,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = CoolHandsAbility
                });
            }
        }

        private void CoolHandsAbility(object sender, EventArgs e)
        {
            CoolHandDecisionSubPhase decision = (CoolHandDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(CoolHandDecisionSubPhase),
                Triggers.FinishTrigger);

            decision.PrepareDecision(this);
        }
    }
}

namespace SubPhases
{

    public class CoolHandDecisionSubPhase : DecisionSubPhase
    {

        public void PrepareDecision(GenericUpgrade upgrade)
        {
            InfoText = string.Format("Discard {0} for one of the following tokens?", Name);

            AddDecision("Focus Token", delegate { AddFocus(upgrade); });
            AddDecision("Evade Token", delegate { AddEvade(upgrade); });

            ShowSkipButton = true;
            RequiredPlayer = upgrade.Host.Owner.PlayerNo;

            //Default AI behavior.
            DefaultDecision = GetDecisions().First().Key;

            Start();            
        }

        private void AddFocus(GenericUpgrade upgrade)
        {
            upgrade.Host.AssignToken(new Tokens.FocusToken(), ConfirmDecision);
            upgrade.TryDiscard(Phases.CurrentSubPhase.Resume);
        }

        private void AddEvade(GenericUpgrade upgrade)
        {
            upgrade.Host.AssignToken(new Tokens.EvadeToken(), ConfirmDecision);
            upgrade.TryDiscard(Phases.CurrentSubPhase.Resume);
        }
    }
}