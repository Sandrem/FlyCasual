using Ship;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class InertialDampeners : GenericUpgrade
    {
        public InertialDampeners() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Inertial Dampeners",
                UpgradeType.Illicit,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.InertialDampenersAbility),
                seImageNumber: 61
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class InertialDampenersAbility : FirstEdition.InertialDampenersAbility
    {
        protected override void CheckAbility(GenericShip ship)
        {
            if (HostShip.State.ShieldsCurrent > 0) RegisterTrigger();
        }

        protected override void FinishAbility()
        {
            HostShip.LoseShield();
            Triggers.FinishTrigger();
        }
    }
}