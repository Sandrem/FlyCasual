using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT1300
    {
        public class ResistanceSympathizer : YT1300
        {
            public ResistanceSympathizer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Resistance Sympathizer",
                    3,
                    38
                );

                ShipInfo.ArcInfo.Firepower = 3;
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 5;

                ShipInfo.SubFaction = SubFaction.Resistance;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Missile);
            }
        }
    }
}