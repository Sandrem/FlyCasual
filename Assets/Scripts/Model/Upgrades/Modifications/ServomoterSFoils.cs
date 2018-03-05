using Ship;
using Upgrade;
using Abilities;
using Ship.XWing;

namespace UpgradesList
{
    public class ServomoterSFoils : GenericUpgrade
    {
        public ServomoterSFoils() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Servomoter S-Foils";
            Cost = 0;

            UpgradeAbilities.Add(new ServomoterSFoilsAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is XWing;
        }
    }

}

namespace Abilities
{
    public class ServomoterSFoilsAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnShipIsPlaced += TurnSFoilsToFlightPositionAlt;
            Phases.OnCombatPhaseStart += HostShip.WingsOpen;
            Phases.OnCombatPhaseEnd += HostShip.WingsClose;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsPlaced -= TurnSFoilsToFlightPositionAlt;
            Phases.OnCombatPhaseStart -= HostShip.WingsOpen;
            Phases.OnCombatPhaseEnd -= HostShip.WingsClose;
        }

        private void TurnSFoilsToFlightPositionAlt(GenericShip ship)
        {
            HostShip.WingsClose();
        }
    }
}
