using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RogueClassStarfighter
    {
        public class MagnaGuardExecutioner : RogueClassStarfighter
        {
            public MagnaGuardExecutioner() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "MagnaGuard Executioner",
                    "",
                    Faction.Separatists,
                    3,
                    4,
                    4,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Cannon,
                        UpgradeType.Cannon,
                        UpgradeType.Modification,
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Droid
                    }
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/magnaguardexecutioner.png";
            }
        }
    }
}