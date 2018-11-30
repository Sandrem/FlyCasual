using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ContrabandCybernetics : GenericUpgrade
    {
        public ContrabandCybernetics() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Contraband Cybernetics",
                UpgradeType.Illicit,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.ContrabandCyberneticsAbility),
                charges: 1,
                seImageNumber: 58
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ContrabandCyberneticsAbility : Abilities.FirstEdition.ContrabandCyberneticsAbility
    {
        protected override void PayActivationCost(Action callback)
        {
            HostUpgrade.SpendCharge();
            callback();
        }

        protected override bool IsAbilityCanBeUsed()
        {
            return HostUpgrade.Charges > 0;
        }

        protected override void FinishAbility()
        {
            Triggers.FinishTrigger();
        }
    }
}