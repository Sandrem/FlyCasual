using Upgrade;
using RuleSets;
using BoardTools;
using Ship;

namespace UpgradesList
{
    public class Selfless : GenericUpgrade
    {
        public Selfless() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Selfless";
            Cost = 3;

            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.Selfless());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class Selfless : DrawTheirFireAbility
        {
            protected override bool AbilityCanBeUsed()
            {
                bool result = base.AbilityCanBeUsed();

                if (result)
                {
                    ShotInfo shotInfo = new ShotInfo(Combat.Attacker, HostShip, Combat.Attacker.PrimaryWeapon);
                    if (!shotInfo.InArc) result = false;
                }

                return result;
            }
        }
    }
}
