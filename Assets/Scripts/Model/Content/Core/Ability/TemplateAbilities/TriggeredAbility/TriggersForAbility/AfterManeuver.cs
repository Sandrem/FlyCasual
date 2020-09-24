using Movement;
using Ship;
using System;
using System.Linq;
using UnityEngine;

namespace Abilities
{
    public class AfterManeuver : TriggerForAbility
    {
        private TriggeredAbility Ability;
        private Type HasToken;

        public int MinSpeed { get; }
        public int MaxSpeed { get; }
        public MovementComplexity Complexity { get; }
        public bool OnlyIfFullyExecuted { get; }
        public bool OnlyIfPartialExecuted { get; }
        public bool OnlyIfMovedThroughFriendlyShip { get; }
        public ManeuverBearing OnlyIfBearing { get; }

        public AfterManeuver(
            ManeuverSpeed minSpeed = ManeuverSpeed.Speed0,
            ManeuverSpeed maxSpeed = ManeuverSpeed.Speed5,
            MovementComplexity complexity = MovementComplexity.None,
            bool onlyIfFullyExecuted = false,
            bool onlyIfPartialExecuted = false,
            bool onlyIfMovedThroughFriendlyShip = false,
            ManeuverBearing onlyIfBearing = ManeuverBearing.None,
            Type hasToken = null
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
            OnlyIfBearing = onlyIfBearing;
            HasToken = hasToken;
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
                && BearingIsCorrect()
                && ((HasToken == null) || (Ability.HostShip.Tokens.HasToken((HasToken), '*')))
            )
            {
                Ability.RegisterAbilityTrigger
                (
                    TriggerTypes.OnMovementFinish,
                    delegate { Ability.Action.DoAction(Ability); }
                );
            }
        }

        private bool BearingIsCorrect()
        {
            bool result = false;

            if (OnlyIfBearing == ManeuverBearing.None)
            {
                result = true;
            }
            else
            {
                if (OnlyIfBearing == ManeuverBearing.SideslipAny)
                {
                    result = (Ability.HostShip.AssignedManeuver.Bearing == ManeuverBearing.SideslipBank || Ability.HostShip.AssignedManeuver.Bearing == ManeuverBearing.SideslipTurn);
                }
                else
                {
                    result = OnlyIfBearing == Ability.HostShip.AssignedManeuver.Bearing;
                }
            }

            return result;
        }
    }
}
