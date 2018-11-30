using Upgrade;
using System.Collections.Generic;
using Ship;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class Selfless : GenericUpgrade
    {
        public Selfless() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Selfless",
                UpgradeType.Elite,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.SelflessAbility),
                restriction: new FactionRestriction(Faction.Rebel),
                seImageNumber: 15
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class SelflessAbility : Abilities.FirstEdition.DrawTheirFireAbility
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