using Abilities;
using ActionsList;
using Upgrade;

namespace UpgradesList
{ 
    public class TargetingComputer : GenericUpgrade
    {
        public TargetingComputer() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Targeting Computer";
            Cost = 2;

            UpgradeAbilities.Add(new GenericActionBarAbility<TargetLockAction>());
        }
    }
}
