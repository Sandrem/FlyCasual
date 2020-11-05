using Upgrade;
using Ship;
using System;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class R2D2Resistance : GenericUpgrade
    {
        public R2D2Resistance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 6,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2D2ResistanceAbility),
                restriction: new FactionRestriction(Faction.Resistance),
                charges: 4
            );

            NameCanonical = "r2d2-resistance";

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/1c/97/1c971472-6fea-493b-ac8f-888fc6363c84/swz68_r2d2.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R2D2ResistanceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, CheckConditions);
            }
        }

        private void CheckConditions(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0
                && HostShip.State.ShieldsCurrent > 0
                && HostShip.Tokens.HasTokenByColor(Tokens.TokenColors.Red)
            )
            {
                AskToRemoveToken();
            }
            else if (HostUpgrade.State.Charges >= 2
                && HostShip.State.ShieldsCurrent == 0
            )
            {
                AskToRegenShields();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AskToRemoveToken()
        {
            Selection.ChangeActiveShip(HostShip);

            R2D2RemoveRedTokenAbilityDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<R2D2RemoveRedTokenAbilityDecisionSubPhase>(
                "R2-D2: Choose Token To Remove",
                Triggers.FinishTrigger
            );

            subphase.AbilityHostShip = HostShip;
            subphase.AbilityHostUpgrade = HostUpgrade;

            subphase.Start();
        }

        private void AskToRegenShields()
        {
            AskToUseAbility(
                "R2-D2",
                AlwaysUseByDefault,
                RegenShield,
                descriptionLong: "Do you want to spend 2 charges and gain 1 Deplete token to restore 1 Shield",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void RegenShield(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharges(2);
            if (HostShip.TryRegenShields()) { Messages.ShowInfo($"R2-D2: {HostShip.PilotInfo.PilotName} restored 1 shield"); }
            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), Triggers.FinishTrigger);
        }
    }
}

namespace SubPhases
{
    public class R2D2RemoveRedTokenAbilityDecisionSubPhase : RemoveRedTokenDecisionSubPhase
    {
        public GenericShip AbilityHostShip;
        public GenericUpgrade AbilityHostUpgrade;

        public override void PrepareCustomDecisions()
        {
            DescriptionShort = "R2-D2";
            DescriptionLong = "You may spend 1 charge and 1 shield to remove 1 red token";

            DecisionOwner = AbilityHostShip.Owner;
            DefaultDecisionName = "None";
        }

        public override void DoCustomFinishDecision()
        {
            AbilityHostUpgrade.State.SpendCharge();
            AbilityHostShip.LoseShield();

            base.DoCustomFinishDecision();
        }
    }
}