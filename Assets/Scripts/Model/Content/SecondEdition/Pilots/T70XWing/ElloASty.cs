using Movement;
using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class ElloASty : T70XWing
        {
            public ElloASty() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ello Asty",
                    5,
                    56,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ElloAstyAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                //seImageNumber: 93
                );

                //ModelInfo.SkinName = "Black One";

                ImageUrl = "http://infinitearenas.com/xw2browse/images/resistance/ello-asty.jpg";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ElloAstyAbility : Abilities.FirstEdition.ElloAstyAbility
    {
        //private const string LEFT_TALON_ROLL = "3.L.E";
        //private const string RIGHT_TALON_ROLL = "3.R.E";

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