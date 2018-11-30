using Upgrade;
using Ship;
using System.Collections.Generic;
using BoardTools;

namespace UpgradesList.FirstEdition
{
    public class TargetingAstromech : GenericUpgrade
    {
        public TargetingAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Astromech",
                UpgradeType.Astromech,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.FlightAssistAstromechAbility)
            );
        }
    }
}

namespace Abilities.FirstEdition
{
    public class TargetingAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterTargetingAstromech;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTargetingAstromech;
        }

        private void RegisterTargetingAstromech(GenericShip hostShip)
        {
            if (HostShip.GetLastManeuverColor() != Movement.MovementComplexity.Complex) return;
            if (Board.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignAstromechTargetingLock);
        }

        private void AssignAstromechTargetingLock(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                HostUpgrade.UpgradeInfo.Name,
                HostUpgrade
            );
        }
    }
}