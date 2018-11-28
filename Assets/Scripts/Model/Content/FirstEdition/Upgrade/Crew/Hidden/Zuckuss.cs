using Ship;
using Upgrade;
using UnityEngine;
using System.Linq;

namespace UpgradesList.FirstEdition
{
    public class Zuckuss : GenericUpgrade
    {
        public Zuckuss() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Zuckuss",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restrictionFaction: Faction.Scum
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(79, 1));
        }        
    }
}