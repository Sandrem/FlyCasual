using Upgrade;
using Ship;
using RuleSets;

namespace UpgradesList
{
    public class Fearless : GenericUpgrade
    {
        public Fearless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Fearless";
            Cost = 1;

            UpgradeRuleType = typeof(SecondEdition);
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}