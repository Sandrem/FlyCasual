using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class HanSolo : GenericUpgrade
    {
        public HanSolo() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Han Solo",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(88, 2));
        }        
    }
}