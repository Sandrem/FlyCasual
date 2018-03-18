using Upgrade;

namespace UpgradesList
{ 
    public class TargetingComputer : GenericActionBarUpgrade<ActionsList.TargetLockAction>
    {
        public TargetingComputer() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Targeting Computer";
            Cost = 2;
        }
    }
}
