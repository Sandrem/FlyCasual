using Upgrade;
using Ship;
using Movement;
using System;

namespace UpgradesList.SecondEdition
{
    public class R4PAstromech : GenericUpgrade
    {
        public R4PAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R4-P Astromech",
                UpgradeType.Astromech,
                cost: 4,
                charges: 2,
                abilityType: typeof(Abilities.SecondEdition.R4PAstromechAbility),
                restriction: new FactionRestriction(Faction.Republic)
                //seImageNumber: ?
            );
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f2/b0/f2b08b38-29fa-4be1-b96d-c09a5ac4bc7c/swz32_r4-p_astromech.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //Before you execute a basic maneuver, you may spend 1 charge. If you do, while you execute that maneuver, reduce its difficulty.
    public class R4PAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivation, AskAbility);
        }

        private void AskAbility(object sender, EventArgs e)
        {
            if (HostShip.AssignedManeuver.IsBasicManeuver && HostUpgrade.State.Charges > 0)
            {
                AskToUseAbility(
                    AlwaysUseByDefault,
                    UseAbility
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                HostShip.AssignedManeuver.ColorComplexity = GenericMovement.ReduceComplexity(HostShip.AssignedManeuver.ColorComplexity);
                HostUpgrade.State.SpendCharge();
            }
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}