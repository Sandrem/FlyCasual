using Abilities.SecondEdition;
using ActionsList;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class AnakinSkywalker : NabooRoyalN1Starfighter
        {
            public AnakinSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Anakin Skywalker",
                    4,
                    45,
                    isLimited: true,
                    force: 1,
                    abilityText: "Before you reveal your maneuver, you may spend 1 force to barrel roll (this does not count as an action).",
                    abilityType: typeof(AnakinSkywalkerNabooN1StarfighterAbility),
                    extraUpgradeIcon: UpgradeType.Force
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AnakinSkywalkerNabooN1StarfighterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsReadyToBeRevealed -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.Force > 0) RegisterAbilityTrigger(TriggerTypes.OnManeuverIsReadyToBeRevealed, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, System.EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                AskToUseAbility(
                    NeverUseByDefault,
                    PerformBarrelRollNotAction,
                    infoText: "Do you want to spend 1 force to barrel roll (this does not count as an action)?"
                );
            }
            else
            {
                Messages.ShowError("No Force to use for Anakin Skywalker's ability");
                Triggers.FinishTrigger();
            }
        }

        private void PerformBarrelRollNotAction(object sender, System.EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.State.Force--;
            Sounds.PlayShipSound("Ill try spinning - thats a good trick!");

            BarrelRollAction brAction = new BarrelRollAction { IsRealAction = false };
            HostShip.AskPerformFreeAction(brAction, Triggers.FinishTrigger, isForced: true);
        }
    }
}
