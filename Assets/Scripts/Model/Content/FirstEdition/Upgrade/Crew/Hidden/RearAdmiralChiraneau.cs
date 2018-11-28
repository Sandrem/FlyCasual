using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class RearAdmiralChiraneau : GenericUpgrade
    {
        public RearAdmiralChiraneau() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Rear Admiral Chiraneau",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restrictionFaction: Faction.Imperial
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(67, 1));
        }        
    }
}