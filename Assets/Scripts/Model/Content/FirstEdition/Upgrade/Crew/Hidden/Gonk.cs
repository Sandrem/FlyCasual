using Ship;
using Upgrade;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class Gonk : GenericUpgrade
    {
        public Gonk() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Gonk",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restrictionFaction: Faction.Scum
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(20, 1));
        }        
    }
}