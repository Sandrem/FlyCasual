using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class MaraJade : GenericUpgrade
    {
        public MaraJade() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Mara Jade",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(39, 1));
        }        
    }
}