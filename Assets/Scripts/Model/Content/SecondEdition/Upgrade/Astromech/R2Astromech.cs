using Upgrade;
using UnityEngine;
using Ship;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class R2Astromech : GenericUpgrade
    {
        public R2Astromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2 Astromech",
                UpgradeType.Astromech,
                cost: 6,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechAbility),
                restrictionFaction: Faction.Rebel,
                charges: 2,
                seImageNumber: 53
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you reveal your dial, you may spend 1 charge and gain 1 disarm token to recover 1 shield.
    public class R2AstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += PlanRegenShield;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= PlanRegenShield;
        }

        private void PlanRegenShield(GenericShip host)
        {
            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax && HostUpgrade.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, RegenShield, null, null, false, HostName + ": Spend 1 charge and gain 1 disarm token to recover 1 shield?");
        }

        private void RegenShield(object sender, EventArgs e)
        {
            HostUpgrade.SpendCharge();
            HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () =>
            {
                if (HostShip.TryRegenShields())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfo(HostName + ": Shield is restored");
                }
                Triggers.FinishTrigger();
            });
        }
    }
}