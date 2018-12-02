using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class TargetingCoordinator : GenericUpgrade
    {
        public TargetingCoordinator() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Coordinator",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true
            );

            Avatar = new AvatarInfo(Faction.None, new Vector2(39, 1));
        }        
    }
}