using Upgrade;

namespace UpgradesList
{ 
    public class TargetingComputer : GenericActionBarUpgrade<ActionsList.TargetLockAction>
    {
        public TargetingComputer() : base()
        {
            Type = UpgradeType.Modification;
            Name = ShortName = "Targeting Computer";
            Cost = 2;
        }
    }
}
