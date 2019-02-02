using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class C3PO : GenericUpgrade
    {
        public C3PO() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "C-3PO",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(47, 1));
        }        
    }
}