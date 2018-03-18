using Upgrade;

namespace UpgradesList
{
    public class WeaponEngineer : GenericUpgrade
    {
        public WeaponEngineer() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Weapon Engineer";
            Cost = 3;

            IsHidden = true;
        }
    }
}
