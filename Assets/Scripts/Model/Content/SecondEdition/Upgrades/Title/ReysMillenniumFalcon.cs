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
            HostShip.OnManeuverIsReadyToBeRevealed += AllowSegnorsLoop;
            HostShip.OnMovementFinish += RestoreFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= UseReysMillenniumFalcon;
            HostShip.OnTokenIsRemoved -= UseReysMillenniumFalcon;
            HostShip.OnManeuverIsReadyToBeRevealed -= AllowSegnorsLoop;
            HostShip.OnMovementFinish -= RestoreFlag;
        }

        private void AllowSegnorsLoop(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.SegnorsLoop)
            {
                manueversWhileStressedFlag = HostShip.CanPerformRedManeuversWhileStressed;
                if (HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
                {
                    HostShip.CanPerformRedManeuversWhileStressed = true;
                }
            }
        }

        private void RestoreFlag(GenericShip ship)
        {
            if (ship.AssignedManeuver.Bearing == Movement.ManeuverBearing.SegnorsLoop)
            {
                HostShip.CanPerformRedManeuversWhileStressed = manueversWhileStressedFlag;
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
