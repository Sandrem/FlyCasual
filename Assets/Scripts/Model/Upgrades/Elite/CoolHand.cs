using Upgrade;
using UnityEngine;
using Ship;
using System;
using SubPhases;
using System.Linq;
using Abilities;
using Tokens;

namespace UpgradesList
{
    public class CoolHand : GenericUpgrade
    {
        public CoolHand() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Cool Hand";
            Cost = 1;

            UpgradeAbilities.Add(new CoolHandAbility());
        }
    }
}

namespace Abilities
{
    public class CoolHandAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= CheckTrigger;
        }

        private void CheckTrigger(GenericShip host, Type type)
        {
            if (type == typeof(Tokens.StressToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, UseCoolHandAbility);
            }
        }

        private void UseCoolHandAbility(object sender, EventArgs e)
        {
            CoolHandDecisionSubPhase decision = (CoolHandDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(CoolHandDecisionSubPhase),
                Triggers.FinishTrigger
            );

            decision.PrepareDecision(HostUpgrade);
            decision.Start();
        }
    }
}

namespace SubPhases
{

    public class CoolHandDecisionSubPhase : DecisionSubPhase
    {

        public GenericUpgrade HostUpgrade;

        public void PrepareDecision(GenericUpgrade upgrade)
        {
            HostUpgrade = upgrade;

            InfoText = string.Format("Discard \"Cool Hand\" to assign token?");

            AddDecision("Focus Token", AddFocus);
            AddDecision("Evade Token", AddEvade);

            ShowSkipButton = true;
            RequiredPlayer = upgrade.Host.Owner.PlayerNo;

            //Default AI behavior.
            DefaultDecisionName = GetDecisions().First().Name;           
        }

        private void AddFocus(object sender, System.EventArgs e)
        {
            HostUpgrade.Host.Tokens.AssignToken(typeof(FocusToken), DiscardUpgrade);
        }

        private void AddEvade(object sender, System.EventArgs e)
        {
            HostUpgrade.Host.Tokens.AssignToken(typeof(EvadeToken), DiscardUpgrade);
        }

        private void DiscardUpgrade()
        {
            HostUpgrade.TryDiscard(DecisionSubPhase.ConfirmDecision);
        }
    }
}