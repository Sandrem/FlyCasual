﻿using Movement;
using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class CountessRyad : TIEDDefender
        {
            public CountessRyad() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Countess Ryad",
                    4,
                    82,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CountessRyadAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 124
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you would execute a straight maneuver, you may increase difficulty of the maeuver. If you do, execute it as a koiogran turn maneuver instead.
    public class CountessRyadAbility : Abilities.FirstEdition.CountessRyadAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeMovementIsExecuted += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeMovementIsExecuted -= RegisterAskChangeManeuver;
        }

        protected override void RegisterAskChangeManeuver(GenericShip ship)
        {
            //I have assumed that you can not use this ability if you execute a red maneuver
            if (HostShip.AssignedManeuver.ColorComplexity != MovementComplexity.Complex
                && HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight
                && !HostShip.AssignedManeuver.IsIonManeuver)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeMovementIsExecuted, AskChangeManeuver);
            }
        }

        protected override MovementComplexity GetNewManeuverComplexity()
        {
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
                throw new Exception("Can't increase difficulty of red maneuvers");

            return HostShip.AssignedManeuver.ColorComplexity + 1;
        }
    }
}