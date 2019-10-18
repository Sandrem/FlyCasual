using Upgrade;
using Ship;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class R4Astromech : GenericUpgrade
    {
        public R4Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R4 Astromech",
                UpgradeType.Astromech,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.R4AstromechAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small),
                seImageNumber: 55
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R4AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += ApplyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= ApplyAbility;
        }

        private void ApplyAbility(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Speed == 1 || HostShip.AssignedManeuver.Speed == 2)
            {
                if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight || HostShip.AssignedManeuver.Bearing == ManeuverBearing.Bank || HostShip.AssignedManeuver.Bearing == ManeuverBearing.Turn)
                {
                    HostShip.AssignedManeuver.ColorComplexity = GenericMovement.ReduceComplexity(HostShip.AssignedManeuver.ColorComplexity);
                    // Update revealed dial in UI
                    Roster.UpdateAssignedManeuverDial(HostShip, HostShip.AssignedManeuver);
                    Messages.ShowInfo("R4 Astromech: Difficulty of maneuver is reduced");
                }
            }
        }
    }
}