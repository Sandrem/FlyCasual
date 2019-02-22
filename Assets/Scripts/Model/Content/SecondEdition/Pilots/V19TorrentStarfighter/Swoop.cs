using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Swoop : V19TorrentStarfighter
    {
        public Swoop()
        {
            PilotInfo = new PilotCardInfo(
                "\"Swoop\"",
                3,
                44,
                true,
                abilityType: typeof(Abilities.SecondEdition.SwoopAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c3/9f/c39f4623-a983-4fea-98aa-c11b37e867c0/swz32_swoop.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly small or medium ship fully executes a speed 3-4 maneuver, if it is at range 0-1, it may perform a red boost action.
    public class SwoopAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishSuccessfullyGlobal -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (ship.Owner == HostShip.Owner
                && (ship.ShipInfo.BaseSize == BaseSize.Small || ship.ShipInfo.BaseSize == BaseSize.Medium)
                && (ship.AssignedManeuver.Speed == 3 || ship.AssignedManeuver.Speed == 4)
                && new BoardTools.DistanceInfo(ship, HostShip).Range <= 1)
            {
                TargetShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskPerformBoostAction);
            }
        }

        private void AskPerformBoostAction(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman(HostName + ": you may perform a red boost action");

            TargetShip.AskPerformFreeAction(
                new List<GenericAction>() { new BoostAction() { Color = ActionColor.Red } },
                Triggers.FinishTrigger
            );
        }
    }
}
