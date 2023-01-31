using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class ZebOrrelios : AttackShuttle
        {
            public ZebOrrelios() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Zeb\" Orrelios",
                    "Spectre-4",
                    Faction.Rebel,
                    2,
                    3,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZebOrreliosPilotAbility),
                    tags: new List<Tags>
                    {
                        Tags.Spectre
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Turret,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 37,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}