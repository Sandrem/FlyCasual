using Abilities;
using Ship;
using Upgrade;
using Board;

namespace UpgradesList
{

    public class TargetingAstromech : GenericUpgrade
    {

        public TargetingAstromech() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "Targeting Astromech";
            Cost = 2;

            UpgradeAbilities.Add(new TargetingAstromechAbility());
        }
    }
}

namespace Abilities
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
            if (HostShip.GetLastManeuverColor() != Movement.ManeuverColor.Red) return;
            if (BoardManager.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AssignAstromechTargetingLock);            
        }

        private void AssignAstromechTargetingLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Targeting Astromech: Aquire a Target Lock");
            Sounds.PlayShipSound("Astromech-Beeping-and-whistling");

            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                HostUpgrade.Name,
                HostUpgrade.ImageUrl
            );
        }
    }
}