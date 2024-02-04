using Actions;
using ActionsList;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TargetingComputer : GenericUpgrade
    {
        public TargetingComputer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Targeting Computer",
                UpgradeType.Modification,
                cost: 1,
                addAction: new ActionInfo(typeof(TargetLockAction))
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/0b/d7/0bd7d42f-4401-4f58-9f9e-a5856e6c94f1/swz47_upgrade-targeting-computer.png";
        }
    }
}