using Actions;
using ActionsList;
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
                PilotInfo = new PilotCardInfo(
                    "L3-37",
                    2,
                    22,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.L337Ability),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 228
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}