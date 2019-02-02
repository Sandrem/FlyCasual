using Ship;
using Upgrade;
using UnityEngine;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class IG88D : GenericUpgrade
    {
        public IG88D() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "IG-88D",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.FirstEdition.Ig2000Ability)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(44, 2));
        }        
    }
}