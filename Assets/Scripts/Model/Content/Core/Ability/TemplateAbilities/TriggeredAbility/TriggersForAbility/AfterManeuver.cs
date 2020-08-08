using Movement;
using Ship;

namespace Abilities
{
    public class AfterManeuver : TriggerForAbility
    {
        private TriggeredAbility Ability;
        public int MinSpeed { get; }
        public int MaxSpeed { get; }

        public AfterManeuver(ManeuverSpeed minSpeed, ManeuverSpeed maxSpeed)
        {
            ManeuverHolder minSpeedHolder = new ManeuverHolder() { Speed = minSpeed };
            MinSpeed = minSpeedHolder.SpeedIntSigned;

            ManeuverHolder maxSpeedHolder = new ManeuverHolder() { Speed = maxSpeed };
            MaxSpeed = maxSpeedHolder.SpeedIntSigned;
        }

        public override void Register(TriggeredAbility ability)
        {
            Ability = ability;
            ability.HostShip.OnMovementFinish += CheckConditions;
        }

        public override void Unregister(TriggeredAbility ability)
        {
            ability.HostShip.OnMovementFinish -= CheckConditions;
        }

        private void CheckConditions(GenericShip ship)
        {
            if (ship.AssignedManeuver.Speed >= MinSpeed && ship.AssignedManeuver.Speed <= MaxSpeed)
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnMovementFinish,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }
    }
}
