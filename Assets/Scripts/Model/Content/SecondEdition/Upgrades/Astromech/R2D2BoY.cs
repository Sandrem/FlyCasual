using Upgrade;
using UnityEngine;
using Ship;
using System.Collections.Generic;
using System;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class R2D2BoY : GenericUpgrade
    {
        public R2D2BoY() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2AstromechBoYAbility),
                charges: 2
            );

            ImageUrl = "https://i.imgur.com/pgrul0D.jpg";

            NameCanonical = "r2d2-battleofyavin";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you reveal your dial, you may spend 1 charge and gain 1 disarm token to recover 1 shield.
    public class R2AstromechBoYAbility : GenericAbility
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
            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax && HostUpgrade.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                RegenShield,
                descriptionLong: "Do you want to spend 1 Charge and gain 1 Disarm Token to recover 1 shield?"
            );
        }

        private void RegenShield(object sender, EventArgs e)
        {
            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), () =>
            {
                if (HostShip.TryRegenShields())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfo(HostName + " causes " + HostShip.PilotInfo.PilotName + " to recover 1 shield and gain 1 disarm token");
                }
                Triggers.FinishTrigger();
            });
        }
    }
}