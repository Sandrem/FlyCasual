using Ship;
using Upgrade;
using ActionsList;
using System.Linq;
using UnityEngine;
using Abilities;
using Tokens;

namespace UpgradesList
{
    public class Countermeasures : GenericUpgrade
    {
        public Countermeasures() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Countermeasures";
            Cost = 3;

            UpgradeAbilities.Add(new CountermeasuresAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return (ship.ShipBaseSize == BaseSize.Large);
        }
    }
}

namespace Abilities
{
    public class CountermeasuresAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers += RegisterCountermeasuresAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart_Triggers -= RegisterCountermeasuresAbility;
        }

        private void RegisterCountermeasuresAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskUseCountermeasuresAbility);
        }

        private void AskUseCountermeasuresAbility(object sender, System.EventArgs e)
        {
            AskToUseAbility(AIShouldUseAbility, ActivateCountermeasures);
        }

        private bool AIShouldUseAbility()
        {
            //TODO: more complex logic
            return HostShip.Tokens.HasToken(typeof(RedTargetLockToken), '*');
        }

        private void ActivateCountermeasures(object sender, System.EventArgs e)
        {
            Messages.ShowInfo(string.Format("{0} used Countermeasures", HostShip.PilotName));

            HostShip.ChangeAgilityBy(+1);
            Phases.OnRoundEnd += DeactivateCountermeasures;

            if (HostShip.Tokens.HasToken(typeof(RedTargetLockToken), '*'))
            {
                SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
                AskToRemoveTargetLock();
            }
            else
            {
                HostUpgrade.TryDiscard(SubPhases.DecisionSubPhase.ConfirmDecision);
            }
        }

        private void DeactivateCountermeasures()
        {
            Phases.OnRoundEnd -= DeactivateCountermeasures;
            HostShip.ChangeAgilityBy(-1);
        }

        private void AskToRemoveTargetLock()
        {
            CountermeasuresDecisionSubPhase selectTargetLockToDiscardDecision = (CountermeasuresDecisionSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(CountermeasuresDecisionSubPhase),
                Triggers.FinishTrigger
            );

            foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
            {
                if (token.GetType() == typeof(RedTargetLockToken))
                {
                    char letter = (token as GenericTargetLockToken).Letter;
                    selectTargetLockToDiscardDecision.AddDecision
                    (
                        "Target Lock " + letter,
                        delegate { RemoveTargetLock(letter); }
                    );
                }
            }

            selectTargetLockToDiscardDecision.InfoText = "Select target lock to remove";

            selectTargetLockToDiscardDecision.DefaultDecisionName = selectTargetLockToDiscardDecision.GetDecisions().First().Name;

            selectTargetLockToDiscardDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectTargetLockToDiscardDecision.Start();
        }

        private void RemoveTargetLock(char letter)
        {
            HostShip.Tokens.RemoveToken(
                typeof(RedTargetLockToken),
                delegate { HostUpgrade.TryDiscard(SubPhases.DecisionSubPhase.ConfirmDecision); },
                letter
            );
        }

        private class CountermeasuresDecisionSubPhase : SubPhases.DecisionSubPhase { }
    }
}
