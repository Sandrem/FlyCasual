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
        private enum SFoilsPositions { Attack, Flight};
        private SFoilsPositions currentSFoilsPosition = SFoilsPositions.Attack;

        public override void ActivateAbility()
        {
            HostShip.OnShipIsPlaced += TurnSFoilsToFlightPositionAlt;
            Phases.OnCombatPhaseStart += TurnSFoilsToAttackPosition;
            Phases.OnCombatPhaseEnd += TurnSFoilsToFlightPosition;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsPlaced -= TurnSFoilsToFlightPositionAlt;
            Phases.OnCombatPhaseStart -= TurnSFoilsToAttackPosition;
            Phases.OnCombatPhaseEnd -= TurnSFoilsToFlightPosition;
        }

        private void TurnSFoilsToAttackPosition()
        {
            if (currentSFoilsPosition != SFoilsPositions.Attack)
            {
                HostShip.WingsOpen();
                currentSFoilsPosition = SFoilsPositions.Attack;
            }
        }

        private void TurnSFoilsToFlightPositionAlt(GenericShip ship)
        {
            TurnSFoilsToFlightPosition();
        }

        private void TurnSFoilsToFlightPosition()
        {
            if (currentSFoilsPosition != SFoilsPositions.Flight)
            {
                HostShip.WingsClose();
                currentSFoilsPosition = SFoilsPositions.Flight;
            }
        }
    }
}
