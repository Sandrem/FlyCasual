using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Ship;
using System;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class SheathipedeClassShuttle : FirstEdition.SheathipedeClassShuttle.SheathipedeClassShuttle
        {
            public SheathipedeClassShuttle() : base()
            {
                ShipInfo.ActionIcons.RemoveActions(typeof(TargetLockAction));

                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.ReverseStraight), MovementComplexity.Complex);

                IconicPilots[Faction.Rebel] = typeof(FennRau);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/0/03/Maneuver_sheathipede.png";

                ShipAbilities.Add(new Abilities.SecondEdition.CommsShuttle());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommsShuttle : GenericAbility
    {
        public override string Name => "Comms Shuttle";

        private bool CoordinateActionWasAdded;

        public override void ActivateAbility()
        {
            HostShip.OnDocked += ApplyAbility;
            HostShip.OnUndocked += RemoveAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDocked -= ApplyAbility;
            HostShip.OnUndocked -= RemoveAbility;
        }

        private void ApplyAbility(GenericShip ship)
        {
            if (!ship.ActionBar.HasAction(typeof(CoordinateAction)))
            {
                CoordinateActionWasAdded = true;
                ship.ActionBar.AddGrantedAction(new CoordinateAction(), null);
            }

            HostShip.DockingHost.OnMovementActivationStart += RegisterFreeAction;
        }

        private void RegisterFreeAction(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskFreeCoordinate);
        }

        private void AskFreeCoordinate(object sender, EventArgs e)
        {
            HostShip.DockingHost.AskPerformFreeAction(
                new CoordinateAction(),
                Triggers.FinishTrigger,
                descriptionShort: Name,
                descriptionLong: "Before your carrier ship activates, it can perfrom a Coordinate action"
            );
        }

        private void RemoveAbility(GenericShip ship)
        {
            if (CoordinateActionWasAdded)
            {
                CoordinateActionWasAdded = false;
                ship.ActionBar.RemoveGrantedAction(typeof(CoordinateAction), null);
            }

            HostShip.DockingHost.OnMovementActivationStart -= RegisterFreeAction;
        }
    }
}
