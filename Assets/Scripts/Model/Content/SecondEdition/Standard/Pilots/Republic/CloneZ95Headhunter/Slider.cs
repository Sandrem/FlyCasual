using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Slider : CloneZ95Headhunter
        {
            public Slider() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Slider\"",
                    "Slider",
                    Faction.Republic,
                    4,
                    3,
                    8,
                    isLimited: true,
                    charges: 2,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.SliderAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/slider.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SliderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckRevealedManeuved;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckRevealedManeuved;
        }

        private void CheckRevealedManeuved(GenericShip ship)
        {
            if (ship.State.Charges > 1
                && ship.RevealedManeuver != null
                && ship.RevealedManeuver.Bearing == Movement.ManeuverBearing.Bank
                && ship.RevealedManeuver.Speed == 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, RegisterSliderAbility);
            }
        }

        private void RegisterSliderAbility(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                DoSideSlip,
                descriptionLong: "Do you want to perform sideslip instead?",
                imageHolder: HostShip
            );
        }

        private void DoSideSlip(object sender, EventArgs e)
        {
            GenericMovement movement = new SideslipBankMovement(
                HostShip.RevealedManeuver.Speed,
                HostShip.RevealedManeuver.Direction,
                ManeuverBearing.SideslipBank,
                HostShip.RevealedManeuver.ColorComplexity
            );

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Maneuver is changed to Sideslip");
            HostShip.SetAssignedManeuver(movement);

            HostShip.SpendCharges(2);

            Triggers.FinishTrigger();
        }
    }
}