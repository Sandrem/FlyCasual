using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Anakin Skywalker",
                    "Hero of the Republic",
                    Faction.Republic,
                    6,
                    6,
                    23,
                    isLimited: true,
                    force: 3,
                    abilityText: "After you fully execute a maneuver, if there is an enemy ship in your standard front arc at range 0-1 or in your bullseye arc, you may spend 1 force to remove 1 stress token.",
                    abilityType: typeof(Abilities.SecondEdition.AnakinSkywalkerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Turret,
                        UpgradeType.Torpedo,
                        UpgradeType.Gunner,
                        UpgradeType.Astromech,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Jedi,
                        Tags.LightSide,
                        Tags.YWing
                    }
                );

                PilotNameCanonical = "anakinskywalker-btlbywing";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/94/5b/945bf6ea-b339-4132-b3af-a47b53b0b9f0/swz48_pilot-anakin-skywalker.png";
            }
        }
    }
}
