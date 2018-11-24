using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;

namespace Ship
{
    namespace SecondEdition.BTLS8KWing
    {
        public class BTLS8KWing : FirstEdition.KWing.KWing
        {
            public BTLS8KWing() : base()
            {
                ShipInfo.ShipName = "BTL-S8 K-wing";
                ShipInfo.Hull = 6;
                ShipInfo.Shields = 3;
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(RotateArcAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction)));

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Turret);
                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.Torpedo);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Missile);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Gunner);

                IconicPilots[Faction.Rebel] = typeof(MirandaDoni);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/58/Maneuver_k-wing.png";
            }
        }
    }
}
