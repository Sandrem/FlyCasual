using Actions;
using ActionsList;
using Ship;
using SubPhases;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class InterceptBoosterAttached : GenericDualUpgrade
    {
        public InterceptBoosterAttached() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Intercept Booster (Attached)",
                UpgradeType.Configuration,
                cost: 0,
                charges: 3,
                regensChargesCount: -1,
                addAction: new ActionInfo(typeof(SlamAction), source: this),
                addActionLink: new LinkedActionInfo(typeof(SlamAction), typeof(TargetLockAction), ActionColor.Red),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.DroidTriFighter.DroidTriFighter)),
                abilityType: typeof(Abilities.SecondEdition.InterceptBoosterAttachedAbility)
            );

            AnotherSide = typeof(InterceptBoosterDetached);
            SelectSideOnSetup = false;

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0d/3f/0d3f071a-1c8e-416f-9c0e-1195ca31c102/swz81_upgrade_intercept-booster_attached.png"; ;
        }
    }

    public class InterceptBoosterDetached : GenericDualUpgrade
    {
        public InterceptBoosterDetached() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Intercept Booster (Detached)",
                UpgradeType.Configuration,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.DroidTriFighter.DroidTriFighter))
            );

            AnotherSide = typeof(InterceptBoosterAttached);

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c5/49/c549df37-e99d-4746-a34c-d42d74d854b4/swz81_upgrade_intercept-booster_detached.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class InterceptBoosterAttachedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivation += RegisterSystemAbilityActivation;
            HostShip.OnCheckSystemsAbilityActivation += HasAbility;

            Phases.Events.OnEndPhaseStart_NoTriggers += CheckEndPhaseAbility;
            Phases.Events.OnCheckSystemSubphaseCanBeSkipped += CheckSystemPhaseSkip;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterSystemAbilityActivation;
            HostShip.OnCheckSystemsAbilityActivation -= HasAbility;

            Phases.Events.OnEndPhaseStart_NoTriggers -= CheckEndPhaseAbility;
            Phases.Events.OnCheckSystemSubphaseCanBeSkipped -= CheckSystemPhaseSkip;
        }

        private void RegisterSystemAbilityActivation(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToFlip);
        }

        private void AskToFlip(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Interceptor Booster (Attached)",
                AlwaysUseByDefault,
                useAbility: FlipThisCard,
                dontUseAbility: GainDisarmToken,
                descriptionLong: "Do you want to flip this card (otherwise you will gain 1 disarm token)",
                imageHolder: HostUpgrade,
                showSkipButton: false,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void FlipThisCard(object sender, EventArgs e)
        {
            (HostUpgrade as GenericDualUpgrade).Flip();

            DecisionSubPhase.ConfirmDecision();
        }

        private void GainDisarmToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.Tokens.AssignToken(
                typeof(Tokens.WeaponsDisabledToken),
                Triggers.FinishTrigger
            );
        }

        private void HasAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        private void CheckEndPhaseAbility()
        {
            if (HostUpgrade.State.Charges == 0)
            {
                (HostUpgrade as GenericDualUpgrade).Flip();
            }
        }

        private void CheckSystemPhaseSkip(ref bool canBeSkipped)
        {
            canBeSkipped = false;
        }
    }
}