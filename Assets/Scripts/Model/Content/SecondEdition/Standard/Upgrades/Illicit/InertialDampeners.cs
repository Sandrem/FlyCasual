using Content;
using Ship;
using System.Collections.Generic;
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
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.InertialDampenersAbility),
                seImageNumber: 61,
                legalityInfo: new List<Legality> { Legality.StandartBanned }
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