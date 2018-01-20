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
            Type = UpgradeType.Modification;
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
            Phases.OnCombatPhaseStart += RegisterCountermeasuresAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.OnCombatPhaseStart -= RegisterCountermeasuresAbility;
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
            return HostShip.HasToken(typeof(RedTargetLockToken), '*');
        }

        private void ActivateCountermeasures(object sender, System.EventArgs e)
        {
            HostShip.ChangeAgilityBy(+1);
            Phases.OnRoundEnd += DeactivateCountermeasures;

            if (HostShip.HasToken(typeof(RedTargetLockToken), '*'))
            {
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
                SubPhases.DecisionSubPhase.ConfirmDecision
            );

            foreach (GenericToken token in HostShip.GetAllTokens())
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

            selectTargetLockToDiscardDecision.DefaultDecision = selectTargetLockToDiscardDecision.GetDecisions().First().Key;

            selectTargetLockToDiscardDecision.RequiredPlayer = HostShip.Owner.PlayerNo;

            selectTargetLockToDiscardDecision.Start();
        }

        private void RemoveTargetLock(char letter)
        {
            HostShip.RemoveToken(typeof(RedTargetLockToken), letter);
            HostUpgrade.TryDiscard(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private class CountermeasuresDecisionSubPhase : SubPhases.DecisionSubPhase { }
    }
}
