using Actions;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EscapeCraft
    {
        public class L337EscapeCraft : EscapeCraft
        {
            public L337EscapeCraft() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "L3-37",
                    "Droid Revolutionary",
                    Faction.Scum,
                    2,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.L337Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    },
                    seImageNumber: 228
                );

                PilotNameCanonical = "l337-escapecraft";

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ShipAbilities.Add(new Abilities.SecondEdition.CoPilotAbility());
            }
        }
    }
}