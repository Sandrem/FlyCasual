using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Intimidation : GenericUpgrade
    {
        public Intimidation() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Intimidation",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.IntimidationAbility)
            );
        }        
    }
}