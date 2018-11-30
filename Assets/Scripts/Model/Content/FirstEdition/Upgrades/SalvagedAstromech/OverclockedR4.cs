using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Tokens;
using System;
using Ship;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class OverclockedR4 : GenericUpgrade
    {
        public OverclockedR4() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Overclocked R4",
                UpgradeType.SalvagedAstromech,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.OverclockedR4Ability)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class OverclockedR4Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, Type type)
        {
            if (Phases.CurrentPhase is MainPhases.CombatPhase && type == typeof(FocusToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, AskAssignFocusToken);
            }
        }

        private void AskAssignFocusToken(object sender, EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AssignFocusToken, null, null, true);
        }

        private void AssignFocusToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), delegate
            {
                HostShip.Tokens.AssignToken(typeof(FocusToken), DecisionSubPhase.ConfirmDecision);
            });
        }
    }
}