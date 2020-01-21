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
                    41,
                    isLimited: true,
                    force: 1,
                    abilityText: "Before you reveal your maneuver, you may spend 1 force to barrel roll (this does not count as an action).",
                    abilityType: typeof(AnakinSkywalkerNabooN1StarfighterAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "anakinskywalker-nabooroyaln1starfighter";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/01/c2/01c23677-9c23-46fd-a97d-082f3f7866fd/swz40_anakin-skywalker.png";
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
                    HostShip.PilotInfo.PilotName,
                    NeverUseByDefault,
                    PerformBarrelRollNotAction,
                    descriptionLong: "Do you want to spend 1 Force to Barrel Roll (this is not an action)?",
                    imageHolder: HostShip
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
            HostShip.AskPerformFreeAction(
                brAction,
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "You must perform Barrel Roll (this is not an action)",
                HostShip,
                isForced: true
            );
        }
    }
}
