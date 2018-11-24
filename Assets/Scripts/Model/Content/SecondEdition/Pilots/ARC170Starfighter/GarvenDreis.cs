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
                PilotInfo = new PilotCardInfo(
                    "Garven Dreis",
                    4,
                    51,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.GarvenDreisAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 66
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GarvenDreisAbility : Abilities.FirstEdition.GarvenDreisAbility
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            // Change the ability to work at r3.
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }
    }
}