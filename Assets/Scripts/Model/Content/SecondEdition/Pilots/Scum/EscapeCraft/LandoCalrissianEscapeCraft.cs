using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EscapeCraft
    {
        public class LandoCalrissianEscapeCraft : EscapeCraft
        {
            public LandoCalrissianEscapeCraft() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lando Calrissian",
                    "Smooth-talking Gambler",
                    Faction.Scum,
                    4,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LandoCalrissianScumPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    seImageNumber: 226
                );

                PilotNameCanonical = "landocalrissian-escapecraft";

                ShipAbilities.Add(new Abilities.SecondEdition.CoPilotAbility());
            }
        }
    }
}