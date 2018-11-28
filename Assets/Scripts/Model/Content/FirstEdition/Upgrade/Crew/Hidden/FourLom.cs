using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class FourLom : GenericUpgrade
    {
        public FourLom() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "4-LOM",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restrictionFaction: Faction.Scum
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(30, 1));
        }        
    }
}