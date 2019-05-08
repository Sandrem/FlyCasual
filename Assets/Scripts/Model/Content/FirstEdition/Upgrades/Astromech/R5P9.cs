using System;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class R5P9 : GenericUpgrade
    {
        public R5P9() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5-P9",
                UpgradeType.Astromech,
                cost: 3,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.R5P9Ability)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    // At the end of the combat phase, you may spend 1 of your focus token to
    // recover 1 shield (up to your shield value)
    public class R5P9Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers += R5P9PlanRegenShield;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers -= R5P9PlanRegenShield;
        }

        private void R5P9PlanRegenShield()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, ShowDecision);
        }

        private void ShowDecision(object sender, System.EventArgs e)
        {
            // check if this ship is stressed
            if (ShouldUseR5P9Ability())
            {
                // Select the ship in order to highlight it, so player knows which ship
                // is using the ability
                Selection.ChangeActiveShip(HostShip);
                // give user the option to use ability
                AskToUseAbility(AlwaysUseByDefault, R5P9RegenShield);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool ShouldUseR5P9Ability()
        {
            if (HostShip.Tokens.HasToken(typeof(Tokens.FocusToken))
                && (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax))
                return true;
            return false;
        }

        private void R5P9RegenShield(object sender, EventArgs e)
        {
            HostShip.Tokens.SpendToken(typeof(Tokens.FocusToken), R5P9GetShield);
        }

        private void R5P9GetShield()
        {
            if (HostShip.TryRegenShields())
            {
                Sounds.PlayShipSound("R2D2-Proud");
                Messages.ShowInfo("R5-P9 has restored 1 shield to " + HostShip.PilotInfo.PilotName);
            }
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}