using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Countermeasures : GenericUpgrade
    {
        public Countermeasures() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Countermeasures",
                UpgradeType.Modification,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.CountermeasuresAbility),
                restriction: new BaseSizeRestriction(BaseSize.Large)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CountermeasuresAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterCountermeasuresAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterCountermeasuresAbility;
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
            Phases.Events.OnRoundEnd += DeactivateCountermeasures;

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
            Phases.Events.OnRoundEnd -= DeactivateCountermeasures;
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