using Upgrade;

namespace UpgradesList
{ 
    public class TargetingComputer : GenericActionBarUpgrade<ActionsList.TargetLockAction>
    {
        public TargetingComputer() : base()
        {
            Type = UpgradeSlot.Modification;
            Name = ShortName = "Targeting Computer";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/5/50/Target_Computer.png";
            Cost = 2;
        }
    }
}
