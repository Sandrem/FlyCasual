using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class YsanneIsard : GenericUpgrade
    {
        public YsanneIsard() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Ysanne Isard",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restrictionFaction: Faction.Imperial
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(21, 1));
        }        
    }
}