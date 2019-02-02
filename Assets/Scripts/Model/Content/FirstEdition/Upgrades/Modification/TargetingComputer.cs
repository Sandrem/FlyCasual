using Upgrade;
using Abilities;
using ActionsList;
using Actions;

namespace UpgradesList.FirstEdition
{
    public class TargetingComputer : GenericUpgrade
    {
        public TargetingComputer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Computer",
                UpgradeType.Modification,
                cost: 2,
                addAction: new ActionInfo(typeof(TargetLockAction))
            );
        }
    }
}