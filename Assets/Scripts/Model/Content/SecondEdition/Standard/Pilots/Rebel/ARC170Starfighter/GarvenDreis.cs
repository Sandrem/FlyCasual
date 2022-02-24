using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ARC170Starfighter
    {
        public class GarvenDreis : ARC170Starfighter
        {
            public GarvenDreis() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Garven Dreis",
                    "Red Leader",
                    Faction.Rebel,
                    4,
                    5,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.GarvenDreisArcAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 66
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GarvenDreisArcAbility : GarvenDreisXWingAbility
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            // Change the ability to work at r3.
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }
    }
}