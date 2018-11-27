using Upgrade;
using Abilities;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class TargetingComputer : GenericUpgrade
    {
        public TargetingComputer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Computer",
                UpgradeType.Modification,
                cost: 2
            );

            UpgradeAbilities.Add(new GenericActionBarAbility<TargetLockAction>());
        }
    }
}