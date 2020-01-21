using Arcs;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class TechnoUnionBomber : HyenaClassDroidBomber
    {
        public TechnoUnionBomber()
        {
            PilotInfo = new PilotCardInfo(
                "Techno Union Bomber",
                1,
                25,
                extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Torpedo, UpgradeType.Missile, UpgradeType.Device }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/7b/20/7b2032d2-dc96-4b69-9c59-92c809ba3da5/swz41_techno-union-bomber.png";
        }
    }
}