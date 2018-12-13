using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class DeadmansSwitch : GenericUpgrade
    {
        public DeadmansSwitch() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Dead Man's Switch",
                UpgradeType.Illicit,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.DeadmansSwitchAbility)
            );
        }        
    }
}
