using Editions;
using System;
using System.Linq;
using Upgrade;

namespace SquadBuilderNS
{
    public class UpgradeRecord
    {
        public GenericUpgrade Instance { get; }
        public string UpgradeName => Instance.UpgradeInfo.Name;
        public string UpgradeNameCanonical => Instance.NameCanonical;
        public string UpgradeTypeName => Instance.GetType().ToString();
        public UpgradeType UpgradeType => Instance.UpgradeInfo.UpgradeTypes.First();
        public bool IsAllowedForSquadBuilder => Instance.IsAllowedForSquadBuilder();

        public UpgradeRecord(Type type)
        {
            Instance = (GenericUpgrade)System.Activator.CreateInstance(type);
            Edition.Current.AdaptUpgradeToRules(Instance);
        }
    }
}
