using Movement;
using Ship;
using System.Linq;

namespace Abilities
{
    public class AfterManeuver : TriggerForAbility
    {
        private TriggeredAbility Ability;
        public int MinSpeed { get; }
        public int MaxSpeed { get; }
        public MovementComplexity Complexity { get; }
        public bool OnlyIfFullyExecuted { get; }
        public bool OnlyIfPartialExecuted { get; }
        public bool OnlyIfMovedThroughFriendlyShip { get; }

        public AfterManeuver(
            ManeuverSpeed minSpeed = ManeuverSpeed.Speed0,
            ManeuverSpeed maxSpeed = ManeuverSpeed.Speed5,
            MovementComplexity complexity = MovementComplexity.None,
            bool onlyIfFullyExecuted = false,
            bool onlyIfPartialExecuted = false,
            bool onlyIfMovedThroughFriendlyShip = false
        )
        {
            ManeuverHolder minSpeedHolder = new ManeuverHolder() { Speed = minSpeed };
            MinSpeed = minSpeedHolder.SpeedIntSigned;

            ManeuverHolder maxSpeedHolder = new ManeuverHolder() { Speed = maxSpeed };
            MaxSpeed = maxSpeedHolder.SpeedIntSigned;
            Complexity = complexity;
            OnlyIfFullyExecuted = onlyIfFullyExecuted;
            OnlyIfPartialExecuted = onlyIfPartialExecuted;
            OnlyIfMovedThroughFriendlyShip = onlyIfMovedThroughFriendlyShip;
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
            if (ship.AssignedManeuver.Speed >= MinSpeed
                && ship.AssignedManeuver.Speed <= MaxSpeed
                && (Complexity == MovementComplexity.None || ship.AssignedManeuver.ColorComplexity == Complexity)
                && (OnlyIfFullyExecuted == false || (OnlyIfFullyExecuted && ship.CheckSuccessOfManeuver()))
                && (OnlyIfPartialExecuted == false || (OnlyIfPartialExecuted && ship.IsBumped))
                && (OnlyIfMovedThroughFriendlyShip == false || (OnlyIfMovedThroughFriendlyShip && ship.ShipsMovedThrough.Any(n => n.Owner.PlayerNo == Ability.HostShip.Owner.PlayerNo)))
            )
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
