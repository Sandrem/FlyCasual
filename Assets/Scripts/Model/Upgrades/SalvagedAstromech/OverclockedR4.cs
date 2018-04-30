using Abilities;
using System;
using Upgrade;
using Ship;
using Tokens;
using SubPhases;

namespace UpgradesList
{
    public class OverclockedR4 : GenericUpgrade
    {
        public OverclockedR4() : base()
        {
            Types.Add(UpgradeType.SalvagedAstromech);
            Name = "Overclocked R4";
            Cost = 1;

            UpgradeAbilities.Add(new OverclockedR4Ability());
        }
    }
}

namespace Abilities
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
            HostShip.Tokens.AssignToken(new StressToken(HostShip), delegate
            {
                HostShip.Tokens.AssignToken(new FocusToken(HostShip), DecisionSubPhase.ConfirmDecision);
            });
        }
    }
}