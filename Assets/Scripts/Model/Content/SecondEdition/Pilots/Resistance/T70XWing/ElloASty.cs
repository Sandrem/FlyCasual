﻿using Content;
using Movement;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class ElloAsty : T70XWing
        {
            public ElloAsty() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ello Asty",
                    "Born to Ill",
                    Faction.Resistance,
                    5,
                    4,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ElloAstyAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Astromech,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ElloAstyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity += CheckElloAstyAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGetManeuverColorDecreaseComplexity -= CheckElloAstyAbility;
        }

        private void CheckElloAstyAbility(GenericShip ship, ref ManeuverHolder movement)
        {
            if (movement.Bearing == ManeuverBearing.TallonRoll)
            {
                if (HostShip.Tokens.CountTokensByType(typeof(StressToken)) <= 2)
                {
                    movement.ColorComplexity = MovementComplexity.Normal;
                }
            }
        }
    }
}