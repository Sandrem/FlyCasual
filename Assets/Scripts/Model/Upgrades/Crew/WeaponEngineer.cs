using Upgrade;

namespace UpgradesList
{
    public class WeaponEngineer : GenericUpgrade
    {
        public WeaponEngineer() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Weapon Engineer";
            Cost = 3;
        }
    }
}
