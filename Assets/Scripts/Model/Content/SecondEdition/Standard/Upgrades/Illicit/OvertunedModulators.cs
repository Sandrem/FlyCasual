using Ship;
using SubPhases;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class OvertunedModulators : GenericUpgrade
    {
        public OvertunedModulators() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Overtuned Modulators",
                UpgradeType.Illicit,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.OvertunedModulatorsAbility),
                charges: 1
            );

            ImageUrl = "https://i.imgur.com/WC14X2N.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class OvertunedModulatorsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivation += RegisterOwnTrigger;
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;

            HostShip.OnTokenIsRemoved += CheckPenalty;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterOwnTrigger;
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;

            HostShip.OnTokenIsRemoved -= CheckPenalty;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (CanBeActivated()) flag = true;
        }

        private bool CanBeActivated()
        {
            return !HostShip.IsStressed
                && HostUpgrade.State.Charges > 0;
        }

        private void RegisterOwnTrigger(GenericShip ship)
        {
            if (CanBeActivated()) RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToActivate);
        }

        private void AskToActivate(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostUpgrade.UpgradeInfo.Name,
                ShouldUse,
                ActivateOvertunedModulators,
                descriptionLong: "Do you want to gain 3 Calculate Tokens?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private bool ShouldUse()
        {
            return HostShip.State.HullCurrent <= 1
                && ActionsHolder.CountEnemiesTargeting(HostShip) > 0;
        }

        private void ActivateOvertunedModulators(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name} are used by {HostShip.PilotInfo.PilotName}");
            HostUpgrade.State.SpendCharge();

            HostShip.Tokens.AssignTokens(CreateCalculateToken, 3, Triggers.FinishTrigger);
        }

        private GenericToken CreateCalculateToken()
        {
            return new CalculateToken(HostShip);
        }

        private void CheckPenalty(GenericShip ship, GenericToken token)
        {
            if (Phases.CurrentPhase is MainPhases.EndPhase
                && HostUpgrade.State.Charges == 0
                && token.TokenColor == TokenColors.Green)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsRemoved, GetStrainToken);
            }
        }

        private void GetStrainToken(object sender, EventArgs e)
        {
            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: Green tokens are removed, Strain tokens are gained");

            HostShip.Tokens.AssignToken(typeof(StrainToken), Triggers.FinishTrigger);
        }
    }
}