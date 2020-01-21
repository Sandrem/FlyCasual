using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTLBYWing
    {
        public class AnakinSkywalker : BTLBYWing
        {
            public AnakinSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Anakin Skywalker",
                    6,
                    60,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.ForcePower, UpgradeType.Astromech },
                    force: 3,
                    abilityText: "After you fully execute a maneuver, if there is an enemy ship in your standard front arc at range 0-1 or in your bullseye arc, you may spend 1 force to remove 1 stress token.",
                    abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility)
                );

                PilotNameCanonical = "anakinskywalker-btlbywing";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/94/5b/945bf6ea-b339-4132-b3af-a47b53b0b9f0/swz48_pilot-anakin-skywalker.png";
            }
        }
    }
}
