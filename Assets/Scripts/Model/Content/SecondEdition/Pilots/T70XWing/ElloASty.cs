using Movement;
using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class ElloAsty : T70XWing
        {
            public ElloAsty() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ello Asty",
                    5,
                    55,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ElloAstyAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/f77180ae05fd919a0dff2225380246a6.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ElloAstyAbility : Abilities.FirstEdition.ElloAstyAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckElloAstyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckElloAstyAbility;
        }

        private void CheckElloAstyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Bearing == ManeuverBearing.TallonRoll)
            {
                if (HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
                {
                    movement.ColorComplexity = MovementComplexity.Normal;
                }
            }
        }
    }
}