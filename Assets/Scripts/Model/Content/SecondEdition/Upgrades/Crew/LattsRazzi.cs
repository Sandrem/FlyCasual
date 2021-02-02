using Ship;
using Upgrade;
using System.Linq;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class LattsRazzi : GenericUpgrade
    {
        public LattsRazzi() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Latts Razzi",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.FirstEdition.LattsRazziCrewAbility),
                seImageNumber: 135
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(376, 8)
            );
        }        
    }
}