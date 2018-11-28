using Ship;
using Upgrade;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class LattsRazzi : GenericUpgrade
    {
        public LattsRazzi() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Latts Razzi",
                UpgradeType.Crew,
                cost: 7,
                isLimited: true,
                restrictionFaction: Faction.Scum,
                abilityType: typeof(Abilities.FirstEdition.LattsRazziCrewAbility),
                seImageNumber: 135
            );
        }        
    }
}