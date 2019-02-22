using Upgrade;
using Ship;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class SawGerreraCrew : GenericUpgrade
    {
        public SawGerreraCrew() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Saw Gerrera",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.SawGerreraCrewAbility),
                seImageNumber: 93
            );
        }        
    }
}