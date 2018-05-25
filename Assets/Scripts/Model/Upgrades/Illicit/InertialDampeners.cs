using Upgrade;
using RuleSets;

namespace UpgradesList
{
    public class InertialDampeners : GenericUpgrade
    {
        public InertialDampeners() : base()
        {
            Types.Add(UpgradeType.Illicit);
            Name = "Inertial Dampeners";
            Cost = 2;

            ImageUrl = "https://i.imgur.com/3NIASof.png";

            UpgradeRuleType = typeof(SecondEdition);
        }
    }
}