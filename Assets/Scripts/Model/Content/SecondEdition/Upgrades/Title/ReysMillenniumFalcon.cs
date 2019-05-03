using Ship;
using Upgrade;
using ActionsList;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class ReysMillenniumFalcon : GenericUpgrade
    {
        public ReysMillenniumFalcon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rey's Millennium Falcon",
                UpgradeType.Title,
                cost: 5,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new ShipRestriction(typeof(Ship.SecondEdition.ScavengedYT1300.ScavengedYT1300)),
                    new FactionRestriction(Faction.Resistance)
                ),
                abilityType: typeof(Abilities.SecondEdition.ReysMillenniumFalconAbility)//,
                //seImageNumber: 103
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/23627443c7f5e7447c306ea7c6242634.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ReysMillenniumFalconAbility : GenericAbility
    {
        private bool set = false;
        private bool manueversWhileStressedFlag;

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += UseReysMillenniumFalcon;
            HostShip.OnTokenIsRemoved += UseReysMillenniumFalcon;
            HostShip.OnTryCanPerformRedManeuverWhileStressed += AllowRedSegnorsLoopWhileStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= UseReysMillenniumFalcon;
            HostShip.OnTokenIsRemoved -= UseReysMillenniumFalcon;
            HostShip.OnTryCanPerformRedManeuverWhileStressed -= AllowRedSegnorsLoopWhileStressed;
        }

        private void AllowRedSegnorsLoopWhileStressed(ref bool isAllowed)
        {
            if ((HostShip.AssignedManeuver != null)
                && (HostShip.AssignedManeuver.Bearing == Movement.ManeuverBearing.SegnorsLoop)
                && (HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
            )
            {
                isAllowed = true;
            }
        }

        private void UseReysMillenniumFalcon(GenericShip ship, System.Type type)
        {
            if (type == typeof(StressToken))
            {
                if (!set && HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
                {
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(BoostAction));
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(RotateArcAction));
                    set = true;
                }
                else if (set && HostShip.Tokens.CountTokensByType(typeof(StressToken)) > 2)
                {
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(BoostAction));
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(RotateArcAction));
                    set = false;
                }
            }
        }
    }
}
