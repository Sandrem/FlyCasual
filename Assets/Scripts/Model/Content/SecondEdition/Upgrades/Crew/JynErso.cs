using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class JynErso : GenericUpgrade
    {
        public JynErso() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jyn Erso",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.JynErsoAbility),
                seImageNumber: 85
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class JynErsoAbility : FirstEdition.JanOrsCrewAbility
    {
        protected override void MarkAbilityAsUsed()
        {
            // Do nothing
        }
    }
}
